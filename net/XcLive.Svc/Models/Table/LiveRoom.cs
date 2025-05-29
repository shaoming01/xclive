using ServiceStack.DataAnnotations;

namespace SchemaBuilder.Svc.Models.Table;

public class LiveRoom : ITable, IUserId, ITenantId
{
    [PrimaryKey] public long Id { get; set; }
    public long TenantId { get; set; }
    public long UserId { get; set; }
    public string? Name { get; set; }

    /// <summary>
    /// 直播间描述、商品或服务描述
    /// </summary>
    public string? ProductText { get; set; }

    /// <summary>
    /// 根据添加进来的主讲商品分析得到的
    /// </summary>
    public string? PromotionText { get; set; }

    /// <summary>
    /// 角色个性描述，限制要求等
    /// </summary>
    public string? PersonaText { get; set; }
}