using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using CefSharp;
using CefSharp.Handler;
using CefSharp.WinForms;
using Frame.Utils;

namespace Frame;

public partial class MainFrame : BorderLessForm
{
    private readonly ChromiumWebBrowser _webbrowser1;


    public MainFrame()
    {
        IniCef();
        InitializeComponent();
        SizeChanged += WindowSizeChanged;

        _webbrowser1 = new ChromiumWebBrowser(AppHelper.ServerHost)
        {
            Dock = DockStyle.Fill,
            RequestHandler = new StrictCertificateRequestHandler(),
        };
        _webbrowser1.RenderProcessMessageHandler = new CustomHandler(); // 可自定义处理


        _webbrowser1.JavascriptObjectRepository.Settings.LegacyBindingEnabled = true;
        _webbrowser1.JavascriptObjectRepository.Register("dotnetObject", new DotNetObject(_webbrowser1, this),
            BindingOptions.DefaultBinder);
        _webbrowser1.FrameLoadEnd += FrameLoadEnd;
        // 监听来自 JavaScript 的消息
        _webbrowser1.JavascriptMessageReceived += Browser_JavascriptMessageReceived;
        Controls.Add(_webbrowser1);
    }

    private void IniCef()
    {
        if (Cef.IsInitialized == true)
        {
            MessageBox.Show("CefSharp初始化有问题，在本行运行前不应该有其他Cef初始化");
            Application.Exit();
            return;
        }

        var settings = new CefSettings();
        settings.CefCommandLineArgs.Add("no-proxy-server", "1");
        Cef.Initialize(settings);
    }

    private void Ini()
    {
        Icon = new Icon("res/star.ico");
        Task.Factory.StartNew(() =>
        {
            try
            {
                var title = AppHelper.GetFormTitle();
                Invoke(() => { Text = title; });
            }
            catch (Exception e)
            {
                Log4.Log.Error(e);
            }
        });
    }

    private void FrameLoadEnd(object? sender, FrameLoadEndEventArgs e)
    {
        if (!e.Frame.IsMain)
            return;
        string script = @"
                    window.addEventListener('keydown', function(event) {
                        if (event.ctrlKey && event.shiftKey && event.key === 'F12') {
                            CefSharp.PostMessage('showDevTool')
                        }
                        if (event.key === 'F5') {
                            CefSharp.PostMessage('refreshPage')
                        }
                    });
                    console.log('注入快捷键事件拦截');
                ";
        try
        {
            _webbrowser1.ExecuteScriptAsync(script);
        }
        catch (Exception exception)
        {
            Log4.Log.Error(exception);
            MessageBox.Show(exception.Message);
        }
    }

    private void WindowSizeChanged(object? sender, EventArgs e)
    {
        var size = 1;
        if (WindowState == FormWindowState.Maximized)
        {
            size = 3;
        }
        else if (WindowState == FormWindowState.Normal)
        {
            size = 2;
        }

        try
        {
            if (!_webbrowser1.IsBrowserInitialized) return;
            _webbrowser1.ExecuteScriptAsync($"if(window['windowSizeChanged'])windowSizeChanged({size})");
        }
        catch (Exception exception)
        {
            Log4.Log.Error(exception);
            MessageBox.Show(exception.Message);
        }
    }

    private void MainFrame_Load(object sender, EventArgs e)
    {
        Ini();
        //var dot = new DotNetObject(_webbrowser1, this);
        //dot.updateNewVersion("tmp/0.0.2504.0.zip");
    }

    [DllImport("user32.dll")]
    private static extern int SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

    [DllImport("user32.dll")]
    private static extern bool ReleaseCapture();

    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    private void Browser_JavascriptMessageReceived(object sender, JavascriptMessageReceivedEventArgs e)
    {
        Invoke(() =>
        {
            try
            {
                if (e.Message.ToString() == "dragWindow")
                {
                    // 获取当前窗口句柄
                    IntPtr hwnd = this.Handle;

                    // 获取窗口的位置和尺寸
                    GetWindowRect(hwnd, out RECT rect);

                    // 检查是否接近屏幕顶部
                    if (rect.Top <= 10) // 距离顶部10px以内
                    {
                        // 最大化窗口
                        this.WindowState = FormWindowState.Maximized;
                    }
                    else
                    {
                        // 否则，恢复到正常窗口状态
                        this.WindowState = FormWindowState.Normal;
                    }

                    ReleaseCapture();
                    SendMessage(Handle, 0xA1, 0x02, 0);
                }
                else if (e.Message.ToString() == "showDevTool")
                {
                    _webbrowser1.ShowDevTools();
                }
                else if (e.Message.ToString() == "refreshPage")
                {
                    _webbrowser1.Reload(true);
                }
                else
                {
                    throw new Exception("未知消息:" + e.Message);
                }
            }
            catch (Exception exception)
            {
                Log4.Log.Error(exception);
                MessageBox.Show(exception.Message);
            }
        });
    }
}

public class CustomHandler : IRenderProcessMessageHandler
{
    public void OnContextCreated(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame)
    {
    }

    public void OnContextReleased(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame)
    {
    }

    public void OnFocusedNodeChanged(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IDomNode node)
    {
    }

    public void OnUncaughtException(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame,
        JavascriptException exception)
    {
        Log4.Log.Error(exception);
        MessageBox.Show(exception.Message);
    }
}

public class StrictCertificateRequestHandler : RequestHandler
{
    protected override IResourceRequestHandler GetResourceRequestHandler(IWebBrowser chromiumWebBrowser,
        IBrowser browser, IFrame frame, IRequest request, bool isNavigation, bool isDownload, string requestInitiator,
        ref bool disableDefaultHandling)
    {
        return new CertificatePinningResourceRequestHandler();
    }
}

public class CertificatePinningResourceRequestHandler : ResourceRequestHandler
{
    protected override CefReturnValue OnBeforeResourceLoad(IWebBrowser chromiumWebBrowser, IBrowser browser,
        IFrame frame, IRequest request,
        IRequestCallback callback)
    {
        if (!CertValidate.CheckCert(request.Url))
        {
            return CefReturnValue.Cancel;
        }

        return CefReturnValue.Continue; // 表示请求继续加载
    }
}