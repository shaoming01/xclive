using SchemaBuilder.Svc.Core.QueryHelper.Data;
using SchemaBuilder.Svc.Models.Attr;
using SchemaBuilder.Svc.Models.Table;
using ServiceStack;

namespace SchemaBuilder.Svc.Models.ViewModel;

[SearchContainer(nameof(UserVm))]
public class UserQuery : QueryDb<UserInfo, UserVm>
{
    public long[]? Id { get; set; }

    [FieldEditor("创建时间", LabelColSpan = 6, WrapperColSpan = 18, Span = 6)]
    public DateQuery? Created { get; set; }

    [FieldEditor("用户名", LabelColSpan = 6, WrapperColSpan = 18, Span = 6)]
    public StringQuery? Name { get; set; }
}

[ModalObjectEditor(nameof(UserEditVm) + "_ModalObjectEditor", "用户", "/api/user/get", "/api/user/save")]
public class UserEditVm : ITable
{
    public long Id { get; set; }
    [FieldEditor("用户名")] public string? Name { get; set; }

    [FieldEditor("密码", Offset = 0, Span = 24)]
    public string? Password { get; set; }

    [FieldEditor("重复密码", Offset = 0, Span = 24)]
    public string? Password2 { get; set; }

    [FieldEditor("备注", Offset = 0, Span = 24)]
    public string? Memo { get; set; }

    [DetailTable("角色")]
    [TableTool(ToolType.LocalDelete, 0, 30)]
    public List<UserRoleDetailVm>? UserRoles { get; set; } = new();
}

public class UserRoleDetailQuery : QueryDb<UserRoleDetail, UserRoleDetailVm>, ILeftJoin<UserRoleDetail, Role>
{
    public long HeaderId { get; set; }
}

[FullTable(nameof(UserVm), TableType.DetailTable, Title = "角色",
    QueryDataUrl = "/api/user/UserRoleDetailQueryList",
    QueryCountUrl = "/api/user/UserRoleDetailQueryCount")]
public class UserRoleDetailVm
{
    public long Id { get; set; }
    public long HeaderId { get; set; }
    public long RoleId { get; set; }

    [TableColumn("角色名称")] public string RoleName { get; set; }
}

[FullTable(nameof(UserVm), TableType.MainTable, MultiSelection = true,
    QueryDataUrl = "/api/user/QueryList",
    QueryCountUrl = "/api/user/QueryCount",
    DeleteIdsUrl = "/api/user/delete")]
[TableTool(ToolType.Add, 7497378383251562416, 10), TableTool(ToolType.Edit, 7497378383251562416, 20),
 TableTool(ToolType.Delete, Index = 30)]
public class UserVm
{
    [TableColumn("Id")] public long Id { get; set; }

    [TableColumn("创建时间")] public DateTime Created { get; set; }
    [TableColumn("用户名")] public string? Name { get; set; }
    [TableColumn("备注")] public string? Memo { get; set; }
    [TableColumn("管理员")] public bool IsAdmin { get; set; }
    [TableColumn("IntVal")] public int IntVal { get; set; }
    [TableColumn("DecimalVal")] public decimal DecimalVal { get; set; }
}

public class UserRegisterVm
{
    public string UserName { get; set; }
    public string Password { get; set; }
    public string CardKey { get; set; }
}

public class UserResetPasswordVm
{
    public string UserName { get; set; }
    public string Password { get; set; }
    public string CardKey { get; set; }
}