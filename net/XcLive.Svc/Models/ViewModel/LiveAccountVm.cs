using SchemaBuilder.Svc.Core.QueryHelper.Data;
using SchemaBuilder.Svc.Models.Attr;
using SchemaBuilder.Svc.Models.Table;
using ServiceStack.Redis.Support;

namespace SchemaBuilder.Svc.Models.ViewModel;

[SearchContainer(nameof(LiveAccountVm))]
public class LiveAccountQuery : QueryDbEx<LiveAccount, LiveAccountVm>, IUserIdQuery, ITenantIdQuery
{
    [FieldEditor("平台", WrapperColSpan = 16, Span = 4)]
    public LivePlatform? Platform { get; set; }

    [FieldEditor("角色类型", WrapperColSpan = 16, Span = 4)]
    public AccountRoleType? RoleType { get; set; }

    [FieldEditor("名称", WrapperColSpan = 16, Span = 4)]
    public StringQuery? Name { get; set; }

    public long? UserId { get; set; }
    public long? TenantId { get; set; }
}

[FullTable(nameof(LiveAccountVm), TableType.MainTable,
    QueryDataUrl = "/api/LiveAccount/LiveAccountQueryList",
    QueryCountUrl = "/api/LiveAccount/LiveAccountQueryCount",
    DeleteIdsUrl = "/api/LiveAccount/LiveAccountDelete",
    PageSize = 100,
    MultiSelection = true)]
[TableTool(ToolType.Add, nameof(LiveAccountEditVm) + "_ModalObjectEditor", 10),
 TableTool(ToolType.Edit, nameof(LiveAccountEditVm) + "_ModalObjectEditor", 20),
 TableTool(ToolType.Delete, 0, 30)]
public class LiveAccountVm
{
    [TableColumn("Id")] public long Id { get; set; }
    [TableColumn("平台")] public LivePlatform Platform { get; set; }
    [TableColumn("角色类型")] public AccountRoleType RoleType { get; set; }
    [TableColumn("名称")] public string? Name { get; set; }
    [TableColumn("平台用户Id")] public string? PlatformUserId { get; set; }
}

[ModalObjectEditor(nameof(LiveAccountEditVm) + "_ModalObjectEditor", "直播账号编辑",
    "/api/LiveAccount/LiveAccountGetEditVm", "/api/LiveAccount/LiveAccountSaveEditVm", SizeMode = 5)]
public class LiveAccountEditVm
{
    public long Id { get; set; }

    [FieldEditor("平台", Offset = 0, Span = 24)]
    public LivePlatform Platform { get; set; }

    [FieldEditor("角色类型", Offset = 0, Span = 24)]
    public AccountRoleType RoleType { get; set; }

    [FieldEditor("名称", Offset = 0, Span = 24)]
    public string? Name { get; set; }

    [FieldEditor("平台用户Id", Offset = 0, Span = 24)]
    public string? PlatformUserId { get; set; }

    [FieldEditor("AuthJson", Offset = 0, Span = 24)]
    public string? AuthJson { get; set; }

    public long TenantId { get; set; }
    public long UserId { get; set; }
}

public class LiveAccountSaveDto
{
    public long AccountId { get; set; }
    public string PlatformAccountId { get; set; }
    public string PlatformAccountName { get; set; }
    public LivePlatform Platform { get; set; }
    public AccountRoleType RoleType { get; set; }

    public string? AuthJson { get; set; }
}

public class CookieDto
{
    public string Name { get; set; }
    public string Value { get; set; }
    public string Domain { get; set; }
}

public class DyAccountAuthVm
{
    public string douyin_unique_id { get; set; }
    public string nick_name { get; set; }
    public string douyin_uid { get; set; }
    public string company_name { get; set; }
    public string cookie { get; set; }
}

public class ByAccountAuthVm
{
    public string buyin_account_id { get; set; }
    public string user_name { get; set; }
    public string user_id { get; set; }
    public string origin_uid { get; set; }
    public string shop_id { get; set; }
    public string cookie { get; set; }
}

public class ByUrlGetReqVm
{
    public string baseUrl { get; set; }
    public string s_v_web_id { get; set; }
    public OrderedDictionary<string, string>? queryParams { get; set; }
    public string? postJson { get; set; }
    public string userAgent { get; set; }

}