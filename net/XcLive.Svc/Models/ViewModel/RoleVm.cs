using SchemaBuilder.Svc.Core.QueryHelper.Data;
using SchemaBuilder.Svc.Models.Attr;
using SchemaBuilder.Svc.Models.Schema;
using SchemaBuilder.Svc.Models.Table;

namespace SchemaBuilder.Svc.Models.ViewModel;

[SearchContainer(nameof(RoleVm))]
public class RoleQuery : QueryDbEx<Role, RoleVm>, ITenantIdQuery
{
    [FieldEditor("角色名称", WrapperColSpan = 16, Span = 4)]
    public StringQuery? Name { get; set; }

    [FieldEditor("备注", WrapperColSpan = 16, Span = 4)]
    public StringQuery? Memo { get; set; }

    [FieldEditor("测试字段1", WrapperColSpan = 16, Span = 4)]
    public StringQuery? Test1 { get; set; }

    [FieldEditor("测试字段2", WrapperColSpan = 16, Span = 4)]
    public TableType? Test2 { get; set; }

    public long? TenantId { get; set; }
}

[FullTable(nameof(Role), TableType.MainTable, QueryDataUrl = "/api/role/queryList",
    QueryCountUrl = "/api/role/queryCount", DeleteIdsUrl = "/api/role/deleteIds", PageSize = 100)]
[TableTool(ToolType.Add, 7497360857977865589, 10), TableTool(ToolType.Edit, 7497360857977865589, 20),
 TableTool(ToolType.Delete, 0, 30)]
public class RoleVm
{
    public long Id { get; set; }
    [TableColumn("名称")] public string? Name { get; set; }
    [TableColumn("备注说明", Editable = true)] public string? Memo { get; set; }

    [TableColumn("T1", Tip = "TIPTIP", Editable = true, ValueListType = ValueDisplayType.Type1,
        RenderType = CellRenderType.ListSelectRender)]
    public string? Test1 { get; set; }

    [TableColumn("T2", Editable = true)] public TableType Test2 { get; set; }
}

[SearchContainer(nameof(RoleSelectVm))]
public class RoleSelectQuery : QueryDbEx<Role, RoleSelectVm>
{
    [FieldEditor("名称", WrapperColSpan = 16, Span = 8)]
    public StringQuery? Name { get; set; }

    [FieldEditor("备注说明", WrapperColSpan = 16, Span = 8)]
    public StringQuery? Memo { get; set; }
}

[FullTable(nameof(RoleSelectVm), TableType.MainTable, QueryDataUrl = "/api/role/RoleSelectQueryList",
    QueryCountUrl = "/api/role/RoleSelectQueryCount", PageSize = 20, MultiSelection = true)]
[ModalDataSelect(nameof(RoleSelectVm) + "_ModalDataSelect", "选择角色")]
public class RoleSelectVm
{
    public long Id { get; set; }
    [TableColumn("名称")] public string? Name { get; set; }
    [TableColumn("备注说明")] public string? Memo { get; set; }
}

[ObjectEditor(nameof(RoleEditVm), "角色")]
[ModalObjectEditor(nameof(RoleEditVm) + "_ModalObjectEditor", "角色编辑", "/api/role/GetEditVm", "/api/role/SaveEditVm")]
public class RoleEditVm
{
    public long Id { get; set; }
    [FieldEditor("角色名称", Require = true)] public string? Name { get; set; }
    [FieldEditor("备注说明")] public string? Memo { get; set; }

    [FieldEditor("T1", ValueListType = ValueDisplayType.Type1)]
    public string? Test1 { get; set; }

    [FieldEditor("T2")] public TableType Test2 { get; set; }
}