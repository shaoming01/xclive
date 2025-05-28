using ServiceStack.DataAnnotations;

namespace SchemaBuilder.Svc.Models.Table;

public class Menu : ITable, ITenantId
{
    [StringLength(128)] public string? Title { get; set; }
    [StringLength(256)] public string? Desc { get; set; }
    public string? Icon { get; set; }
    public string? Url { get; set; }
    [PrimaryKey] public long Id { get; set; }
    public long TenantId { get; set; }
    public long ParentId { get; set; }
    public bool Hidden { get; set; }
    public string? RoleFunctions { get; set; }
    [StringLength(12000)] public string? AddCol1 { get; set; }
}