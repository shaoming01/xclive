using System.Data;
using System.Text.RegularExpressions;
using SchemaBuilder.Svc.Core;
using SchemaBuilder.Svc.Core.Cache;
using SchemaBuilder.Svc.Core.KeyLock;
using SchemaBuilder.Svc.Models.Table;
using ServiceStack.OrmLite;

namespace SchemaBuilder.Svc.Svc;

public class TtsModelSvc
{
    private static readonly int MaxCachedModelCount = 2;
    private static readonly Dictionary<string, DateTime> ModelLastUsageDic = new(); //每个模型最后一次使用时间

    public static R<VoiceBuildResponseVm> BuildVoice(VoiceBuildRequestVm req)
    {
        using var db = Db.Open();
        var model = db.SingleById<TtsModel>(req.TtsModelId);
        if (model == null)
        {
            return R.Faild<VoiceBuildResponseVm>("模型Id无效，刷新后重新选择语音模型");
        }

        var modelIdR = GetModelId(model);
        if (!modelIdR.Success)
        {
            return R.Faild<VoiceBuildResponseVm>("加载模型失败" + modelIdR.Message);
        }

        try
        {
            var client = new TtsApiClient();
            var voiceRe = client.GetVoice(new VoiceRequest()
            {
                model_id = modelIdR.Data,
                text = ConvertToVoiceText(req.Content),
            }).Result;
            if (voiceRe.status > 0)
            {
                return R.Faild<VoiceBuildResponseVm>("生成语音失败" + voiceRe.detail);
            }

            return R.OK(new VoiceBuildResponseVm()
            {
                Voice = voiceRe.Data,
            });
        }
        catch (Exception e)
        {
            Log4.Log.Error(e);
            return R.Faild<VoiceBuildResponseVm>("生成语音失败" + e.Message);
        }
        finally
        {
            ModelLastUsageDic[model.Name ?? ""] = DateTime.Now;
        }
    }

    /// <summary>
    /// 试听模型
    /// </summary>
    /// <param name="modelId"></param>
    /// <returns></returns>
    public static R<VoiceBuildResponseVm> ListenVoice(long modelId)
    {
        using var db = Db.Open();
        var model = db.SingleById<TtsModel>(modelId);
        if (model == null)
        {
            return R.Faild<VoiceBuildResponseVm>("模型Id不存在");
        }

        var path = $"{model.Name}/ck.wav";
        var client = new TtsApiClient();
        var re = client.GetVoiceFile(new VoiceFileRequest()
        {
            filename = path,
        }).Result;
        if (re.status > 0)
        {
            return R.Faild<VoiceBuildResponseVm>(re.detail);
        }

        return R.OK(new VoiceBuildResponseVm()
        {
            Voice = re.Data,
        });
    }

    private static string ConvertToVoiceText(string reqContent)
    {
        var reg = @"\[[^\[\]]+\]";
        return Regex.Replace(reqContent, reg, "");
    }

    private static R<string> GetModelId(TtsModel model)
    {
        return GetModelIdInternal(model);
    }

    public static R<string> GetModelIdInternal(TtsModel model)
    {
        try
        {
            var client = new TtsApiClient();
            var resp = client.GetModelInfo().Result;
            foreach (var pair in resp)
            {
                if (pair.Value.config_path.StartsWith(model.Name ?? ""))
                {
                    return R.OK(pair.Key);
                }
            }

            Log4.Log.Info("开始获取加载锁:" + model.ModelPath);
            using var modelLoad = KeyLockHelper.ModelUseLock.Lock(""); //全局锁，加载模型即使是不同模型也不要并发

            //移除不用模型
            if (resp.Keys.Count >= MaxCachedModelCount)
            {
                var modelIds = resp.Keys.ToList().OrderBy(l =>
                {
                    var modelName = resp[l].config_path.Replace("-config.json", "");
                    if (ModelLastUsageDic.TryGetValue(modelName, out DateTime lastUsage))
                    {
                        return lastUsage.Ticks;
                    }
                    else
                    {
                        return 0;
                    }
                }).ToList(); //按最后使用时间排序
                for (int i = 0; i < resp.Keys.Count - MaxCachedModelCount + 1; i++)
                {
                    var modelName = resp[modelIds[i]].config_path.Replace("-config.json", "");
                    using var modelUse = KeyLockHelper.ModelUseLock.Lock(modelName);
                    if (!IsModelLoaded(modelName))
                    {
                        continue;
                    }

                    Log4.Log.Info("开始卸载模型:" + modelName);
                    var rr = client.DeleteModel(new ModelDeleteReq()
                    {
                        model_id = modelIds[i]
                    }).Result;
                    if (rr.status > 0)
                    {
                        return R.Faild<string>("语音模型达到缓存数，清理失败" + rr.detail);
                    }
                }
            }

            //加载当前模型
            Log4.Log.Info("开始加载模型:" + model.ModelPath);
            var ar = client.AddModel(new ModelAddRequest()
            {
                model_path = model.ModelPath ?? "",
                port = 64000 + (int)model.Id,
            }).Result;
            if (ar.status > 0)
            {
                return R.Faild<string>($"加载语音模型{model.ModelPath}失败" + ar.detail);
            }

            Log4.Log.Info("成功加载模型:" + model.ModelPath);

            return R.OK(ar.Data.model_id);
        }
        catch (Exception e)
        {
            Log4.Log.Error(e);
            return R.Faild<string>(e.ToString());
        }
    }

    private static bool IsModelLoaded(string modelName)
    {
        var client = new TtsApiClient();
        var resp = client.GetModelInfo().Result;
        foreach (var pair in resp)
        {
            if (pair.Value.config_path.StartsWith(modelName ?? ""))
            {
                return true;
            }
        }

        return false;
    }

    public class ModelUsageDto
    {
        /// <summary>
        /// 最后使用的日期，使用日期较早说明可以优先释放
        /// </summary>
        public DateTime LastUsed { get; set; }

        /// <summary>
        /// 正在使用的数量，如果是0，代表这个模型目前没人在用，可以释放的
        /// </summary>
        public int UsingCount { get; set; }
    }

    public class ModelUsage : IDisposable
    {
        private readonly string _modelName;

        public ModelUsage(string modelName)
        {
            _modelName = modelName;
            //更新最后使用时间，使用数+1
            using var ll = KeyLockHelper.ModelUsageUpdatelock.Lock(_modelName);
            var key = $"ModelUsage:{modelName}";
            var usage = CacheHelper.Get<ModelUsageDto>(0, key);
            if (usage == null)
            {
                usage = new ModelUsageDto()
                {
                    LastUsed = DateTime.Now,
                    UsingCount = 0
                };
            }
            else
            {
                usage.LastUsed = DateTime.Now;
                usage.UsingCount++;
            }

            CacheHelper.SetKey(0, key, usage);
        }

        public void Dispose()
        {
            //更新最后使用时间，使用数-1，因为我已经用完了
            using var ll = KeyLockHelper.ModelUsageUpdatelock.Lock(_modelName);
            var key = $"ModelUsage:{_modelName}";
            var usage = CacheHelper.Get<ModelUsageDto>(0, key);
            if (usage == null)
            {
                usage = new ModelUsageDto()
                {
                    LastUsed = DateTime.Now,
                    UsingCount = 0
                };
            }
            else
            {
                usage.LastUsed = DateTime.Now;
                usage.UsingCount--;
            }

            CacheHelper.SetKey(0, key, usage);
        }
    }
}