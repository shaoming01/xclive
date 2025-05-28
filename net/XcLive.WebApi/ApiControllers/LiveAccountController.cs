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
public class LiveAccountController : ControllerBase
{
    [HttpGet, HttpPost]
    public R<List<LiveAccountVm>> LiveAccountQueryList(PageQueryObject<LiveAccountQuery> query)
    {
        var user = Request.GetLoginUser();
        var list = QuerySvc.QueryList<LiveAccountQuery, LiveAccount, LiveAccountVm>(query, user);
        return R.OK(list);
    }

    [HttpGet, HttpPost]
    public R<int> LiveAccountQueryCount(PageQueryObject<LiveAccountQuery> query)
    {
        var user = Request.GetLoginUser();
        var cnt = QuerySvc.QueryCount<LiveAccountQuery, LiveAccount, LiveAccountVm>(query, user);
        return R.OK(cnt);
    }

    [HttpGet, HttpPost]
    public R<LiveAccountEditVm> LiveAccountGetEditVm(long id)
    {
        using var db = Db.Open();
        var user = Request.GetLoginUser();
        var model = db.Single<LiveAccount>(a => a.Id == id && a.UserId == user.UserId && a.TenantId == user.TenantId);
        if (model == null)
        {
            return R.Faild<LiveAccountEditVm>("未查找到数据");
        }

        return R.OK(model.Adapt<LiveAccountEditVm>());
    }

    [HttpGet, HttpPost]
    public R<LiveAccountEditVm> LiveAccountSaveEditVm(LiveAccountEditVm vm)
    {
        using var db = Db.Open();

        var model = vm.Adapt<LiveAccount>();
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

        return R.OK(model.Adapt<LiveAccountEditVm>());
    }

    [HttpGet, HttpPost]
    public R LiveAccountDelete(string ids)
    {
        var idList = ids.SplitEx().Select(id => id.ToLong() ?? 0).ToList();
        using var db = Db.Open();
        var user = Request.GetLoginUser();
        db.Delete<LiveAccount>(x => idList.Contains(x.Id) && x.UserId == user.UserId && x.TenantId == user.TenantId);
        return R.OK();
    }

    [HttpGet, HttpPost]
    public R SaveLiveAccount(LiveAccountSaveDto dto)
    {
        var user = Request.GetLoginUser();
        return LiveAccountSvc.SaveAccount(dto, user);
    }
}