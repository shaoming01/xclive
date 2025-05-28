using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SchemaBuilder.Svc.Core;
using SchemaBuilder.Svc.Core.Cache;
using SchemaBuilder.Svc.Core.Ext;
using SchemaBuilder.Svc.Helpers;
using SchemaBuilder.Svc.Models.Dto;
using SchemaBuilder.Web.ApiControllers;

namespace SchemaBuilder.Web.Helpers;

public class ApiAuthorizationFilter : IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // 检查是否标记了 AllowAnonymousAttribute
        var allowAnonymous = context.ActionDescriptor.EndpointMetadata
            .Any(em => em is AllowAnonymousAttribute);

        if (allowAnonymous)
        {
            // 如果有 AllowAnonymousAttribute，则跳过授权检查
            return;
        }

        try
        {
            context.HttpContext.Request.GetLoginUser();
        }
        catch (Exception e)
        {
            context.Result = new JsonResult(new R
            {
                Code = 401,
                Message = e.Message,
            });
        }
    }
}