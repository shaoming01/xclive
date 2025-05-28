using SchemaBuilder.Svc.Core.QueryHelper.Data;
using SchemaBuilder.Svc.Models.Attr;
using SchemaBuilder.Svc.Models.Table;

namespace SchemaBuilder.Svc.Models.ViewModel;

// Query
[SearchContainer(nameof(ShelfTaskConfigVm))]
public class ShelfTaskConfigQuery : QueryDbEx<ShelfTaskConfig, ShelfTaskConfigVm>, ITenantIdQuery
{
    [FieldEditor("配置名称", WrapperColSpan = 16, Span = 4)]
    public StringQuery? Name { get; set; }

    public long? TenantId { get; set; }
}

// 列表视图
[FullTable(nameof(ShelfTaskConfigVm), TableType.MainTable,
    QueryDataUrl = "/api/ShelfTaskConfig/ShelfTaskConfigQueryList",
    QueryCountUrl = "/api/ShelfTaskConfig/ShelfTaskConfigQueryCount",
    DeleteIdsUrl = "/api/ShelfTaskConfig/ShelfTaskConfigDelete",
    PageSize = 100, MultiSelection = true)]
[TableTool(ToolType.Add, nameof(ShelfTaskConfigEditVm) + "_ModalObjectEditor", 10),
 TableTool(ToolType.Edit, nameof(ShelfTaskConfigEditVm) + "_ModalObjectEditor", 20),
 TableTool(ToolType.Delete, 0, 30)]
public class ShelfTaskConfigVm
{
    [TableColumn("Id")] public long Id { get; set; }

    [TableColumn("配置名称")] public string? Name { get; set; }

    [TableColumn("配置数据")] public string? DataJson { get; set; }
}

// 编辑视图
[ModalObjectEditor(nameof(ShelfTaskConfigEditVm) + "_ModalObjectEditor", "上下架任务配置编辑",
    "/api/ShelfTaskConfig/ShelfTaskConfigGetEditVm",
    "/api/ShelfTaskConfig/ShelfTaskConfigSaveEditVm", SizeMode = 3)]
public class ShelfTaskConfigEditVm
{
    public long Id { get; set; }

    [FieldEditor("配置名称", Offset = 0, Span = 24, LabelColSpan = 4, WrapperColSpan = 20)]
    public string? Name { get; set; }

    [FieldEditor("配置数据", Offset = 0, Span = 24, Rows = 8, LabelColSpan = 4, WrapperColSpan = 20)]
    public string? DataJson { get; set; }
}