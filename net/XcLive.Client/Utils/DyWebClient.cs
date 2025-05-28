using System.Net;
using System.Text;
using Frame.Data;

namespace Frame.Utils;

public class DyWebClient
{
    private readonly Cookie[] _cookies;

    public DyWebClient(Cookie[] cookies)
    {
        _cookies = cookies;
    }

    public R<string> Get(string url)
    {
        var client = CreateClient();
        client.DefaultRequestHeaders.Add("Cookie", ToCookieString(_cookies));
        var response = client.GetAsync(url).Result;
        if (!response.IsSuccessStatusCode) return R.Faild<string>("请求出错：" + response.StatusCode);
        var resp = response.Content.ReadAsStringAsync().Result;
        return R.OK(resp);
    }

    public R<string> Post(string url, string json)
    {
        var client = CreateClient();
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var resp = client.PostAsync(url, content).Result;
        if (!resp.IsSuccessStatusCode) return R.Faild<string>("请求出错：" + resp.StatusCode);
        var html = resp.Content.ReadAsStringAsync().Result;
        return R.OK(html);
    }

    private HttpClient CreateClient()
    {
        var handler = new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.Brotli
        };
        var client = new HttpClient(handler);
        client.DefaultRequestHeaders.Add("Cookie", ToCookieString(_cookies));
        client.DefaultRequestHeaders.Add("User-Agent",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
        client.DefaultRequestHeaders.Add("Accept", "application/json, text/plain, */*");
        client.DefaultRequestHeaders.Add("Referer",
            "https://leads.cluerich.com/livefe/edouyin/live/current?sec_user_id=MS4wLjABAAAAGq5_au7Ulo5ICcqeS6dpVvpl1yal87fM9mEnUULCShRTuQH8ORAPBHbW8Q4CoDmm&leads_tag=2074151699369360&game_pad_per=0");
        return client;
    }

    private IEnumerable<string?> ToCookieString(Cookie[] cookies)
    {
        return cookies.Select(x => $"{x.Name}={x.Value}");
    }
}

public static class UriExt
{
    public static void AddQueryParam(this UriBuilder uri, string key, string value)
    {
        if (uri == null) return;
        uri.AddQueryParam(new[]
        {
            new KeyValuePair<string, string>(key, value)
        });
    }

    public static void AddQueryParam(this UriBuilder uri, IEnumerable<KeyValuePair<string, string>> vals)
    {
        if (uri == null) return;
        var dic = uri.GetQueryParams();
        foreach (var item in vals)
        {
            dic.Add(item);
        }

        uri.Query = dic.ToQueryParam();
    }

    public static string ToQueryParam(this IDictionary<string, string> dic, bool encode = true)
    {
        if (dic == null || dic.Count == 0)
            return string.Empty;

        var queryParams = dic.Select(kvp => kvp.Key + "=" + (encode ? WebUtility.UrlEncode(kvp.Value) : kvp.Value));
        return string.Join("&", queryParams);
    }

    public static IDictionary<string, string> GetQueryParams(this UriBuilder uri)
    {
        if (uri == null)
        {
            throw new ArgumentNullException(nameof(uri));
        }

        var queryParams = uri.Query.TrimStart('?');

        if (string.IsNullOrEmpty(queryParams))
        {
            return new Dictionary<string, string>();
        }

        return queryParams
            .Split('&')
            .Select(param => param.Split('='))
            .ToDictionary(
                keyValuePair => Uri.UnescapeDataString(keyValuePair[0]),
                keyValuePair => Uri.UnescapeDataString(keyValuePair.Length > 1 ? keyValuePair[1] : ""));
    }
}