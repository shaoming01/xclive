using ServiceStack.DataAnnotations;

namespace SchemaBuilder.Svc.Models.Table;

/// <summary>
/// AI行业主播
/// </summary>
public class AiVerticalAnchor : ITable, ITenantId
{
    [PrimaryKey] public long Id { get; set; }
    public long TenantId { get; set; }
    public long UserId { get; set; }

    /// <summary>
    /// 行业主播名称
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 主播音色模型
    /// </summary>
    public long? PrimaryTtsModelId { get; set; }

    /// <summary>
    /// 助播音色模型
    /// </summary>
    public long? SecondaryTtsModelId { get; set; }

    /// <summary>
    /// 生成话术模板
    /// </summary>
    public string? ScriptTemplateIds { get; set; }

    /// <summary>
    /// 回复话术模板
    /// </summary>
    public string? ChatTemplateIds { get; set; }

    /// <summary>
    /// 互动话术模板
    /// </summary>
    public string? InteractTemplateIds { get; set; }
}