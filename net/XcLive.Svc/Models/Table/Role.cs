using SchemaBuilder.Svc.Models.Attr;

namespace SchemaBuilder.Svc.Models.Table;

public class Role : ITable, ITenantId
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public string? Name { get; set; }
    public string? Memo { get; set; }

    public string? Test1 { get; set; }

    public TableType Test2 { get; set; }
}

public class RoleMenuDetail : ITable
{
    public long Id { get; set; }
    public long HeaderId { get; set; }
    public long MenuId { get; set; }

    /// <summary>
    /// 可操作按钮
    /// </summary>
    public string? Options { get; set; }
}