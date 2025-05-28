using SchemaBuilder.Svc.Core.QueryHelper.Data;
using SchemaBuilder.Svc.Models.Attr;
using SchemaBuilder.Svc.Models.Schema;
using SchemaBuilder.Svc.Models.Table;

namespace SchemaBuilder.Svc.Models.Query
{
    [SearchContainer(nameof(LiveScriptVm))]
    public class LiveScriptQuery : QueryDbEx<LiveScript, LiveScriptVm>, IUserIdQuery, ITenantIdQuery
    {
        [FieldEditor("脚本模板", ValueListType = ValueDisplayType.LiveScriptTemplate, WrapperColSpan = 16, Span = 4)]
        public long? LiveScriptTemplateId { get; set; }

        [FieldEditor("直播间", ValueListType = ValueDisplayType.LiveRoom, WrapperColSpan = 16, Span = 4)]
        public long? LiveRoomId { get; set; }

        [FieldEditor("嘉宾消息", WrapperColSpan = 16, Span = 4)]
        public StringQuery? GuestMessage { get; set; }

        [FieldEditor("AI回复", WrapperColSpan = 16, Span = 4)]
        public StringQuery? AssistantText { get; set; }

        public long? UserId { get; set; }
        public long? TenantId { get; set; }
    }
}

public class LiveScriptVoiceDetailQuery : QueryDbEx<LiveScriptVoiceDetail, LiveScriptVoiceDetailVm>
{
    public long HeaderId { get; set; }
}

[FullTable(nameof(LiveScriptVm), TableType.MainTable, QueryDataUrl = "/api/LiveScript/LiveScriptQueryList",
    QueryCountUrl = "/api/LiveScript/LiveScriptQueryCount", DeleteIdsUrl = "/api/LiveScript/LiveScriptDelete",
    PageSize = 100,
    MultiSelection = true)]
[TableTool(ToolType.Add, nameof(LiveScriptEditVm) + "_ModalObjectEditor", 10),
 TableTool(ToolType.Edit, nameof(LiveScriptEditVm) + "_ModalObjectEditor", 20),
 TableTool(ToolType.Delete, 0, 30),]
public class LiveScriptVm
{
    [TableColumn("Id")] public long Id { get; set; }

    [TableColumn("话术模板", ValueListType = ValueDisplayType.LiveScriptTemplate)]
    public long? LiveScriptTemplateId { get; set; }

    [TableColumn("直播间", ValueListType = ValueDisplayType.LiveRoom)]
    public long? LiveRoomId { get; set; }

    [TableColumn("留言", RenderType = CellRenderType.LongStringRender)]
    public string? GuestMessage { get; set; }

    [TableColumn("回复", RenderType = CellRenderType.LongStringRender)]
    public string? AssistantText { get; set; }
}

[ModalObjectEditor(nameof(LiveScriptEditVm) + "_ModalObjectEditor", "话术编辑", "/api/LiveScript/LiveScriptGetEditVm",
    "/api/LiveScript/LiveScriptSaveEditVm",
    SizeMode = 3)]
public class LiveScriptEditVm
{
    public long Id { get; set; }

    [FieldEditor("话术模板", ValueListType = ValueDisplayType.LiveScriptTemplate, Offset = 0, Span = 24, LabelColSpan = 4,
        WrapperColSpan = 20)]
    public long? LiveScriptTemplateId { get; set; }

    [FieldEditor("直播间", ValueListType = ValueDisplayType.LiveRoom, Offset = 0, Span = 24, LabelColSpan = 4,
        WrapperColSpan = 20)]
    public long? LiveRoomId { get; set; }

    [FieldEditor("留言", Offset = 0, Span = 24, LabelColSpan = 4, WrapperColSpan = 20)]
    public string? GuestMessage { get; set; }

    [FieldEditor("回复", Offset = 0, Span = 24, Rows = 5, LabelColSpan = 4, WrapperColSpan = 20)]
    public string? AssistantText { get; set; }
}

[FullTable(nameof(LiveScriptVm), TableType.DetailTable, QueryDataUrl = "/api/LiveScript/LiveScriptVoiceDetailQueryList",
    PageSize = 100, MultiSelection = true, Title = "分段语音", AutoQuery = false)]
public class LiveScriptVoiceDetailVm
{
    [TableColumn("Id")] public long Id { get; set; }
    public long HeaderId { get; set; }

    [TableColumn("子句", RenderType = CellRenderType.LongStringRender)]
    public string? Text { get; set; }
}

public class VoiceBuildRequestVm
{
    public string Content { get; set; }
    public long TtsModelId { get; set; }
}

public class VoiceBuildResponseVm
{
    public byte[] Voice { get; set; }
}