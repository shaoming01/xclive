namespace SchemaBuilder.Svc.Models.Dto;

/// <summary>
/// 后端内部传递用的
/// </summary>
public class UserLoginInfo
{
    public long UserId { get; set; }
    public long TenantId { get; set; }
    public string? Name { get; set; }

    public List<string> Tokens { get; set; }
}

/// <summary>
/// 入前端传的
/// </summary>
public class UserLoginInfoVm
{
    public long UserId { get; set; }
    public string? Name { get; set; }
    public string Token { get; set; }
    public long TenantId { get; set; }
    public string days { get; set; }
}