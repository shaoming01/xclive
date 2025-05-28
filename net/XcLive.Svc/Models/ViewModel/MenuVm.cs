using SchemaBuilder.Svc.Core.QueryHelper.Data;
using SchemaBuilder.Svc.Models.Attr;
using SchemaBuilder.Svc.Models.Table;
using ServiceStack;

namespace SchemaBuilder.Svc.Models.ViewModel;

[SearchContainer(nameof(MenuVm))]
public class MenuQuery : QueryDbEx<Menu, MenuVm>, ITenantIdQuery
{
    [FieldEditor("标题", WrapperColSpan = 16, Span = 4)]
    public StringQuery? Title { get; set; }

    [FieldEditor("描述", WrapperColSpan = 16, Span = 4)]
    public StringQuery? Desc { get; set; }

    [FieldEditor("图标", WrapperColSpan = 16, Span = 4)]
    public StringQuery? Icon { get; set; }

    [FieldEditor("URL", WrapperColSpan = 16, Span = 4)]
    public StringQuery? Url { get; set; }

    [FieldEditor("父级菜单", WrapperColSpan = 16, Span = 4)]
    public long? ParentId { get; set; }

    [FieldEditor("隐藏", WrapperColSpan = 16, Span = 4)]
    public bool? Hidden { get; set; }

    [FieldEditor("角色功能", WrapperColSpan = 16, Span = 4)]
    public StringQuery? RoleFunctions { get; set; }

    [FieldEditor("附加列1", WrapperColSpan = 16, Span = 4)]
    public StringQuery? AddCol1 { get; set; }

    public long? TenantId { get; set; }
}

[FullTable(nameof(Menu), TableType.MainTable, QueryDataUrl = "/api/menu/MenuQueryList",
    QueryCountUrl = "/api/menu/MenuQueryCount", DeleteIdsUrl = "/api/menu/MenuDelete", PageSize = 100)]
[TableTool(ToolType.Add, nameof(MenuEditVm) + "_ModalObjectEditor", 10),
 TableTool(ToolType.Edit, nameof(MenuEditVm) + "_ModalObjectEditor", 20),
 TableTool(ToolType.Delete, 0, 30),]
public class MenuVm
{
    [TableColumn("菜单名称")] public string? Title { get; set; }
    [TableColumn("说明")] public string? Desc { get; set; }

    [TableColumn("图标", RenderType = CellRenderType.IconRender)]
    public string? Icon { get; set; }

    [TableColumn("URL")] public string? Url { get; set; }

    [TableColumn("是否隐藏")] public bool Hidden { get; set; }
    [TableColumn("权限功能")] public string? RoleFunctions { get; set; }
    [TableColumn("Id")] public long Id { get; set; }
    [TableColumn("ParentId")] public long ParentId { get; set; }
}

[ModalObjectEditor(nameof(MenuEditVm) + "_ModalObjectEditor", "菜单编辑", "/api/menu/MenuGetEditVm", "/api/menu/MenuSaveEditVm",
    SizeMode = 6)]
public class MenuEditVm
{
    public long Id { get; set; }
    [FieldEditor("菜单名称")] public string? Title { get; set; }
    [FieldEditor("说明")] public string? Desc { get; set; }

    [FieldEditor("图标", EditorType = FieldEditorType.IconSelectInput)]
    public string? Icon { get; set; }

    [FieldEditor("URL")] public string? Url { get; set; }

    [FieldEditor("是否隐藏")] public bool Hidden { get; set; }
    [FieldEditor("权限功能", Tip = "多个用逗号分隔")] public string? RoleFunctions { get; set; }
    [FieldEditor("ParentId")] public long ParentId { get; set; }
}