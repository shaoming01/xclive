namespace SchemaBuilder.Svc.Models.Table;

public class CardKey : ITable, ITenantId
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public DateTime Created { get; set; }
    public bool InValid { get; set; }

    /// <summary>
    /// 时长，单位：天
    /// </summary>
    public int? Days { get; set; }

    /// <summary>
    /// 过期时间，为空表示永久有效
    /// </summary>
    public DateTime? Expiry { get; set; }

    public DateTime? ActiveDate { get; set; }

    /// <summary>
    /// 初次激活的用户
    /// </summary>
    public long? ActiveUserId { get; set; }

    public long? CurrentUserId { get; set; }
}