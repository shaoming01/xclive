using System.Net;
using Frame.Data;
using Frame.Ext;

namespace Frame.ChromeExt;

internal static class LoginUtil
{
    public static void StartLogin()
    {
        ChromeHelper.KillChromeProcess();
        ChromeHelper.StartChrome("https://www.douyin.com");
    }

    public static R<Cookie[]> GetLoginCookie()
    {
        var rCookie = ChromeHelper.GetCookie();
        if (!rCookie.success) return R.Faild<Cookie[]>(rCookie.message);
        var sessionId =
            rCookie.data.FirstOrDefault(x =>
                x.Name == "passport_fe_beating_status"); //passport_assist_user，passport_fe_beating_status
        if (sessionId == null || sessionId.Value.IsNullOrEmpty()) return R.Faild<Cookie[]>("");
        return R.OK(rCookie.data);
    }
}