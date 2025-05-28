using SchemaBuilder.Svc.Core.QueryHelper.Data;
using SchemaBuilder.Svc.Models.Attr;
using SchemaBuilder.Svc.Models.Schema;
using SchemaBuilder.Svc.Models.Table;

namespace SchemaBuilder.Svc.Models.ViewModel;

// 列表视图
[FullTable(nameof(LiveAiAnchorVm), TableType.MainTable,
    QueryDataUrl = "/api/AiVerticalAnchor/AiVerticalAnchorQueryList",
    DeleteIdsUrl = "/api/AiVerticalAnchor/AiVerticalAnchorDelete",
    PageSize = 100, MultiSelection = false)]
[TableTool(ToolType.Add, nameof(LiveAiAnchorVm) + "_ModalObjectEditor", 10),
 TableTool(ToolType.Edit, nameof(LiveAiAnchorVm) + "_ModalObjectEditor", 20),
 TableTool(ToolType.Delete, 0, 30)]
public class LiveAiAnchorVm
{
    public long Id { get; set; }

    [TableColumn("名称")] public string? Name { get; set; }

    [TableColumn("主播语音", ValueListType = ValueDisplayType.TtsModel)]
    public long? PrimaryTtsModelId { get; set; }

    [TableColumn("助播语音", ValueListType = ValueDisplayType.TtsModel)]
    public long? SecondaryTtsModelId { get; set; }

    [TableColumn("生成话术模板", Width = 180, ValueListType = ValueDisplayType.MainScriptTemplate)]
    public string? ScriptTemplateIds { get; set; }

    [TableColumn("聊天回复模板", Width = 180, ValueListType = ValueDisplayType.ChatScriptTemplate)]
    public string? ChatTemplateIds { get; set; }

    [TableColumn("互动回复模板", Width = 180, ValueListType = ValueDisplayType.InteractScriptTemplate)]
    public string? InteractTemplateIds { get; set; }
}

// 编辑视图
[ModalObjectEditor(nameof(LiveAiAnchorVm) + "_ModalObjectEditor", "AI主播编辑",
    "/api/AiVerticalAnchor/AiVerticalAnchorGetEditVm",
    "/api/AiVerticalAnchor/AiVerticalAnchorSaveEditVm", SizeMode = 4, Centered = true)]
public class LiveAiAnchorEditVm
{
    public long Id { get; set; }

    [FieldEditor("名称", AllowClear = true, Offset = 0, Span = 24, LabelColSpan = 6, WrapperColSpan = 16)]
    public string? Name { get; set; }

    [FieldEditor("主播语音", ValueListType = ValueDisplayType.TtsModel, AllowClear = true, Offset = 0, Span = 24,
        LabelColSpan = 6, WrapperColSpan = 16)]
    public long? PrimaryTtsModelId { get; set; }

    [FieldEditor("助播语音", ValueListType = ValueDisplayType.TtsModel, AllowClear = true, Offset = 0, Span = 24,
        LabelColSpan = 6, WrapperColSpan = 16)]
    public long? SecondaryTtsModelId { get; set; }

    [FieldEditor("生成话术模板", ValueListType = ValueDisplayType.MainScriptTemplate, AllowClear = true,
        PropJson = @"{""multiple"":true}",
        Offset = 0, Span = 24, LabelColSpan = 6, WrapperColSpan = 16)]
    public string? ScriptTemplateIds { get; set; }

    [FieldEditor("聊天回复模板", ValueListType = ValueDisplayType.ChatScriptTemplate, AllowClear = true,
        PropJson = @"{""multiple"":true}",
        Offset = 0,
        Span = 24, Rows = 3,
        LabelColSpan = 6, WrapperColSpan = 16)]
    public string? ChatTemplateIds { get; set; }

    [FieldEditor("互动回复模板", ValueListType = ValueDisplayType.InteractScriptTemplate, AllowClear = true,
        PropJson = @"{""multiple"":true}",
        Offset = 0,
        Span = 24, Rows = 3,
        LabelColSpan = 6, WrapperColSpan = 16)]
    public string? InteractTemplateIds { get; set; }
}