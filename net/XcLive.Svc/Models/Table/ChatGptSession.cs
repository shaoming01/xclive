using ServiceStack.DataAnnotations;

namespace SchemaBuilder.Svc.Models.Table;

public class ChatGptSession : ITable
{
    public long Id { get; set; }
    public string? Title { get; set; }
}

public class ChatGptSessionMessage : ITable
{
    public long Id { get; set; }
    public long HeaderId { get; set; }
    public string Role { get; set; }
    [StringLength(80000)] public string Message { get; set; }
}