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
public class LiveAccountProductController : ControllerBase
{
    [HttpGet, HttpPost]
    public R<List<LiveAccountProductVm>> LiveAccountProductQueryList(PageQueryObject<LiveAccountProductQuery> query)
    {
        var user = Request.GetLoginUser();
        var list = QuerySvc.QueryList<LiveAccountProductQuery, LiveAccountProduct, LiveAccountProductVm>(query, user);
        return R.OK(list);
    }

    [HttpGet, HttpPost]
    public R<int> LiveAccountProductQueryCount(PageQueryObject<LiveAccountProductQuery> query)
    {
        var user = Request.GetLoginUser();
        var cnt = QuerySvc.QueryCount<LiveAccountProductQuery, LiveAccountProduct, LiveAccountProductVm>(query, user);
        return R.OK(cnt);
    }

    [HttpGet, HttpPost]
    public R<LiveAccountProductEditVm> LiveAccountProductGetEditVm(long id)
    {
        using var db = Db.Open();
        var user = Request.GetLoginUser();
        var model = db.Single<LiveAccountProduct>(a =>
            a.Id == id && a.UserId == user.UserId && a.TenantId == user.TenantId);
        if (model == null)
        {
            return R.Faild<LiveAccountProductEditVm>("未查找到数据");
        }

        return R.OK(model.Adapt<LiveAccountProductEditVm>());
    }

    [HttpGet, HttpPost]
    public R<LiveAccountProductEditVm> LiveAccountProductSaveEditVm(LiveAccountProductEditVm vm)
    {
        using var db = Db.Open();

        var model = vm.Adapt<LiveAccountProduct>();
        var user = Request.GetLoginUser();
        model.UserId = user.UserId;
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

        return R.OK(model.Adapt<LiveAccountProductEditVm>());
    }

    [HttpGet, HttpPost]
    public R LiveAccountProductDelete(string ids)
    {
        var idList = ids.SplitEx().Select(id => id.ToLong() ?? 0).ToList();
        using var db = Db.Open();
        var user = Request.GetLoginUser();
        db.Delete<LiveAccountProduct>(x =>
            idList.Contains(x.Id) && x.UserId == user.UserId && x.TenantId == user.TenantId);
        return R.OK();
    }
}