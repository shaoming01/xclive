using SchemaBuilder.Svc.Core.QueryHelper.Data;
using SchemaBuilder.Svc.Models.Attr;
using SchemaBuilder.Svc.Models.Schema;
using SchemaBuilder.Svc.Models.Table;

namespace SchemaBuilder.Svc.Models.ViewModel;

[SearchContainer(nameof(LiveAccountProductVm))]
public class LiveAccountProductQuery : QueryDbEx<LiveAccountProduct, LiveAccountProductVm>, IUserIdQuery, ITenantIdQuery
{
    [FieldEditor("平台", WrapperColSpan = 16, Span = 4)]
    public LivePlatform? Platform { get; set; }

    [FieldEditor("平台账户", ValueListType = ValueDisplayType.LiveHuangCheOperate, WrapperColSpan = 16, Span = 4)]
    public long LiveAccountId { get; set; }

    [FieldEditor("商品名称", WrapperColSpan = 16, Span = 4)]
    public StringQuery? ProductName { get; set; }

    [FieldEditor("商品Id", WrapperColSpan = 16, Span = 4)]
    public StringQuery? ProductId { get; set; }

    public long? UserId { get; set; }
    public long? TenantId { get; set; }
}

[FullTable(nameof(LiveAccountProductVm), TableType.MainTable,
    QueryDataUrl = "/api/LiveAccountProduct/LiveAccountProductQueryList",
    QueryCountUrl = "/api/LiveAccountProduct/LiveAccountProductQueryCount",
    DeleteIdsUrl = "/api/LiveAccountProduct/LiveAccountProductDelete",
    PageSize = 100,
    MultiSelection = true)]
[TableTool(ToolType.Add, nameof(LiveAccountProductEditVm) + "_ModalObjectEditor", 10),
 TableTool(ToolType.Edit, nameof(LiveAccountProductEditVm) + "_ModalObjectEditor", 20),
 TableTool(ToolType.Delete, 0, 30)]
public class LiveAccountProductVm
{
    [TableColumn("Id")] public long Id { get; set; }
    [TableColumn("平台")] public LivePlatform Platform { get; set; }

    [TableColumn("平台账户", ValueListType = ValueDisplayType.LiveHuangCheOperate)]
    public long LiveAccountId { get; set; }

    [TableColumn("商品名称")] public string? ProductName { get; set; }
    [TableColumn("商品Id")] public string? ProductId { get; set; }

    [TableColumn("图片", RenderType = CellRenderType.IconRender)]
    public string? ImgUrl { get; set; }

    public string? ProductJson { get; set; }
    public string? AnalyzeResult { get; set; }
}

[ModalObjectEditor(nameof(LiveAccountProductEditVm) + "_ModalObjectEditor", "直播账号编辑",
    "/api/LiveAccountProduct/LiveAccountProductGetEditVm", "/api/LiveAccountProduct/LiveAccountProductSaveEditVm",
    SizeMode = 5)]
public class LiveAccountProductEditVm
{
    public long Id { get; set; }

    [FieldEditor("平台", Offset = 0, Span = 24)]
    public LivePlatform Platform { get; set; }

    [FieldEditor("平台账户", ValueListType = ValueDisplayType.LiveHuangCheOperate, Offset = 0, Span = 24)]
    public long LiveAccountId { get; set; }

    [FieldEditor("商品名称", Offset = 0, Span = 24)]
    public string? ProductName { get; set; }

    [FieldEditor("商品Id", Offset = 0, Span = 24)]
    public string? ProductId { get; set; }

    [FieldEditor("ProductJson", Offset = 0, Span = 24)]
    public string? ProductJson { get; set; }
    public string? AnalyzeResult { get; set; }

    public string? ImgUrl { get; set; }

    public long TenantId { get; set; }
    public long UserId { get; set; }
}