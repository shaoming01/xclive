namespace SchemaBuilder.Svc.Models.Table;

public class UserInfo : ITable, ITenantId
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public DateTime Created { get; set; }
    public string? CardKey { get; set; }
    public string? Name { get; set; }
    public string? Password { get; set; }
    public string? Memo { get; set; }
    public bool IsAdmin { get; set; }
    public int IntVal { get; set; }
    public decimal DecimalVal { get; set; }
}

public class UserRoleDetail : ITable
{
    public long Id { get; set; }
    public long HeaderId { get; set; }
    public long RoleId { get; set; }
}

public class UserFavMenuDetail : ITable
{
    public long Id { get; set; }
    public long HeaderId { get; set; }
    public long MenuId { get; set; }
}