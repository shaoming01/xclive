using System.Net;
using System.Text;
using Newtonsoft.Json;
using SchemaBuilder.Svc.Core;
using SchemaBuilder.Svc.Helpers;
using SchemaBuilder.Svc.Models.Table;

namespace SchemaBuilder.Svc.Svc;

public class TextChatResp
{
    public TextChatChoice[] choices { get; set; }
    public ChatUsage usage { get; set; }
}

public class ImageChatResp
{
    public ImageChatChoice[] choices { get; set; }
    public ChatUsage usage { get; set; }
}

public class ChatUsage
{
    public int completion_tokens { get; set; }
    public int prompt_tokens { get; set; }
    public int total_tokens { get; set; }
}

public class TextChatChoice
{
    public TextMessage? message { get; set; }
}

public class ImageChatChoice
{
    public ImageMessage message { get; set; }
}

public class TextMessage
{
    public string role { get; set; }
    public string content { get; set; }
}

public class ImageMessage
{
    public string role { get; set; }
    public ImageContent[] content { get; set; }
}

public class ImageContent
{
    public string type { get; set; }
    public string text { get; set; }
    public ImageUrl image_url { get; set; }
}

public class ImageUrl
{
    public static ImageUrl FromPath(string imagePath)
    {
        // 读取图片文件为字节数组
        byte[] imgArr = File.ReadAllBytes(imagePath);

        // 将字节数组转换为Base64字符串
        string base64 = Convert.ToBase64String(imgArr);

        // 获取图片扩展名
        string extension = Path.GetExtension(imagePath).ToLowerInvariant().TrimStart('.');

        // 构建Data URL格式的字符串
        var base64Url = $"data:image/{extension};base64,{base64}";
        return new ImageUrl()
        {
            url = base64Url
        };
    }

    public string url { get; set; }
}

public class TextChatReq
{
    [JsonIgnore] public string? apiPath { get; set; } = "/chat/completions";
    public string? model { get; set; }
    public TextMessage[] messages { get; set; }
}

public class ImageChatReq
{
    [JsonIgnore] public string apiPath { get; set; } = "/chat/completions";
    public string model { get; set; }
    public ImageMessage[] messages { get; set; }
}

public class DouBaoClient
{
    private readonly string _apiBaseUrl;
    private readonly string? _apiKey;
    private readonly HttpClient _httpClient;

    public DouBaoClient(string apiBaseUrl, string? apiKey)
    {
        _apiBaseUrl = apiBaseUrl;
        _apiKey = apiKey;
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + _apiKey);
    }

    public R<TextChatResp> SendTextChat(TextChatReq req)
    {
        if (!_apiKey.Has())
        {
            return R.Faild<TextChatResp>("apiKey不能为空");
        }

        var url = $"{_apiBaseUrl.TrimEnd('/')}/{req.apiPath.TrimStart('/')}";
        var json = JsonConvert.SerializeObject(req);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = _httpClient.PostAsync(url, content).Result;
        var respContent = response.Content.ReadAsStringAsync().Result;
        if (response.StatusCode != HttpStatusCode.OK)
        {
            return R.Faild<TextChatResp>("请求出错:" + response.StatusCode + " 返回：" + respContent);
        }

        var obj = JsonConvert.DeserializeObject<TextChatResp>(respContent);
        return R.OK(obj);
    }

    public R<TextChatResp> SendImageChat(ImageChatReq req)
    {
        if (!_apiKey.Has())
        {
            return R.Faild<TextChatResp>("apiKey不能为空");
        }

        var url = $"{_apiBaseUrl.TrimEnd('/')}/{req.apiPath.TrimStart('/')}";
        var json = JsonConvert.SerializeObject(req);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = _httpClient.PostAsync(url, content).Result;
        var respContent = response.Content.ReadAsStringAsync().Result;
        if (response.StatusCode != HttpStatusCode.OK)
        {
            return R.Faild<TextChatResp>("请求出错:" + response.StatusCode + " 返回：" + respContent);
        }

        var obj = JsonConvert.DeserializeObject<TextChatResp>(respContent);
        return R.OK(obj);
    }
}

public static class ModelApiHelper
{
    public static R<string> TextPrompt(ModelAuthInfo model, string? sysContent, string? userContent)
    {
        var client = new DouBaoClient("https://ark.cn-beijing.volces.com/api/v3/", model.ApiKey);
        var resp = client.SendTextChat(new TextChatReq()
        {
            model = model.TextModelId, apiPath = "chat/completions", messages = new[]
            {
                new TextMessage()
                {
                    content = sysContent,
                    role = "system"
                },
                new TextMessage()
                {
                    content = userContent,
                    role = "user"
                }
            }
        });
        if (!resp.Success)
        {
            return R.Faild<string>(resp.Message);
        }

        var first = resp.Data.choices.FirstOrDefault();
        if (first == null || first.message == null)
        {
            return R.Faild<string>("返回内容异常");
        }

        return R.OK(first.message.content);
    }

    public static R<string> ImagePrompt(ModelAuthInfo model, string userContent, List<string> imageUrl)
    {
        var client = new DouBaoClient("https://ark.cn-beijing.volces.com/api/v3/", model.ApiKey);
        var userContents = new List<ImageContent>();
        userContents.Add(new ImageContent()
        {
            text = userContent,
            type = "text",
        });
        userContents.AddRange(imageUrl.Select(url =>
        {
            return new ImageContent()
            {
                type = "image_url",
                image_url = new ImageUrl()
                {
                    url = url,
                }
            };
        }));
        var resp = client.SendImageChat(new ImageChatReq()
        {
            model = model.ImageModelId!, apiPath = "chat/completions", messages =
            [
                new ImageMessage()
                {
                    content = userContents.ToArray(),
                    role = "user"
                }
            ]
        });
        if (!resp.Success)
        {
            return R.Faild<string>(resp.Message);
        }

        var first = resp.Data.choices.FirstOrDefault();
        if (first == null || first.message == null)
        {
            return R.Faild<string>("返回内容异常");
        }

        return R.OK(first.message.content);
    }
}