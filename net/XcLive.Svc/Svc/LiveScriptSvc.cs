using System.Text.RegularExpressions;
using SchemaBuilder.Svc.Core;
using SchemaBuilder.Svc.Core.Cache;
using SchemaBuilder.Svc.Core.Ext;
using SchemaBuilder.Svc.Helpers;
using SchemaBuilder.Svc.Models.Dto;
using SchemaBuilder.Svc.Models.Table;
using ServiceStack.OrmLite;

namespace SchemaBuilder.Svc.Svc;

public class LiveScriptSvc
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="aiVerticalAnchorId"></param>
    /// <param name="chatText">如果提供了Chat内容则是Chat类型</param>
    /// <param name="interactText">如果提供了互动消息，则是互动类型</param>
    /// <param name="user"></param>
    /// <returns></returns>
    public static R<LiveScriptVoiceDetail[]> BuildAiVerticalAnchorScript(long aiVerticalAnchorId, string? chatText,
        string? interactText, UserLoginInfo user)
    {
        var scriptRe = CreateLiveScript(aiVerticalAnchorId, chatText, interactText, user);
        if (!scriptRe.Success)
        {
            return R.Faild<LiveScriptVoiceDetail[]>(scriptRe.Message);
        }

        var buildRe = BuildScript(scriptRe.Data.Id);
        if (!buildRe.Success)
        {
            return R.Faild<LiveScriptVoiceDetail[]>(buildRe.Message);
        }

        using var db = Db.Open();
        var list = db.Select<LiveScriptVoiceDetail>(d => d.HeaderId == scriptRe.Data.Id);
        return R.OK(list.ToArray());
    }

    public static R<LiveScriptVoiceDetail[]> BuildScriptByTemplate(long templateId, string? inputText,
        UserLoginInfo user)
    {
        var scriptRe = CreateLiveScript(templateId, inputText, user);
        if (!scriptRe.Success)
        {
            return R.Faild<LiveScriptVoiceDetail[]>(scriptRe.Message);
        }

        var buildRe = BuildScript(scriptRe.Data.Id);
        if (!buildRe.Success)
        {
            return R.Faild<LiveScriptVoiceDetail[]>(buildRe.Message);
        }

        using var db = Db.Open();
        var list = db.Select<LiveScriptVoiceDetail>(d => d.HeaderId == scriptRe.Data.Id);
        return R.OK(list.ToArray());
    }

    private static R<LiveScript> CreateLiveScript(long templateId, string? inputText, UserLoginInfo user)
    {
        using var db = Db.Open();
        var template = db.SingleById<LiveScriptTemplate>(templateId);
        if (templateId == null)
        {
            return R.Faild<LiveScript>("模板未找到，请刷新后再操作");
        }

        var liveRoom = db.Single<LiveRoom>(a => a.UserId == user.UserId);
        if (liveRoom == null)
        {
            return R.Faild<LiveScript>("未设置房间信息和主播专业知识，请在AI主播页面进行设置");
        }

        var script = new LiveScript()
        {
            Id = Id.NewId(),
            LiveRoomId = liveRoom.Id,
            GuestMessage = inputText,
            TenantId = user.TenantId,
            UserId = user.UserId,
            LiveScriptTemplateId = template.Id,
        };
        db.Insert(script);
        return R.OK(script);
    }

    private static R<LiveScript> CreateLiveScript(long aiVerticalAnchorId, string? chatText, string? interactText,
        UserLoginInfo user)
    {
        using var db = Db.Open();
        var verticalAnchor = db.SingleById<AiVerticalAnchor>(aiVerticalAnchorId);
        if (verticalAnchor == null)
        {
            return R.Faild<LiveScript>("未找到行业主播");
        }

        if (chatText.Has() && !verticalAnchor.ChatTemplateIds.Has())
        {
            return R.Faild<LiveScript>("选择的行业主播未设置聊天回复模板");
        }
        else if (interactText.Has() && !verticalAnchor.InteractTemplateIds.Has())
        {
            return R.Faild<LiveScript>("选择的行业主播未设置互动回复模板");
        }
        else if (!verticalAnchor.ScriptTemplateIds.Has())
        {
            return R.Faild<LiveScript>("选择的行业主播未设置生成话术模板");
        }

        var usage = chatText.Has() ? UsageType.Chat : UsageType.LiveScript;

        var templateIds = chatText.Has() ? verticalAnchor.ChatTemplateIds : verticalAnchor.ScriptTemplateIds;
        if (interactText.Has())
        {
            usage = UsageType.InteractScriptTemplate;
            templateIds = verticalAnchor.InteractTemplateIds;
        }

        var templateRe = GetNextTemplate(user, aiVerticalAnchorId, usage, templateIds);
        if (!templateRe.Success)
        {
            return R.Faild<LiveScript>(templateRe.Message);
        }

        var liveRoom = db.Single<LiveRoom>(a => a.UserId == user.UserId);
        if (liveRoom == null)
        {
            return R.Faild<LiveScript>("未设置房间信息和主播专业知识，请在AI主播页面进行设置");
        }

        var script = new LiveScript()
        {
            Id = Id.NewId(),
            LiveRoomId = liveRoom.Id,
            GuestMessage = chatText,
            TenantId = user.TenantId,
            UserId = user.UserId,
            LiveScriptTemplateId = templateRe.Data.Id,
        };
        db.Insert(script);
        return R.OK(script);
    }

    private static R<LiveScriptTemplate> GetNextTemplate(UserLoginInfo user, long aiVerticalAnchorId, UsageType usage,
        string? templateIds)
    {
        var ids = templateIds?.Split(',') ?? [];
        using var db = Db.Open();
        var templates = db.Select<LiveScriptTemplate>(t => ids.Contains(t.Id.ToString()));
        if (!templates.Has())
        {
            return R.Faild<LiveScriptTemplate>("未设置话术模板或者模板无效");
        }

        var cacheKey = $"LastScriptTemplateIndex:{aiVerticalAnchorId}_{usage}:{user.UserId}";
        var indexDto = CacheHelper.Get<IndexDto>(user.TenantId, cacheKey);
        var lastIndex = indexDto?.Index ?? -1;
        var index = lastIndex + 1;
        if (index >= templates.Count)
        {
            index = 0;
        }

        CacheHelper.SetKey(user.TenantId, cacheKey, new IndexDto()
        {
            Index = index,
        });
        return R.OK(templates[index]);
    }


    public static R BuildScript(long id)
    {
        using var db = Db.Open();

        var script = db.SingleById<LiveScript>(id);
        if (script == null)
        {
            return R.Faild("未查找到话术生成任务数据");
        }

        var template = db.SingleById<LiveScriptTemplate>(script.LiveScriptTemplateId);
        var liveRoom = db.SingleById<LiveRoom>(script.LiveRoomId);
        if (template == null)
        {
            return R.Faild("未选择话术模板");
        }

        if (liveRoom == null)
        {
            return R.Faild("未选择直播间");
        }

        var model = db.Select<ModelAuthInfo>().FirstOrDefault();
        if (model == null)
        {
            return R.Faild("script或model不能为空");
        }

        if (template.Usage == UsageType.Chat && !script.GuestMessage.Has())
        {
            return R.Faild("请输入用户消息");
        }

        // 生成系统提示
        var systemText = LiveScriptTemplateSvc.BuildText(template.Usage, true, template.SystemTemplate ?? "",
            liveRoom.PersonaText ?? "",
            liveRoom.ProductText ?? "", script.GuestMessage ?? "");
        var userText = LiveScriptTemplateSvc.BuildText(template.Usage, false, template.UserTemplate ?? "",
            liveRoom.PersonaText ?? "",
            liveRoom.ProductText ?? "", script.GuestMessage ?? "");


        var promptRe = ModelApiHelper.TextPrompt(model, systemText, userText);
        if (!promptRe.Success)
        {
            return R.Faild(promptRe.Message);
        }

        script.AssistantText = promptRe.Data;
        string pattern = @"<answer>([\s\S]*?)</answer>";
        var match = Regex.Match(promptRe.Data, pattern, RegexOptions.IgnoreCase);

        if (!match.Success)
        {
            return R.Faild("解析返回内容出错:" + promptRe.Data);
        }

        string text = match.Groups[1].Value;
        var rows = text.SplitEx('\n');
        var details = rows.Select(r => new LiveScriptVoiceDetail()
        {
            Id = Id.NewId(),
            Text = r,
            HeaderId = script.Id,
        }).ToList();
        db.Delete<LiveScriptVoiceDetail>(d => d.HeaderId == script.Id);
        db.InsertAll(details);
        db.Update(script);
        return R.OK();
    }

    public static R<LiveScriptVoiceDetail[]> BuildLiveScript(UserLoginInfo user)
    {
        var templateRe = GetNextTemplate(user, UsageType.LiveScript);
        if (!templateRe.Success)
        {
            return R.Faild<LiveScriptVoiceDetail[]>(templateRe.Message);
        }

        using var db = Db.Open();
        var liveRoom = db.Single<LiveRoom>(a => a.UserId == user.UserId);
        if (liveRoom == null)
        {
            return R.Faild<LiveScriptVoiceDetail[]>("未设置直播间说明");
        }

        var script = new LiveScript
        {
            UserId = user.UserId,
            TenantId = user.TenantId,
            GuestMessage = "",
            LiveRoomId = liveRoom.Id,
            AssistantText = "",
            LiveScriptTemplateId = templateRe.Data.Id,
            Id = Id.NewId(),
        };
        db.Insert(script);
        var buildRe = BuildScript(script.Id);
        if (!buildRe.Success)
        {
            return R.Faild<LiveScriptVoiceDetail[]>(buildRe.Message);
        }

        var list = db.Select<LiveScriptVoiceDetail>(d => d.HeaderId == script.Id);
        return R.OK(list.ToArray());
    }

    private static R<LiveScriptTemplate> GetNextTemplate(UserLoginInfo user, UsageType usage)
    {
        using var db = Db.Open();
        //选择脚本，创建存储对象，
        var tempList = db.Select<LiveScriptTemplate>(t => t.Usage == usage);
        if (!tempList.Has())
        {
            return R.Faild<LiveScriptTemplate>("未找到生成话术的模板，请先配置话术模板");
        }

        var tmpIndex = 0;
        var cacheKey = $"LastScriptTemplateIndex:{user.UserId}";
        var indexDto = CacheHelper.Get<IndexDto>(user.TenantId, cacheKey);
        if (indexDto == null)
        {
            indexDto = new IndexDto()
            {
                Index = 0,
            };
        }
        else
        {
            tmpIndex = indexDto.Index + 1;
            if (tmpIndex >= tempList.Count)
            {
                tmpIndex = 0;
            }

            indexDto.Index = tmpIndex;
        }

        CacheHelper.SetKey(user.TenantId, cacheKey, indexDto);
        return R.OK(tempList[tmpIndex]);
    }

    public class IndexDto
    {
        public int Index { get; set; }
    }

    /*public static R<LiveScriptVoiceDetailVm> BuildVoiceById(UserLoginInfo user, long id)
    {
        using var db = Db.Open();
        var vm = db.SingleById<LiveScriptVoiceDetail>(id);
        if (vm == null) return R.Faild<LiveScriptVoiceDetailVm>("未找到数据");
        var header = db.SingleById<LiveScript>(vm.HeaderId);
        if (header == null) return R.Faild<LiveScriptVoiceDetailVm>("未找到话术数据");
        var anchor = db.SingleById<Anchor>(header.AnchorId);
        if (anchor == null) return R.Faild<LiveScriptVoiceDetailVm>("未找到主播信息");
        if (!anchor.TtsModelId.HasValue)
        {
            return R.Faild<LiveScriptVoiceDetailVm>("主播未设置模型语音");
        }

        var buildRe = TtsModelSvc.BuildVoice(new VoiceBuildRequestVm
        {
            Content = vm.Text ?? "", TtsModelId = anchor.TtsModelId ?? 0,
        });
        if (!buildRe.Success)
        {
            return R.Faild<LiveScriptVoiceDetailVm>(buildRe.Message);
        }

        vm.Voice = buildRe.Data.Voice;
        db.Save(vm);
        return R.OK(vm.Adapt<LiveScriptVoiceDetailVm>());
    }*/


    public static R<VoiceBuildResponseVm> BuildVoiceByVm(UserLoginInfo user, VoiceBuildRequestVm vm)
    {
        return TtsModelSvc.BuildVoice(vm);
    }
}