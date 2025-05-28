using ServiceStack.DataAnnotations;

namespace SchemaBuilder.Svc.Models.Table;

/// <summary>
/// 话术生成
/// </summary>
public class LiveScript : ITable, IUserId, ITenantId
{
    [PrimaryKey] public long Id { get; set; }
    public long TenantId { get; set; }
    public long UserId { get; set; }
    public long? LiveScriptTemplateId { get; set; }
    public long? LiveRoomId { get; set; }
    public string? GuestMessage { get; set; }
    [StringLength(4000)] public string? AssistantText { get; set; }
}

/// <summary>
/// 话术分句
/// </summary>
public class LiveScriptVoiceDetail : IDetailTable
{
    [PrimaryKey] public long Id { get; set; }
    public long HeaderId { get; set; }
    [StringLength(1000)] public string? Text { get; set; }
}