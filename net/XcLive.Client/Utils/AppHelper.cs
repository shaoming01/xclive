using Frame.Data;
using Newtonsoft.Json;

namespace Frame.Utils;

public class AppHelper
{
#if DEBUG
    public static readonly string ServerHost = "http://localhost:7777/";
    public static readonly string TenantId = "1";
    public static readonly bool IsDebug = true;

#else
    public static readonly string ServerHost = "https://live.erp12345.com/";
    public static readonly string TenantId = "1";
    public static readonly bool IsDebug = false;
#endif
    private static AppInfo? _appInfo;

    public static AppInfo GetAppInfo()
    {
        if (_appInfo != null)
        {
            return _appInfo;
        }

        var emptyRe = new AppInfo
        {
            ProductName = "直播助手",
            Version = new Version(1, 1),
            ExeVersion = new Version(1, 1),
            TenantId = TenantId,
        };
        try
        {
            var url = $"{ServerHost.TrimEnd('/')}/api/product/info?tenantId={TenantId}";
            var client = new HttpClient();
            var resp = client.GetAsync(url).Result;
            if (!resp.IsSuccessStatusCode)
            {
                return emptyRe;
            }

            var json = resp.Content.ReadAsStringAsync().Result;
            var re = JsonConvert.DeserializeObject<R<AppInfo>>(json);
            _appInfo = re?.data ?? emptyRe;
            var appVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version ?? new Version();

            _appInfo.Version = new Version(_appInfo.Version.Major, _appInfo.Version.Minor, appVersion.Build);
            _appInfo.ExeVersion = appVersion;
            _appInfo.TenantId = TenantId;
            return _appInfo;
        }
        catch (Exception e)
        {
            Log4.Log.Error(e);
            return emptyRe;
        }
    }

    public class AppInfo
    {
        public Version Version { get; set; }
        public Version ExeVersion { get; set; }
        public string ProductName { get; set; }
        public string TenantId { get; set; }
    }

    public class CefAppInfo
    {
        public string productName { get; set; }
        public string version { get; set; }
        public string exeVersion { get; set; }
        public string tenantId { get; set; }
    }

    public static string GetFormTitle()
    {
        var info = GetAppInfo();
        return $"{info.ProductName}-{info.Version}";
    }
}