using System.ComponentModel;

namespace SchemaBuilder.Svc.Models.Table;

/// <summary>
/// 直播间观察员
/// </summary>
public class LiveAccount : ITable, IUserId, ITenantId
{
    public long Id { get; set; }
    public LivePlatform Platform { get; set; }
    public AccountRoleType RoleType { get; set; }
    public long TenantId { get; set; }

    /// <summary>
    /// 归属用户
    /// </summary>
    public long UserId { get; set; }

    public string? Name { get; set; }
    public string? PlatformUserId { get; set; }
    public string? AuthJson { get; set; }
}

[Flags]
public enum LivePlatform
{
    [Description("抖音")] Douyin = 1,
    [Description("视频号")] Weixin = 2,
    [Description("小红书")] Xiaohongshu = 3,
    [Description("快手")] Kuaishou = 4,
}

[Flags]
public enum AccountRoleType
{
    [Description("观察员")] Observer = 1,
    [Description("操作员")] HuangCheOprate = 2,
}