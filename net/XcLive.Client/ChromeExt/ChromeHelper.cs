using System.Diagnostics;
using System.Net;
using Frame.Data;
using Frame.Utils;
using PuppeteerSharp;

namespace Frame.ChromeExt;

public static class ChromeHelper
{
    private static IBrowser? _browser;
    private const int DebugPort = 9333;
    private const string ChromePath = @"tools\Chrome99\chrome.exe";
    private const string ProcessName = @"chrome";

    public static void KillChromeProcess()
    {
        var processes = Process.GetProcessesByName(ProcessName);

        // 找到需要关闭的进程
        foreach (var process in processes)
        {
            try
            {
                // 获取进程的路径
                string processPath = process.MainModule.FileName;
                var exePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ChromePath);
                var exeFile = new FileInfo(exePath);
                // 检查进程路径是否与目标路径匹配
                if (string.Equals(processPath, exeFile.FullName, StringComparison.OrdinalIgnoreCase))
                {
                    // 如果路径匹配，则终止该进程
                    Console.WriteLine($"进程 {ProcessName} 已经在运行，路径匹配，正在终止...");
                    process.Kill();
                    process.WaitForExit(); // 等待进程完全退出
                    Console.WriteLine($"进程 {ProcessName} 已终止.");
                    break; // 找到并关闭目标进程后退出循环
                }
            }
            catch (Exception ex)
            {
                // 处理无法获取进程路径的异常（如权限不足）
                Console.WriteLine($"无法获取进程 {process.Id} 的路径: {ex.Message}");
            }
        }
    }

    public static R StartChrome(string url, string dataPath = "chromeData")
    {
        StartProgress(url, dataPath);
        var connRe = ConnectWithRetriesAsync(DebugPort, 3, TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(10));
        if (!connRe.success)
        {
            return connRe;
        }

        _browser = connRe.data;
        _page = _browser.NewPageAsync().Result;
        _page.GoToAsync(url, WaitUntilNavigation.DOMContentLoaded).Wait();
        return R.OK();
    }

    public static void NewPage(string url)
    {
        _page = _browser?.NewPageAsync().Result;
        _page?.GoToAsync(url, WaitUntilNavigation.DOMContentLoaded).Wait();
    }

    private static void StartProgress(string url, string dataPath)
    {
        var exePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ChromePath);
        var exeFile = new FileInfo(exePath);
        if (!exeFile.Exists)
        {
            throw new Exception($"Chrome不存在{exePath}");
        }

        var dataDir = Path.Combine(exeFile.Directory?.Parent?.ToString() ?? "C:\\", dataPath);
        var arg = $"--remote-debugging-port={DebugPort}  --user-data-dir=\"{dataDir}\" " +
                  $" --no-default-browser-check";
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = ChromePath,
                Arguments = arg,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = false
            }
        };
        process.Start();
        Console.WriteLine("Chrome 已启动...");
    }

    static R<IBrowser> ConnectWithRetriesAsync(int debugPort, int maxRetries, TimeSpan retryDelay, TimeSpan timeout)
    {
        var options = new ConnectOptions
        {
            BrowserURL = $"http://localhost:{debugPort}",
            DefaultViewport = null, //必须要的，否则就只有800X600，页面会因此变形
        };

        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                Log4.Log.Info($"尝试第 {attempt} 次连接...");
                // 开始连接任务

                var connectTask = Puppeteer.ConnectAsync(options);
                // 使用 WhenAny 来实现超时判断
                var completedTask = Task.WhenAny(connectTask, Task.Delay(timeout)).Result;
                if (completedTask == connectTask)
                {
                    // 连接成功且在超时时间内完成
                    return R.OK(connectTask.Result);
                }
            }
            catch (Exception ex)
            {
                Log4.Log.Warn($"第 {attempt} 次连接失败：{ex.Message}");
                if (attempt == maxRetries)
                {
                    return R.Faild<IBrowser>($"第 {attempt} 次连接失败：{ex.Message}");
                }

                Task.Delay(retryDelay).Wait();
            }
        }

        return R.Faild<IBrowser>("达到最大重试次数，仍无法连接。");
    }

    static IPage? _page = null;

    public static R<Cookie[]> GetCookie()
    {
        if (_page == null)
        {
            return R.Faild<Cookie[]>("未启动Chrome或页面没打开，请重试");
        }

        var cookies = _page.GetCookiesAsync().Result;
        var list = cookies.Where(c => !string.IsNullOrEmpty(c.Name))
            .Select(c => new Cookie(c.Name, c.Value, c.Path, c.Domain)).ToArray();
        return R.OK(list);
    }

    public static R<string> GetHtml()
    {
        if (_page == null)
        {
            return R.Faild<string>("未启动Chrome或页面没打开，请重试");
        }

        var cookieRe = GetCookie();
        if (!cookieRe.success) return R.Faild<string>("Cookies获取失败");
        var c = new DyWebClient(cookieRe.data);
        var htmlRe = c.Get("https://live.douyin.com/211421553240");
        //直接从DOM拿到的HTML不知道为什么有问题，好像是转换过了的
        return htmlRe;
    }
}