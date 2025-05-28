using SchemaBuilder.Svc.Core.QueryHelper.Data;
using SchemaBuilder.Svc.Models.Attr;
using SchemaBuilder.Svc.Models.Table;

namespace SchemaBuilder.Svc.Models.ViewModel;

[SearchContainer(nameof(LiveScriptTemplateVm))]
public class LiveScriptTemplateQuery : QueryDbEx<LiveScriptTemplate, LiveScriptTemplateVm>, ITenantIdQuery, IUserIdQuery
{
    [FieldEditor("模板名称", WrapperColSpan = 16, Span = 4)]
    public StringQuery? Name { get; set; }

    [FieldEditor("使用场景", WrapperColSpan = 16, Span = 4)]
    public UsageType? Usage { get; set; }

    [FieldEditor("系统模板", WrapperColSpan = 16, Span = 4)]
    public StringQuery? SystemTemplate { get; set; }

    [FieldEditor("用户模板", WrapperColSpan = 16, Span = 4)]
    public StringQuery? UserTemplate { get; set; }

    public long? TenantId { get; set; }
    public long? UserId { get; set; }
}

[FullTable(nameof(LiveScriptTemplateVm), TableType.MainTable,
    QueryDataUrl = "/api/LiveScriptTemplate/LiveScriptTemplateQueryList",
    QueryCountUrl = "/api/LiveScriptTemplate/LiveScriptTemplateQueryCount",
    DeleteIdsUrl = "/api/LiveScriptTemplate/LiveScriptTemplateDelete",
    PageSize = 100,
    MultiSelection = true)]
[TableTool(ToolType.Add, nameof(LiveScriptTemplateEditVm) + "_ModalObjectEditor", 10),
 TableTool(ToolType.Edit, nameof(LiveScriptTemplateEditVm) + "_ModalObjectEditor", 20),
 TableTool(ToolType.Delete, 0, 30)]
public class LiveScriptTemplateVm
{
    [TableColumn("Id")] public long Id { get; set; }
    [TableColumn("名称")] public string? Name { get; set; }
    [TableColumn("使用场景")] public UsageType Usage { get; set; }

    [TableColumn("系统模板", RenderType = CellRenderType.LongStringRender)]
    public string? SystemTemplate { get; set; }

    [TableColumn("用户模板", RenderType = CellRenderType.LongStringRender)]
    public string? UserTemplate { get; set; }
}

[ModalObjectEditor(nameof(LiveScriptTemplateEditVm) + "_ModalObjectEditor", "LiveScript 模板编辑",
    "/api/LiveScriptTemplate/LiveScriptTemplateGetEditVm",
    "/api/LiveScriptTemplate/LiveScriptTemplateSaveEditVm", SizeMode = 5)]
public class LiveScriptTemplateEditVm
{
    public long Id { get; set; }

    [FieldEditor("名称", Offset = 0, Span = 24, LabelColSpan = 3, WrapperColSpan = 21)]
    public string? Name { get; set; }

    [FieldEditor("使用场景", Offset = 0, Span = 24, LabelColSpan = 3, WrapperColSpan = 21)]
    public UsageType Usage { get; set; }

    [FieldEditor("系统模板", Offset = 0, Span = 24, Rows = 6, LabelColSpan = 3, WrapperColSpan = 21,
        Tip = "支持变量：{{互动消息}} {{个性描述}} {{直播间描述}}")]
    public string? SystemTemplate { get; set; }

    [FieldEditor("用户模板", Offset = 0, Span = 24, Rows = 6, LabelColSpan = 3, WrapperColSpan = 21,
        Tip = "支持变量：{{互动消息}} {{个性描述}} {{直播间描述}}")]
    public string? UserTemplate { get; set; }
}