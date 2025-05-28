using ServiceStack.DataAnnotations;

namespace SchemaBuilder.Svc.Models.Table;

/// <summary>
/// 上下架任务配置，一个配置里有多个子任务，如停顿，上架讲解等
/// </summary>
public class ShelfTaskConfig : ITable, ITenantId
{
    [PrimaryKey] public long Id { get; set; }
    public long TenantId { get; set; }
    public long UserId { get; set; }
    [StringLength(128)] public string? Name { get; set; }
    [StringLength(16000)] public string? DataJson { get; set; }
}