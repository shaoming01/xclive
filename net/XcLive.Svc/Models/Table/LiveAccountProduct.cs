namespace SchemaBuilder.Svc.Models.Table;

public class LiveAccountProduct : ITable, IUserId, ITenantId
{
    public long Id { get; set; }
    public LivePlatform Platform { get; set; }
    public long LiveAccountId { get; set; }
    public long TenantId { get; set; }

    /// <summary>
    /// 归属用户
    /// </summary>
    public long UserId { get; set; }

    public string? ProductName { get; set; }
    public string? ProductId { get; set; }
    public string? ImgUrl { get; set; }
    public string? ProductJson { get; set; }
}