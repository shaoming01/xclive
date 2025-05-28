using SchemaBuilder.Svc.Core.Ext;
using SchemaBuilder.Svc.Helpers;
using SchemaBuilder.Svc.Models.Dto;
using SchemaBuilder.Svc.Svc;

namespace SchemaBuilder.Web.ApiControllers;

public static class HttpRequestExt
{
    public static UserLoginInfo GetLoginUser(this HttpRequest req)
    {
        req.Cookies.TryGetValue("token", out var token);
        if (token.IsNullOrWhiteSpace())
        {
            req.Query.TryGetValue("token", out var urlToken);
            if (!urlToken.Has() || urlToken.First().IsNullOrEmpty())
            {
                throw new Exception("用户未登录");
            }

            token = urlToken.First();
        }

        return UserSvc.GetLoginUser(token);
    }
}