using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using SchemaBuilder.Svc.Core.Ext;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace SchemaBuilder.Svc.Svc;

public class TtsApiClient
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public TtsApiClient(string baseUrl = "http://localhost:8866")
    {
        _httpClient = new HttpClient()
        {
            Timeout = new TimeSpan(0, 3, 0),
        };
        _baseUrl = baseUrl.TrimEnd('/');
    }

    private async Task<T> SendRequest<T>(string url, HttpMethod method, object data = null)
    {
        var request = new HttpRequestMessage(method, $"{_baseUrl}{url}");
        if (data != null)
        {
            string jsonData = JsonSerializer.Serialize(data);
            request.Content = new StringContent(jsonData, Encoding.UTF8, "application/json");
        }

        HttpResponseMessage response = await _httpClient.SendAsync(request);
        return ProcessResp<T>(response);
    }

    private T ProcessResp<T>(HttpResponseMessage response)
    {
        if (response.StatusCode == HttpStatusCode.OK && typeof(T) == typeof(Resp<byte[]>))
        {
            var byteContent = response.Content.ReadAsByteArrayAsync().Result;
            return (T)(object)new Resp<byte[]>()
            {
                Data = byteContent,
            };
        }

        var responseString = response.Content.ReadAsStringAsync().Result;
        if (!responseString.IsJson())
        {
            throw new Exception("返回内容异常：" + response.StatusCode + ":" + responseString);
        }

        return JsonConvert.DeserializeObject<T>(responseString);
    }

    private async Task<T> PostFile<T>(string url, string filePath)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}{url}");
        var formData = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(await File.ReadAllBytesAsync(filePath));
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        formData.Add(fileContent, "file", Path.GetFileName(filePath));
        request.Content = formData;
        HttpResponseMessage response = await _httpClient.SendAsync(request);
        return ProcessResp<T>(response);
    }

    public async Task<Resp<ModelAddInfo>> AddModel(ModelAddRequest request) =>
        await SendRequest<Resp<ModelAddInfo>>("/models/add", HttpMethod.Post, request);

    public async Task<Resp<byte[]>> GetVoice(VoiceRequest request) =>
        await SendRequest<Resp<byte[]>>("/voice", HttpMethod.Post, request);
    
    public async Task<Resp<byte[]>> GetVoiceFile(VoiceFileRequest request) =>
        await SendRequest<Resp<byte[]>>("/voice/file", HttpMethod.Post, request);

    public async Task<Resp<object>> DeleteModel(ModelDeleteReq request) =>
        await SendRequest<Resp<object>>("/models/delete", HttpMethod.Post, request);

    public async Task<Resp<object>> Control(ControlRequest request) =>
        await SendRequest<Resp<object>>("/control", HttpMethod.Post, request);

    public async Task<object> GetStatus() =>
        await SendRequest<object>("/status", HttpMethod.Get);

    public async Task<Dictionary<string, ModelInfoResponse>> GetModelInfo() =>
        await SendRequest<Dictionary<string, ModelInfoResponse>>("/models/info", HttpMethod.Post, null);

    public async Task<Resp<string>> GetVoiceText(VoiceTextRequest request) =>
        await SendRequest<Resp<string>>("/voice/text", HttpMethod.Post, request);

    public async Task<Resp<string>> UploadVoiceFile(string filePath) =>
        await PostFile<Resp<string>>("/voice/save", filePath);


    public async Task<Resp<ModelPathInfo[]>> GetModelListPaths() =>
        await SendRequest<Resp<ModelPathInfo[]>>("/models/listpaths", HttpMethod.Get);
}

// DTO 定义
public class ModelAddRequest
{
    public string model_path { get; set; }
    public int port { get; set; }
}

public class ModelInfo
{
    public string config_path { get; set; }
    public string device { get; set; }
    public Dictionary<string, string> id2Spk { get; set; }
}

public class VoiceRequest
{
    public string text { get; set; }
    public string model_id { get; set; }
}
public class VoiceFileRequest
{
    public string filename { get; set; }
}

public class ControlRequest
{
    public string command { get; set; }
    public string port { get; set; }
}

public class ModelAddInfo
{
    public string model_id { get; set; }
    public object model_info { get; set; }
}

public class ModelDeleteReq
{
    public string model_id { get; set; }
}

public class ModelInfoResponse
{
    public string config_path { get; set; }
    public string device { get; set; }
    public Dictionary<string, string> id2spk { get; set; }
    public Dictionary<string, string> spk2id { get; set; }
    public string language { get; set; }
    public string model_path { get; set; }
    public string version { get; set; }
}

public class VoiceTextRequest
{
    public string path { get; set; }
}

public class ModelPathInfo
{
    public string config_path { get; set; }
    public string model_path { get; set; }
    public string speaker_name { get; set; }
}

public class Resp<T>
{
    /// <summary>
    /// 0成功，其他错误
    /// </summary>
    public int status { get; set; }

    public string detail { get; set; }
    public T Data { get; set; }
}