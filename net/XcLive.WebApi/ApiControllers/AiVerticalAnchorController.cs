using Mapster;
using Microsoft.AspNetCore.Mvc;
using SchemaBuilder.Svc.Core;
using SchemaBuilder.Svc.Core.Ext;
using SchemaBuilder.Svc.Core.QueryHelper.Data;
using SchemaBuilder.Svc.Helpers;
using SchemaBuilder.Svc.Models.Table;
using SchemaBuilder.Svc.Models.ViewModel;
using SchemaBuilder.Svc.Svc;
using SchemaBuilder.Web.ApiControllers;
using ServiceStack.OrmLite;

[ApiController]
[Route("api/[controller]/[action]")]
public class AiVerticalAnchorController : ControllerBase
{
    [HttpGet, HttpPost]
    public R<List<AiVerticalAnchorVm>> AiVerticalAnchorQueryList(PageQueryObject<AiVerticalAnchorQuery> query)
    {
        var user = Request.GetLoginUser();
        var list = QuerySvc.QueryList<AiVerticalAnchorQuery, AiVerticalAnchor, AiVerticalAnchorVm>(query, user);
        return R.OK(list);
    }

    [HttpGet, HttpPost]
    public R<int> AiVerticalAnchorQueryCount(PageQueryObject<AiVerticalAnchorQuery> query)
    {
        var user = Request.GetLoginUser();
        var cnt = QuerySvc.QueryCount<AiVerticalAnchorQuery, AiVerticalAnchor, AiVerticalAnchorVm>(query, user);
        return R.OK(cnt);
    }

    [HttpGet, HttpPost]
    public R<AiVerticalAnchorEditVm> AiVerticalAnchorGetEditVm(long id)
    {
        using var db = Db.Open();
        var user = Request.GetLoginUser();
        var model = db.Single<AiVerticalAnchor>(a =>
            a.Id == id && a.TenantId == user.TenantId && a.UserId == user.UserId);
        if (model == null)
            return R.Faild<AiVerticalAnchorEditVm>("未查找到数据");
        return R.OK(model.Adapt<AiVerticalAnchorEditVm>());
    }

    [HttpGet, HttpPost]
    public R<AiVerticalAnchorEditVm> AiVerticalAnchorSaveEditVm(AiVerticalAnchorEditVm vm)
    {
        using var db = Db.Open();
        var model = vm.Adapt<AiVerticalAnchor>();
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

        return R.OK(model.Adapt<AiVerticalAnchorEditVm>());
    }

    [HttpGet, HttpPost]
    public R AiVerticalAnchorDelete(string ids)
    {
        var idList = ids.SplitEx().Select(id => id.ToLong() ?? 0).ToList();
        using var db = Db.Open();
        var user = Request.GetLoginUser();
        db.Delete<AiVerticalAnchor>(x =>
            idList.Contains(x.Id) && x.TenantId == user.TenantId && x.TenantId == user.UserId);
        return R.OK();
    }
}