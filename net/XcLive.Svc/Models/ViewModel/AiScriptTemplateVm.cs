using SchemaBuilder.Svc.Models.Attr;
using SchemaBuilder.Svc.Models.Table;

namespace SchemaBuilder.Svc.Models.ViewModel;

[FullTable(nameof(AiScriptTemplateVm), TableType.MainTable,
    QueryDataUrl = "/api/LiveScriptTemplate/LiveScriptTemplateQueryList",
    DeleteIdsUrl = "/api/LiveScriptTemplate/LiveScriptTemplateDelete",
    PageSize = 100,
    MultiSelection = false)]
[TableTool(ToolType.Add, nameof(AiScriptTemplateEditVm) + "_ModalObjectEditor", 10),
 TableTool(ToolType.Edit, nameof(AiScriptTemplateEditVm) + "_ModalObjectEditor", 20),
 TableTool(ToolType.Delete, 0, 30)]
public class AiScriptTemplateVm
{
    public long Id { get; set; }
    [TableColumn("模板名称")] public string? Name { get; set; }
    [TableColumn("使用场景")] public UsageType Usage { get; set; }

    [TableColumn("角色说明", Width = 200, RenderType = CellRenderType.LongStringRender)]
    public string? SystemTemplate { get; set; }

    [TableColumn("模板内容", Width = 200, RenderType = CellRenderType.LongStringRender)]
    public string? UserTemplate { get; set; }
}

[ModalObjectEditor(nameof(AiScriptTemplateEditVm) + "_ModalObjectEditor", "Ai话术模板编辑",
    "/api/LiveScriptTemplate/LiveScriptTemplateGetEditVm",
    "/api/LiveScriptTemplate/LiveScriptTemplateSaveEditVm", SizeMode = 5, Centered = true)]
public class AiScriptTemplateEditVm
{
    public long Id { get; set; }

    [FieldEditor("模板名称", Offset = 0, Span = 24, LabelColSpan = 3, WrapperColSpan = 21)]
    public string? Name { get; set; }

    [FieldEditor("使用场景", Offset = 0, Span = 24, LabelColSpan = 3, WrapperColSpan = 21)]
    public UsageType Usage { get; set; }

    [FieldEditor("角色说明", Offset = 0, Span = 24, Rows = 3, LabelColSpan = 3, WrapperColSpan = 21,
        Tip = "支持变量：{{互动消息}} {{个性描述}} {{直播间描述}}")]
    public string? SystemTemplate { get; set; }

    [FieldEditor("模板内容", Offset = 0, Span = 24, Rows = 5, LabelColSpan = 3, WrapperColSpan = 21,
        Tip = "支持变量：{{互动消息}} {{个性描述}} {{直播间描述}}")]
    public string? UserTemplate { get; set; }
}