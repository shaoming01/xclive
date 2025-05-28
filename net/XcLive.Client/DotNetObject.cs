using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Barrage.GrabServices;
using CefSharp;
using CefSharp.WinForms;
using Frame.ChromeExt;
using Frame.Data;
using Frame.Ext;
using Frame.Utils;
using NAudio.CoreAudioApi;
using Newtonsoft.Json;

namespace Frame;

/// <summary>
/// 此对象会注入到浏览器中，供JS调用
/// </summary>
/// <param name="browser"></param>
/// <param name="mainForm"></param>
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class DotNetObject
{
    private readonly ChromiumWebBrowser _browser;
    private readonly SoundCardManager soundCardManager = new();
    private readonly MainFrame mainForm;

    private DouyinBarrageGrabService? _barrageSvc;
    private readonly string VirtualSoundCardInstall = "tools/虚拟麦克风.exe";
    private readonly AudioPlayUtil audioPlayUtil = new();

    public DotNetObject(ChromiumWebBrowser browser, MainFrame mainForm)
    {
        _browser = browser;
        this.mainForm = mainForm;
        audioPlayUtil.OnMessage += (sender, message) => { sendPlayMessageToBrowser(message); };
    }


    public void minWindow()
    {
        mainForm.Invoke(() => { mainForm.WindowState = FormWindowState.Minimized; });
    }

    public void switchWindow()
    {
        mainForm.Invoke(() =>
        {
            if (mainForm.WindowState == FormWindowState.Maximized)
            {
                mainForm.WindowState = FormWindowState.Normal;
            }
            else
                mainForm.WindowState = FormWindowState.Maximized;
        });
    }

    public void closeWindow()
    {
        mainForm.Invoke(mainForm.Close);
    }

    public void hideBorder(bool noBorder)
    {
        mainForm.Invoke(() => { mainForm.SetNoBorder(noBorder); });
    }

    public R downloadFile(string url, string fileName)
    {
        try
        {
            var re = WgetHelper.DownloadFileAsync(url, fileName).Result;
            if (re.Status == WgetHelper.DownloadStatus.Completed)
            {
                return R.OK();
            }

            return R.Faild(re.Message);
        }
        catch (Exception e)
        {
            Log4.Log.Error(e);
            return R.Faild(e.Message);
        }
    }

    public R<bool> fileExists(string fileName)
    {
        try
        {
            return R.OK(File.Exists(fileName));
        }
        catch (Exception e)
        {
            Log4.Log.Error(e);
            return R.Faild<bool>(e.Message);
        }
    }

    public R updateNewVersion(string zipFilePath)
    {
        try
        {
            File.Copy("update.bat", "update_tmp.bat", true);
            var process = new Process();
            process.StartInfo.FileName = "update_tmp.bat";
            process.StartInfo.Arguments = zipFilePath;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true; // 隐藏窗口
            process.Start();
        }
        catch (Exception e)
        {
            Log4.Log.Error(e);
            return R.Faild(e.Message);
        }

        Application.Exit();
        return R.OK();
    }

    public void playVoice(VoicePlayData data)
    {
        audioPlayUtil.PlayVoice(data);
    }

    public R<bool> isIdInQueue(string id)
    {
        return R.OK(audioPlayUtil.isIdInQueue(id));
    }

    public R<string> getDeviceId()
    {
        return R.OK(DeviceIdUtil.GetMacAddress());
    }

    public void pausePlay()
    {
        audioPlayUtil.Pause();
    }

    public void resumePlay()
    {
        audioPlayUtil.ResumePlay();
    }

    public void stopPlay()
    {
        audioPlayUtil.Stop();
    }

    public R<VoicePlayStatus> getPlayStatus()
    {
        return R.OK(audioPlayUtil.GetStatus());
    }

    public R<AppHelper.CefAppInfo> getAppInfo()
    {
        var app = AppHelper.GetAppInfo();
        return R.OK(new AppHelper.CefAppInfo
        {
            productName = app.ProductName,
            version = app.Version.ToString(),
            exeVersion = app.ExeVersion.ToString(),
            tenantId = app.TenantId,
        });
    }

    public R startWatchBarrage(string roomNo, string liveObserverCookie, string loginToken)
    {
        try
        {
            _barrageSvc?.Dispose();

            _barrageSvc = new DouyinBarrageGrabService((string roomId, string uniqueId) =>
                SignHelper.GetWssUrl(roomId, uniqueId, loginToken));
            _barrageSvc.OnOpen += (o, args) => { sendWsMessageToBrowser(WsMessageType.Open, args); };
            _barrageSvc.OnError += (o, args) => { sendWsMessageToBrowser(WsMessageType.Error, args.Message); };
            _barrageSvc.OnClose += (o, args) => { sendWsMessageToBrowser(WsMessageType.Close, args); };
            _barrageSvc.OnMessage += (sender, message) => { sendWsMessageToBrowser(WsMessageType.Message, message); };
            _barrageSvc.Start(roomNo);
        }
        catch (Exception e)
        {
            Log4.Log.Error(e);
            return R.Faild(e.Message);
        }

        return R.OK();
    }

    public R stopWatchBarrage()
    {
        try
        {
            _barrageSvc?.Stop();
        }
        catch (Exception e)
        {
            Log4.Log.Error(e);
            return R.Faild(e.Message);
        }

        return R.OK();
    }

    public R startBrowser(string url, string dataPath)
    {
        try
        {
            var r = ChromeHelper.StartChrome(url, dataPath);
            return r;
        }
        catch (Exception e)
        {
            Log4.Log.Error(e);
            return R.Faild(e.Message);
        }

        return R.OK();
    }

    public R closeBrowser()
    {
        try
        {
            ChromeHelper.KillChromeProcess();
        }
        catch (Exception e)
        {
            Log4.Log.Error(e);
            return R.Faild(e.Message);
        }

        return R.OK();
    }

    public R<CookieDto[]> getBrowserCookie()
    {
        try
        {
            var c = ChromeHelper.GetCookie();
            if (!c.success)
            {
                return R.Faild<CookieDto[]>(c.message);
            }

            if (!c.data.Has())
            {
                return R.Faild<CookieDto[]>("未找到登录信息，请检查浏览器是否正确打开，并登录账号");
            }

            var cookies = c.data.Where(c => !c.Value.IsNullOrEmpty() && !c.Name.IsNullOrEmpty()).Select(c =>
                new CookieDto
                {
                    name = c.Name,
                    domain = c.Domain, value = c.Value,
                }).ToArray();
            return R.OK(cookies);
        }
        catch (Exception e)
        {
            Log4.Log.Error(e);
            return R.Faild<CookieDto[]>(e.Message);
        }
    }

    public R<string> getHtml(string url, object? headers, CookieDto[]? cookies, string? postJson)
    {
        try
        {
            Dictionary<string, string>? dicHeaders = new Dictionary<string, string>();
            if (headers != null)
            {
                dicHeaders =
                    JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonConvert.SerializeObject(headers));
            }

            return HttpHelper.GetHtml(url, dicHeaders, cookies, postJson);
        }
        catch (Exception e)
        {
            Log4.Log.Error(e);
            return R.Faild<string>(e.Message);
        }
    }

    public R<ValueDisplay[]> getVoiceDeviceList()
    {
        try
        {
            var list = soundCardManager.GetAllOutputDevices();

            var reList = list.Select(l => new ValueDisplay
            {
                label = l.Name,
                value = l.Id,
            }).ToArray();
            return R.OK(reList);
        }
        catch (Exception e)
        {
            Log4.Log.Error(e);
            return R.Faild<ValueDisplay[]>(e.Message);
        }
    }

    public R<int> getSoundCardVolume(string id)
    {
        var list = soundCardManager.GetAllOutputDevices();
        var card = list.FirstOrDefault(l => l.Id == id || (id == "-1" && l.IsDefault));
        if (card == null)
        {
            return R.OK(0);
        }

        return R.OK(card.Volume);
    }

    public R setSoundCardVolume(string id, int volume)
    {
        var list = soundCardManager.GetAllOutputDevices();
        var card = list.FirstOrDefault(l => l.Id == id || (id == "-1" && l.IsDefault));
        if (card == null)
        {
            return R.Faild("未找到该声卡");
        }

        soundCardManager.SetVolume(card.Id, volume);
        return R.OK();
    }

    public R setupVirtualSoundCard()
    {
        try
        {
            // 获取 exe 的完整路径
            string exePath = Path.Combine(AppContext.BaseDirectory, VirtualSoundCardInstall);
// 启动进程
            Process.Start(new ProcessStartInfo
            {
                FileName = exePath,
                UseShellExecute = true // WinForms 下建议加这个
            });
            return R.OK();
        }
        catch (Exception e)
        {
            Log4.Log.Error(e);
            return R.Faild(e.Message);
        }
    }

    public R sendWsMessageToBrowser(WsMessageType msgType, object? param)
    {
        try
        {
            var settings = new JsonSerializerSettings
            {
                Converters = new List<JsonConverter>
                    { new LongToStringConverter(), new SimpleNullableLongToStringConverter() }
            };
            var parJson = param == null ? "undefined" : JsonConvert.SerializeObject(param, settings);
            var script = $"wssOnMessage({(int)msgType}, {parJson})";
            _browser.EvaluateScriptAsync(script).Wait();
        }
        catch (Exception e)
        {
            Log4.Log.Error(e);
            return R.Faild(e.Message);
        }

        return R.OK();
    }

    private R sendPlayMessageToBrowser(VoicePlayMessage message)
    {
        try
        {
            var parJson = JsonConvert.SerializeObject(message);
            var script = $"if(window['playOnMessage'])playOnMessage({parJson})";
            _browser.EvaluateScriptAsync(script).Wait();
        }
        catch (Exception e)
        {
            Log4.Log.Error(e);
            return R.Faild(e.Message);
        }

        return R.OK();
    }

    public enum WsMessageType
    {
        Open = 1,
        Error = 2,
        Message = 3,
        Close = 4,
    }
}

public class CookieDto
{
    public string name { get; set; }
    public string value { get; set; }
    public string domain { get; set; }
}

public class ValueDisplay
{
    public string value { get; set; }
    public string label { get; set; }
}

public static class SignHelper
{
    public static string GetWssUrl(string roomId, string uniqueId, string loginToken)
    {
        var url = $"{AppHelper.ServerHost}api/sign/GetWssUrl?roomId={roomId}&uniqueId={uniqueId}";
        var re = HttpHelper.GetHtml(url, null, new[]
        {
            new CookieDto()
            {
                name = "token", value = loginToken
            }
        }, null);
        if (!re.success)
        {
            throw new Exception(re.message);
        }

        var r = JsonConvert.DeserializeObject<R<string>>(re.data);
        if (r == null || !r.success)
        {
            throw new Exception(r?.message);
        }

        return r.data;
    }
}