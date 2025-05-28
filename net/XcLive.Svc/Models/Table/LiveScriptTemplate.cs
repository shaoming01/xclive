using ServiceStack.DataAnnotations;

namespace SchemaBuilder.Svc.Models.Table;

/// <summary>
/// 租户设置好，用户不能自己调
/// </summary>
public class LiveScriptTemplate : ITenantId, ITable, IUserId
{
    [PrimaryKey] public long Id { get; set; }
    public long TenantId { get; set; }

    /// <summary>
    /// 设置了用户Id私有，否则公有
    /// </summary>
    public long UserId { get; set; }

    public string? Name { get; set; }
    public UsageType Usage { get; set; }
    public string? SystemTemplate { get; set; }
    public string? UserTemplate { get; set; }
}