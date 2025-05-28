using SchemaBuilder.Svc.Core.QueryHelper.Data;
using SchemaBuilder.Svc.Models.Attr;
using SchemaBuilder.Svc.Models.Table;

namespace SchemaBuilder.Svc.Models.ViewModel;

[SearchContainer(nameof(ModelAuthInfoVm))]
public class ModelAuthInfoQuery : QueryDbEx<ModelAuthInfo, ModelAuthInfoVm>, ITenantIdQuery
{
    [FieldEditor("平台类型", WrapperColSpan = 16, Span = 4)]
    public ModelPlatformType? PlatformType { get; set; }

    [FieldEditor("名称", WrapperColSpan = 16, Span = 4)]
    public StringQuery? Name { get; set; }

    [FieldEditor("EndPoint", WrapperColSpan = 16, Span = 4)]
    public StringQuery? EndPoint { get; set; }

    [FieldEditor("ApiKey", WrapperColSpan = 16, Span = 4)]
    public StringQuery? ApiKey { get; set; }

    [FieldEditor("TextModelId", WrapperColSpan = 16, Span = 4)]
    public StringQuery? TextModelId { get; set; }

    [FieldEditor("ImageModelId", WrapperColSpan = 16, Span = 4)]
    public StringQuery? ImageModelId { get; set; }

    public long? TenantId { get; set; }
}

[FullTable(nameof(ModelAuthInfoVm), TableType.MainTable, QueryDataUrl = "/api/ModelAuthInfo/ModelAuthInfoQueryList",
    QueryCountUrl = "/api/ModelAuthInfo/ModelAuthInfoQueryCount",
    DeleteIdsUrl = "/api/ModelAuthInfo/ModelAuthInfoDelete",
    PageSize = 100,
    MultiSelection = true)]
[TableTool(ToolType.Add, nameof(ModelAuthInfoEditVm) + "_ModalObjectEditor", 10),
 TableTool(ToolType.Edit, nameof(ModelAuthInfoEditVm) + "_ModalObjectEditor", 20),
 TableTool(ToolType.Delete, 0, 30),]
public class ModelAuthInfoVm
{
    [TableColumn("Id")] public long Id { get; set; }
    [TableColumn("平台类型")] public ModelPlatformType PlatformType { get; set; }
    [TableColumn("名称")] public string? Name { get; set; }
    [TableColumn("端点")] public string? EndPoint { get; set; }
    [TableColumn("应用密钥")] public string? ApiKey { get; set; }
    [TableColumn("文字模型 ID")] public string? TextModelId { get; set; }
    [TableColumn("图片模型 ID")] public string? ImageModelId { get; set; }
}

[ModalObjectEditor(nameof(ModelAuthInfoEditVm) + "_ModalObjectEditor", "模型认证信息编辑",
    "/api/ModelAuthInfo/ModelAuthInfoGetEditVm",
    "/api/ModelAuthInfo/ModelAuthInfoSaveEditVm",
    SizeMode = 5)]
public class ModelAuthInfoEditVm
{
    public long Id { get; set; }

    [FieldEditor("平台类型", Offset = 0, Span = 24)]
    public ModelPlatformType PlatformType { get; set; }

    [FieldEditor("名称", Offset = 0, Span = 24)]
    public string? Name { get; set; }

    [FieldEditor("端点", Offset = 0, Span = 24)]
    public string? EndPoint { get; set; }

    [FieldEditor("应用密钥", Offset = 0, Span = 24)]
    public string? ApiKey { get; set; }

    [FieldEditor("文字模型 ID", Offset = 0, Span = 24)]
    public string? TextModelId { get; set; }

    [FieldEditor("图片模型 ID", Offset = 0, Span = 24)]
    public string? ImageModelId { get; set; }
}