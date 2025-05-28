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
public class MenuController : ControllerBase
{
    [HttpGet, HttpPost]
    public R<List<MenuVm>> MenuQueryList(PageQueryObject<MenuQuery> query)
    {
        var user = HttpRequestExt.GetLoginUser(Request);
        var list = QuerySvc.QueryList<MenuQuery, Menu, MenuVm>(query, user);
        return R.OK(list);
    }

    [HttpGet, HttpPost]
    public R<int> MenuQueryCount(PageQueryObject<MenuQuery> query)
    {
        var user = HttpRequestExt.GetLoginUser(Request);
        var cnt = QuerySvc.QueryCount<MenuQuery, Menu, MenuVm>(query, user);
        return R.OK(cnt);
    }

    [HttpGet, HttpPost]
    public R<MenuEditVm> MenuGetEditVm(long id)
    {
        using var db = Db.Open();
        var user = HttpRequestExt.GetLoginUser(Request);
        var model = db.Single<Menu>(a => a.Id == id && a.TenantId == user.TenantId);
        if (model == null)
        {
            return R.Faild<MenuEditVm>("未查找到数据");
        }

        return R.OK(model.Adapt<MenuEditVm>());
    }

    [HttpGet, HttpPost]
    public R<MenuEditVm> MenuSaveEditVm(MenuEditVm vm)
    {
        using var db = Db.Open();

        var model = vm.Adapt<Menu>();
        var user = HttpRequestExt.GetLoginUser(Request);
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

        return R.OK(model.Adapt<MenuEditVm>());
    }

    [HttpGet, HttpPost]
    public R MenuDelete(string ids)
    {
        var idList = ids.SplitEx().Select(id => id.ToLong() ?? 0).ToList();
        using var db = Db.Open();
        var user = HttpRequestExt.GetLoginUser(Request);
        db.Delete<Menu>(x => idList.Contains(x.Id) && x.TenantId == user.TenantId);
        return R.OK();
    }
}