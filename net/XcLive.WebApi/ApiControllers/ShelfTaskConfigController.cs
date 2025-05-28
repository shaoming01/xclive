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
public class ShelfTaskConfigController : ControllerBase
{
    [HttpGet, HttpPost]
    public R<List<ShelfTaskConfigVm>> ShelfTaskConfigQueryList(PageQueryObject<ShelfTaskConfigQuery> query)
    {
        var user = Request.GetLoginUser();
        var list = QuerySvc.QueryList<ShelfTaskConfigQuery, ShelfTaskConfig, ShelfTaskConfigVm>(query, user);
        return R.OK(list);
    }

    [HttpGet, HttpPost]
    public R<int> ShelfTaskConfigQueryCount(PageQueryObject<ShelfTaskConfigQuery> query)
    {
        var user = Request.GetLoginUser();
        var cnt = QuerySvc.QueryCount<ShelfTaskConfigQuery, ShelfTaskConfig, ShelfTaskConfigVm>(query, user);
        return R.OK(cnt);
    }

    [HttpGet, HttpPost]
    public R<ShelfTaskConfigEditVm> ShelfTaskConfigGetEditVm(long id)
    {
        using var db = Db.Open();
        var user = Request.GetLoginUser();
        var model = db.Single<ShelfTaskConfig>(a => a.Id == id && a.TenantId == user.TenantId);
        if (model == null)
            return R.Faild<ShelfTaskConfigEditVm>("未查找到数据");
        return R.OK(model.Adapt<ShelfTaskConfigEditVm>());
    }

    [HttpGet, HttpPost]
    public R<ShelfTaskConfigEditVm> ShelfTaskConfigSaveEditVm(ShelfTaskConfigEditVm vm)
    {
        using var db = Db.Open();
        var model = vm.Adapt<ShelfTaskConfig>();
        var user = Request.GetLoginUser();
        model.TenantId = user.TenantId;
        model.UserId = user.UserId;

        if (model.Id == 0)
        {
            model.Id = Id.NewId();
            db.Insert(model);
        }
        else
        {
            db.Update(model);
        }

        return R.OK(model.Adapt<ShelfTaskConfigEditVm>());
    }

    [HttpGet, HttpPost]
    public R ShelfTaskConfigDelete(string ids)
    {
        var idList = ids.SplitEx().Select(id => id.ToLong() ?? 0).ToList();
        using var db = Db.Open();
        var user = HttpRequestExt.GetLoginUser(Request);
        db.Delete<ShelfTaskConfig>(x => idList.Contains(x.Id) && x.TenantId == user.TenantId);
        return R.OK();
    }
}