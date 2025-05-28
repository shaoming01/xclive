using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using SchemaBuilder.Svc.Core;

namespace SchemaBuilder.Svc.Helpers;

public class OpenAIHelper
{
    //  private static readonly string
    //    apiKey = "sk-XJvf5b1AYWqlmlikEeUm4cyhgEHjUXh5tfcRIERlFY7yk46u"; // 替换为你自己的 OpenAI API 密钥

    // private static readonly string apiUrl = "https://newapi.super-api.cn/v1/chat/completions";
    private static readonly string
        apiKey = "sk-XJvf5b1AYWqlmlikEeUm4cyhgEHjUXh5tfcRIERlFY7yk46u"; // 替换为你自己的 OpenAI API 密钥

    private static readonly string apiUrl = "https://newapi.super-api.cn/v1/chat/completions";


    public static R<OpenAIResponse> GetChatGptResponse(ReqModel model)
    {
        using var client = new HttpClient();
        // 设置 Authorization header
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
        var jsonContent = JsonSerializer.Serialize(model);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        // 发起请求并获取响应
        var response = client.PostAsync(apiUrl, content).Result;

        if (!response.IsSuccessStatusCode)
        {
            return R.Faild<OpenAIResponse>($"Error: {response.StatusCode}, {response.Content.ReadAsStringAsync().Result}");
        }

        var responseContent = response.Content.ReadAsStringAsync().Result;

        // 解析并提取生成的内容
        var openAiResponse = JsonSerializer.Deserialize<OpenAIResponse>(responseContent);
        return R.OK(openAiResponse);
    }

    public class OpenAIResponse
    {
        public class Choice
        {
            public int index { get; set; }
            public string finish_reason { get; set; }
            public Message message { get; set; }
        }

        public class Usage
        {
            public int prompt_tokens { get; set; }
            public int completion_tokens { get; set; }
            public int total_tokens { get; set; }
        }


        public string id { get; set; }
        [JsonPropertyName("object")] public string objectType { get; set; }
        public int created { get; set; }
        public string model { get; set; }
        public Choice[] choices { get; set; }
        public Usage usage { get; set; }
    }

    public class Message
    {
        public string role { get; set; }
        public string content { get; set; }
    }

    public class ReqModel
    {
        public string model { get; set; }
        public Message[] messages { get; set; }
    }
}