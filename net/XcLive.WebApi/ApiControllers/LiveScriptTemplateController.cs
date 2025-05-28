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
public class LiveScriptTemplateController : ControllerBase
{
    [HttpGet, HttpPost]
    public R<List<LiveScriptTemplateVm>> LiveScriptTemplateQueryList(PageQueryObject<LiveScriptTemplateQuery> query)
    {
        var user = HttpRequestExt.GetLoginUser(Request);
        var list = QuerySvc.QueryList<LiveScriptTemplateQuery, LiveScriptTemplate, LiveScriptTemplateVm>(query, user);
        return R.OK(list);
    }

    [HttpGet, HttpPost]
    public R<int> LiveScriptTemplateQueryCount(PageQueryObject<LiveScriptTemplateQuery> query)
    {
        var user = HttpRequestExt.GetLoginUser(Request);
        var cnt = QuerySvc.QueryCount<LiveScriptTemplateQuery, LiveScriptTemplate, LiveScriptTemplateVm>(query, user);
        return R.OK(cnt);
    }

    [HttpGet, HttpPost]
    public R<LiveScriptTemplateEditVm> LiveScriptTemplateGetEditVm(long id)
    {
        using var db = Db.Open();
        var user = HttpRequestExt.GetLoginUser(Request);
        var model = db.Single<LiveScriptTemplate>(a =>
            a.Id == id && a.TenantId == user.TenantId && a.UserId == user.UserId);
        if (model == null)
        {
            return R.Faild<LiveScriptTemplateEditVm>("未查找到数据");
        }

        return R.OK(model.Adapt<LiveScriptTemplateEditVm>());
    }

    [HttpGet, HttpPost]
    public R<LiveScriptTemplateEditVm> LiveScriptTemplateSaveEditVm(LiveScriptTemplateEditVm vm)
    {
        using var db = Db.Open();

        var model = vm.Adapt<LiveScriptTemplate>();
        var user = HttpRequestExt.GetLoginUser(Request);
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

        return R.OK(model.Adapt<LiveScriptTemplateEditVm>());
    }

    [HttpGet, HttpPost]
    public R LiveScriptTemplateDelete(string ids)
    {
        var idList = ids.SplitEx().Select(id => id.ToLong() ?? 0).ToList();
        using var db = Db.Open();
        var user = HttpRequestExt.GetLoginUser(Request);
        db.Delete<LiveScriptTemplate>(x =>
            idList.Contains(x.Id) && x.TenantId == user.TenantId && x.UserId == user.UserId);
        return R.OK();
    }
}