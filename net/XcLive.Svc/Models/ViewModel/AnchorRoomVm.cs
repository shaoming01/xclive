using SchemaBuilder.Svc.Core.QueryHelper.Data;
using SchemaBuilder.Svc.Models.Attr;
using SchemaBuilder.Svc.Models.Table;

namespace SchemaBuilder.Svc.Models.ViewModel;

[SearchContainer(nameof(LiveRoomVm))]
public class LiveRoomQuery : QueryDbEx<LiveRoom, LiveRoomVm>, IUserIdQuery, ITenantIdQuery
{
    [FieldEditor("名称", WrapperColSpan = 16, Span = 4)]
    public StringQuery? Name { get; set; }

    [FieldEditor("直播间描述", WrapperColSpan = 16, Span = 4)]
    public StringQuery? ProductText { get; set; }

    [FieldEditor("个性描述", WrapperColSpan = 16, Span = 4)]
    public StringQuery? PersonaText { get; set; }

    public long? UserId { get; set; }
    public long? TenantId { get; set; }
}

[FullTable(nameof(LiveRoomVm), TableType.MainTable, QueryDataUrl = "/api/LiveRoom/LiveRoomQueryList",
    QueryCountUrl = "/api/LiveRoom/LiveRoomQueryCount", DeleteIdsUrl = "/api/LiveRoom/LiveRoomDelete",
    PageSize = 100,
    MultiSelection = true)]
[TableTool(ToolType.Add, nameof(LiveRoomEditVm) + "_ModalObjectEditor", 10),
 TableTool(ToolType.Edit, nameof(LiveRoomEditVm) + "_ModalObjectEditor", 20),
 TableTool(ToolType.Delete, 0, 30)]
public class LiveRoomVm
{
    [TableColumn("Id")] public long Id { get; set; }
    [TableColumn("房间名称")] public string? Name { get; set; }
    [TableColumn("直播间描述")] public string? ProductText { get; set; }
    [TableColumn("商品描述")] public string? PromotionText { get; set; }
    [TableColumn("个性描述")] public string? PersonaText { get; set; }
}

[ModalObjectEditor(nameof(LiveRoomEditVm) + "_ModalObjectEditor", "直播房间编辑", "/api/LiveRoom/LiveRoomGetEditVm",
    "/api/LiveRoom/LiveRoomSaveEditVm", SizeMode = 5)]
public class LiveRoomEditVm
{
    public long Id { get; set; }

    [FieldEditor("房间名称", Offset = 0, Span = 24, LabelColSpan = 3, WrapperColSpan = 21)]
    public string? Name { get; set; }

    [FieldEditor("直播间描述", Offset = 0, Span = 24, Rows = 5, LabelColSpan = 3, WrapperColSpan = 21)]
    public string? ProductText { get; set; }

    [FieldEditor("商品描述", Offset = 0, Span = 24, Rows = 5, LabelColSpan = 3, WrapperColSpan = 21)]
    public string? PromotionText { get; set; }

    [FieldEditor("个性描述", Offset = 0, Span = 24, Rows = 5, LabelColSpan = 3, WrapperColSpan = 21)]
    public string? PersonaText { get; set; }
}