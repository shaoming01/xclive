using System.Net;
using System.Text;
using Frame.Data;
using Frame.Ext;

namespace Frame.Utils;

public class HttpHelper
{
    public static R<string> GetHtml(string url, Dictionary<string, string>? headers, CookieDto[]? cookies,
        string? postJson = null)
    {
        var client = CreateClient(headers, cookies);
        HttpResponseMessage response;
        if (!postJson.IsNullOrEmpty())
        {
            HttpContent content;
            if (postJson.IsJson())
            {
                content = new StringContent(postJson!, Encoding.UTF8, "application/json");
            }
            else
            {
                content = new StringContent(postJson!, Encoding.UTF8, "application/x-www-form-urlencoded");
            }

            response = client.PostAsync(url, content).Result;
        }
        else
        {
            response = client.GetAsync(url).Result;
        }

        if (!response.IsSuccessStatusCode) return R.Faild<string>("请求出错：" + response.StatusCode);
        var html = response.Content.ReadAsStringAsync().Result;
        return R.OK(html);
    }

    private static HttpClient CreateClient(Dictionary<string, string>? headers, CookieDto[]? cookies)
    {
        var handler = new HttpClientHandler
        {
            AutomaticDecompression =
                DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.Brotli,
            UseProxy = AppHelper.IsDebug,
            Proxy = AppHelper.IsDebug ? WebRequest.DefaultWebProxy : null,
            //验证HTTPS证书，防止抓包
            ServerCertificateCustomValidationCallback = (request, cert, chain, errors) =>
                CertValidate.CheckCert(request.RequestUri?.ToString() ?? "", cert)
        };
        var client = new HttpClient(handler);
        client.DefaultRequestHeaders.Add("Cookie", ToCookieString(cookies));
        client.DefaultRequestHeaders.Add("User-Agent",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/118.0.0.0 Safari/537.36 Edg/118.0.2088.122");
        client.DefaultRequestHeaders.Add("Accept", "application/json, text/plain, */*");
        client.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9,en;q=0.8,en-GB;q=0.7,en-US;q=0.6");
        //client.DefaultRequestHeaders.Add("Referer",
            //"https://buyin.jinritemai.com/dashboard/live/control?btm_ppre=a0.b0.c0.d0&btm_pre=a10091.b61626.c68160.d839440_i606617&btm_show_id=3b9a3828-bd73-4729-bb79-2e2c5ef676d3&pre_universal_page_params_id=&universal_page_params_id=fe8d426c-5855-4fcd-a01e-75de0d4597f2");

        // 设置其他请求头
        foreach (var header in headers ?? new Dictionary<string, string>())
        {
            if (client.DefaultRequestHeaders.Contains(header.Key))
            {
                client.DefaultRequestHeaders.Remove(header.Key);
            }

            client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
        }

        return client;
    }


    private static IEnumerable<string?> ToCookieString(CookieDto[]? cookies)
    {
        if (!cookies.Has())
        {
            return [];
        }

        return cookies.Select(x => $"{x.name}={x.value}");
    }
}