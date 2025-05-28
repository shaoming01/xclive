using System.Text;
using Newtonsoft.Json;
using SchemaBuilder.Svc.Core;
using SchemaBuilder.Svc.Core.Ext;
using SchemaBuilder.Svc.Models.ViewModel;
using ServiceStack;

namespace SchemaBuilder.Svc.Svc;

public static class SignSvc
{
    private const string SignHost = "https://live.erp12345.com";

    public static R<string> GetByReqUrl(ByUrlGetReqVm vm)
    {
        var re = Request<string>("signApi/sign/GetByReqUrl", null, vm.ToJson());
        return re;
    }

    public static R<string> GetWssUrl(string roomId, string uniqueId)
    {
        var par = new Dictionary<string, string>()
        {
            {
                "roomId", roomId
            },
            {
                "uniqueId", uniqueId
            }
        };
        var re = Request<string>("signApi/sign/GetWssUrl", par, null);
        return re;
    }

    public static R<T> Request<T>(string path, Dictionary<string, string>? queryParams, string? postData)
    {
        var url = BuildUrl(SignHost, path, queryParams);
        var client = new HttpClient();
        HttpResponseMessage response;
        if (!StringExtent.IsNullOrEmpty(postData))
        {
            HttpContent content;
            if (postData.IsJson())
            {
                content = new StringContent(postData!, Encoding.UTF8, "application/json");
            }
            else
            {
                content = new StringContent(postData!, Encoding.UTF8, "application/x-www-form-urlencoded");
            }

            response = client.PostAsync(url, content).Result;
        }
        else
        {
            response = client.GetAsync(url).Result;
        }

        if (!response.IsSuccessStatusCode) return R.Faild<T>("请求出错：" + response.StatusCode);
        var html = response.Content.ReadAsStringAsync().Result;
        var obj = JsonConvert.DeserializeObject<R<T>>(html);
        return obj;
    }

    public static string BuildUrl(string baseUrl, string path, Dictionary<string, string>? queryParams)
    {
        // 1. 合并路径
        var baseUri = new Uri(baseUrl.TrimEnd('/') + "/");
        var fullUri = new Uri(baseUri, path.TrimStart('/'));

        // 2. 添加参数
        var query = System.Web.HttpUtility.ParseQueryString(fullUri.Query);
        if (queryParams != null)
        {
            foreach (var kv in queryParams)
            {
                query[kv.Key] = kv.Value;
            }
        }


        // 3. 重建 URL
        var uriBuilder = new UriBuilder(fullUri)
        {
            Query = query.ToString() // 会自动编码
        };

        return uriBuilder.ToString();
    }
}