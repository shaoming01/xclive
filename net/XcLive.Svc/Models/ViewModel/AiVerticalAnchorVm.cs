using SchemaBuilder.Svc.Core.QueryHelper.Data;
using SchemaBuilder.Svc.Models.Attr;
using SchemaBuilder.Svc.Models.Schema;
using SchemaBuilder.Svc.Models.Table;

namespace SchemaBuilder.Svc.Models.ViewModel;

[SearchContainer(nameof(AiVerticalAnchorVm))]
public class AiVerticalAnchorQuery : QueryDbEx<AiVerticalAnchor, AiVerticalAnchorVm>, ITenantIdQuery, IUserIdQuery
{
    [FieldEditor("行业主播名称", WrapperColSpan = 16, Span = 4)]
    public StringQuery? Name { get; set; }

    [FieldEditor("主播音色模型", ValueListType = ValueDisplayType.TtsModel, WrapperColSpan = 16, Span = 4)]
    public long? PrimaryTtsModelId { get; set; }

    [FieldEditor("助播音色模型", ValueListType = ValueDisplayType.TtsModel, WrapperColSpan = 16, Span = 4)]
    public long? SecondaryTtsModelId { get; set; }

    public long? TenantId { get; set; }
    public long? UserId { get; set; }
}

// 列表视图
[FullTable(nameof(AiVerticalAnchorVm), TableType.MainTable,
    QueryDataUrl = "/api/AiVerticalAnchor/AiVerticalAnchorQueryList",
    QueryCountUrl = "/api/AiVerticalAnchor/AiVerticalAnchorQueryCount",
    DeleteIdsUrl = "/api/AiVerticalAnchor/AiVerticalAnchorDelete",
    PageSize = 100, MultiSelection = true)]
[TableTool(ToolType.Add, nameof(AiVerticalAnchorEditVm) + "_ModalObjectEditor", 10),
 TableTool(ToolType.Edit, nameof(AiVerticalAnchorEditVm) + "_ModalObjectEditor", 20),
 TableTool(ToolType.Delete, 0, 30)]
public class AiVerticalAnchorVm
{
    [TableColumn("Id")] public long Id { get; set; }

    [TableColumn("名称")] public string? Name { get; set; }

    [TableColumn("主播模型", ValueListType = ValueDisplayType.TtsModel)]
    public long? PrimaryTtsModelId { get; set; }

    [TableColumn("助播模型", ValueListType = ValueDisplayType.TtsModel)]
    public long? SecondaryTtsModelId { get; set; }

    [TableColumn("生成模板", ValueListType = ValueDisplayType.MainScriptTemplate)]
    public string? ScriptTemplateIds { get; set; }

    [TableColumn("聊天模板", ValueListType = ValueDisplayType.ChatScriptTemplate)]
    public string? ChatTemplateIds { get; set; }

    [TableColumn("互动模板", ValueListType = ValueDisplayType.InteractScriptTemplate)]
    public string? InteractTemplateIds { get; set; }
}

// 编辑视图
[ModalObjectEditor(nameof(AiVerticalAnchorEditVm) + "_ModalObjectEditor", "行业主播编辑",
    "/api/AiVerticalAnchor/AiVerticalAnchorGetEditVm",
    "/api/AiVerticalAnchor/AiVerticalAnchorSaveEditVm", SizeMode = 3)]
public class AiVerticalAnchorEditVm
{
    public long Id { get; set; }

    [FieldEditor("名称", Offset = 0, Span = 24, LabelColSpan = 4, WrapperColSpan = 20)]
    public string? Name { get; set; }

    [FieldEditor("主播语音", ValueListType = ValueDisplayType.TtsModel, Offset = 0, Span = 24,
        LabelColSpan = 4, WrapperColSpan = 20)]
    public long? PrimaryTtsModelId { get; set; }

    [FieldEditor("助播语音", ValueListType = ValueDisplayType.TtsModel, Offset = 0, Span = 24,
        LabelColSpan = 4, WrapperColSpan = 20)]
    public long? SecondaryTtsModelId { get; set; }

    [FieldEditor("生成话术模板", ValueListType = ValueDisplayType.MainScriptTemplate, PropJson = @"{""multiple"":true}",
        Offset = 0, Span = 24, LabelColSpan = 4,
        WrapperColSpan = 20)]
    public string? ScriptTemplateIds { get; set; }

    [FieldEditor("聊天回复模板", ValueListType = ValueDisplayType.ChatScriptTemplate, PropJson = @"{""multiple"":true}",
        Offset = 0,
        Span = 24, Rows = 3,
        LabelColSpan = 4, WrapperColSpan = 20)]
    public string? ChatTemplateIds { get; set; }

    [FieldEditor("互动回复模板", ValueListType = ValueDisplayType.InteractScriptTemplate, PropJson = @"{""multiple"":true}",
        Offset = 0,
        Span = 24, Rows = 3,
        LabelColSpan = 4, WrapperColSpan = 20)]
    public string? InteractTemplateIds { get; set; }
}