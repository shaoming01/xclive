namespace SchemaBuilder.Svc.Models.Schema;

public class ApiCallSchema
{
    public string ApiUrl { get; set; }
    public Dictionary<string, object>? QueryParams { get; set; }
    public object? PostParams { get; set; }
    public bool Cacheable { get; set; }
}