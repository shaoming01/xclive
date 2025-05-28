using Microsoft.AspNetCore.Mvc;
using SchemaBuilder.Svc.Core;
using SchemaBuilder.Svc.Core.Ext;
using SchemaBuilder.Svc.Core.QueryHelper.Data;
using SchemaBuilder.Svc.Models.Table;
using SchemaBuilder.Svc.Models.ViewModel;
using SchemaBuilder.Svc.Svc;
using ServiceStack.OrmLite;

namespace SchemaBuilder.Web.ApiControllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class ModuleController : ControllerBase
{
    [HttpGet, HttpPost]
    public R<List<ModuleVm>> QueryList(PageQueryObject<ModuleQuery> query)
    {
        var user = HttpRequestExt.GetLoginUser(Request);
        var list = QuerySvc.QueryList<ModuleQuery, Module, ModuleVm>(query, user);
        return R.OK(list);
    }

    [HttpGet, HttpPost]
    public R<int> QueryCount(PageQueryObject<ModuleQuery> query)
    {
        var user = HttpRequestExt.GetLoginUser(Request);
        var cnt = QuerySvc.QueryCount<ModuleQuery, Module, ModuleVm>(query, user);
        return R.OK(cnt);
    }


    [HttpGet, HttpPost]
    public R<ModuleVm> Save(ModuleVm module)
    {
        var re = ModuleSvc.Save(module);
        return R.OK(re);
    }

    [HttpGet, HttpPost]
    public R DeleteIds(string ids)
    {
        var idArr = ids.SplitEx();
        using var db = Db.Open();
        db.Delete<Module>(r => idArr.Contains(r.Id.ToString()));
        return R.OK();
    }

    [HttpGet, HttpPost]
    public R<ModuleVm> GetModule(long? moduleId, string? sysModuleId)
    {
        var vm = moduleId > 0 ? ModuleSvc.GetId(moduleId.Value) : ModuleSvc.GetSysModuleId(sysModuleId);
        if (vm == null)
        {
            return R.Faild<ModuleVm>("不存在该模块Id");
        }

        return R.OK(vm);
    }
}