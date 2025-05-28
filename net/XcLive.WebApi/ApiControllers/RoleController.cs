using Mapster;
using Microsoft.AspNetCore.Mvc;
using SchemaBuilder.Svc.Core;
using SchemaBuilder.Svc.Core.Ext;
using SchemaBuilder.Svc.Core.QueryHelper.Data;
using SchemaBuilder.Svc.Helpers;
using SchemaBuilder.Svc.Models.Table;
using SchemaBuilder.Svc.Models.ViewModel;
using SchemaBuilder.Svc.Svc;
using ServiceStack.OrmLite;

namespace SchemaBuilder.Web.ApiControllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class RoleController : ControllerBase
{
    [HttpGet, HttpPost]
    public R<List<RoleVm>> RoleQueryList(PageQueryObject<RoleQuery> query)
    {
        var user = Request.GetLoginUser();
        var list = QuerySvc.QueryList<RoleQuery, Role, RoleVm>(query, user);
        return R.OK(list);
    }

    [HttpGet, HttpPost]
    public R<int> RoleQueryCount(PageQueryObject<RoleQuery> query)
    {
        var user = Request.GetLoginUser();
        var cnt = QuerySvc.QueryCount<RoleQuery, Role, RoleVm>(query, user);
        return R.OK(cnt);
    }

    [HttpGet, HttpPost]
    public R<RoleEditVm> RoleGetEditVm(long id)
    {
        using var db = Db.Open();
        var user = Request.GetLoginUser();
        var model = db.Single<Role>(a => a.Id == id && a.TenantId == user.TenantId);
        if (model == null)
        {
            return R.Faild<RoleEditVm>("未查找到数据");
        }

        return R.OK(model.Adapt<RoleEditVm>());
    }

    [HttpGet, HttpPost]
    public R<RoleEditVm> RoleSaveEditVm(RoleEditVm vm)
    {
        using var db = Db.Open();

        var model = vm.Adapt<Role>();
        var user = Request.GetLoginUser();
        model.TenantId = user.TenantId;
        if (model.Id == 0)
        {
            model.Id = Id.NewId();
            db.Insert(model);
        }
        else
        {
            db.Update(model);
        }

        return R.OK(model.Adapt<RoleEditVm>());
    }

    [HttpGet, HttpPost]
    public R RoleDelete(string ids)
    {
        var idList = ids.SplitEx().Select(id => id.ToLong() ?? 0).ToList();
        using var db = Db.Open();
        var user = HttpRequestExt.GetLoginUser(Request);
        db.Delete<Role>(x => idList.Contains(x.Id) && x.TenantId == user.TenantId);
        return R.OK();
    }
}