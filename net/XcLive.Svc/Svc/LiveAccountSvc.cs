using Newtonsoft.Json;
using SchemaBuilder.Svc.Core;
using SchemaBuilder.Svc.Core.Ext;
using SchemaBuilder.Svc.Helpers;
using SchemaBuilder.Svc.Models.Dto;
using SchemaBuilder.Svc.Models.Table;
using SchemaBuilder.Svc.Models.ViewModel;
using ServiceStack.OrmLite;

namespace SchemaBuilder.Svc.Svc;

public static class LiveAccountSvc
{
    public static R SaveAccount(LiveAccountSaveDto dto, UserLoginInfo user)
    {
        if (!dto.AuthJson.IsJson())
        {
            return R.Faild("授权信息有误");
        }

        var name = dto.PlatformAccountName;
        var platAccountId = dto.PlatformAccountId;

        using var db = Db.Open();
        var extList = db.Select<LiveAccount>(a =>
            a.PlatformUserId == platAccountId && a.Platform == dto.Platform && a.RoleType == dto.RoleType);

        if (extList.Has())
        {
            var ids = extList.Select(l => l.Id).ToList();
            db.UpdateOnly(() => new LiveAccount()
            {
                Name = name, AuthJson = dto.AuthJson
            }, l => ids.Contains(l.Id));
        }

        if (!extList.Has(l => l.RoleType == dto.RoleType))
        {
            var newAccount = new LiveAccount()
            {
                Name = name,
                AuthJson = dto.AuthJson,
                PlatformUserId = platAccountId,
                Platform = dto.Platform,
                RoleType = dto.RoleType,
                Id = Id.NewId(),
                TenantId = user.TenantId,
                UserId = user.UserId,
            };
            db.Insert(newAccount);
        }

        return R.OK();
    }

    /*
    private static R<PlatformLiveAccount> AnalizeUserInfo(LivePlatform platform, AccountRoleType dtoRoleType,
        string html)
    {
        if (dtoRoleType == AccountRoleType.Observer)
        {
            return AnalizeUserInfoFromDyLive(platform, dtoRoleType, html);
        }

        return AnalizeUserInfoFromDyLive(platform, dtoRoleType, html);
    }
    */

    /*
    /// <summary>
    /// live.douyin.com页面分析
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    private static R<PlatformLiveAccount> AnalizeUserInfoFromDyLive(LivePlatform platform, AccountRoleType dtoRoleType,
        string html)
    {
        if (!html.IsJson())
        {
            return R.Faild<PlatformLiveAccount>("解析数据出错，不是JSON，可能是未登录");
        }

        try
        {
            JObject dict = JObject.Parse(html);
            var userId = dict.SelectToken("data.douyin_unique_id")?.ToString();
            var name = dict.SelectToken("nick_name")?.ToString();
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(name))
                return R.Faild<PlatformLiveAccount>("解析账户信息出错，内容为空");
            return R.OK(new PlatformLiveAccount
            {
                Name = name,
                PlatformUserId = userId,
            });
        }
        catch (Exception e)
        {
            Log4.Log.Error(e);
            return R.Faild<PlatformLiveAccount>("解析数据出错:" + e.Message);
        }
    }
    */

    /*
    public class PlatformLiveAccount
    {
        public string Name { get; set; }
        public string PlatformUserId { get; set; }
    }
    */

    /*
    private static bool CheckLoginStatus(LivePlatform dtoPlatform, AccountRoleType dtoRoleType, CookieDto[] dtoCookies)
    {
        if (dtoRoleType == AccountRoleType.Observer)
        {
            return dtoCookies.Has(c => c.Name == "sessionid" && c.Value.Has());
        }

        return dtoCookies.Has(c => c.Name == "sessionid" && c.Value.Has());
    }
*/
}