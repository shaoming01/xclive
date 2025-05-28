using ServiceStack.DataAnnotations;

namespace SchemaBuilder.Svc.Models.Table;

public class SettingInfo : ITable, ITenantId, IUserId
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long UserId { get; set; }
    public string ClassName { get; set; }
    [StringLength(10000)] public string JsonValue { get; set; }
}