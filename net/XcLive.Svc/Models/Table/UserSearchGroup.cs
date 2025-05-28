using ServiceStack.DataAnnotations;

namespace SchemaBuilder.Svc.Models.Table;

public class UserSearchGroup : ITable, IUserId
{
    public long Id { get; set; }
    public int Index { get; set; }
    public long UserId { get; set; }
    public string? Path { get; set; }
    public string? Name { get; set; }
    [StringLength(80000)] public string? ConditionsJson { get; set; }
}