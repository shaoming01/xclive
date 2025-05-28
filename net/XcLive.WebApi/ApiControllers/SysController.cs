using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SchemaBuilder.Svc.Core;
using SchemaBuilder.Svc.Models.Schema;
using SchemaBuilder.Svc.Models.Table;
using SchemaBuilder.Svc.Svc;
using SchemaBuilder.Svc.Svc.SchemaBuild;

namespace SchemaBuilder.Web.ApiControllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class SysController : ControllerBase
{
    [HttpGet, HttpPost]
    public R<List<ValueDisplay>> ListValueDisplay(ValueDisplayQuery query)
    {
        var user = HttpRequestExt.GetLoginUser(Request);
        var list = SysSvc.ListValuerDisplay(query, user, true);
        return R.OK(list);
    }

    [HttpGet, HttpPost]
    public R<object> GetSysModuleSchema(string moduleName)
    {
        var re = SysModuleBuilder.GetSchema(moduleName);
        if (re == null)
        {
            return R.Faild<object>("找不到此系统模块" + moduleName);
        }

        return R.OK(re);
    }

    [HttpGet, HttpPost]
    public R<object> GetSetting(string typeName)
    {
        var user = HttpRequestExt.GetLoginUser(Request);
        var re = SettingSvc.GetSetting(user.TenantId, user.UserId, typeName);
        return R.OK(re.Data as object);
    }

    [HttpGet, HttpPost]
    public R SaveSetting(string typeName, JsonElement json)
    {
        var user = HttpRequestExt.GetLoginUser(Request);
        var type = typeof(ISettingObject).Assembly.GetTypes().FirstOrDefault(t => t.Name == typeName);
        if (type == null)
        {
            return R.Faild($"未找到类型{typeName}");
        }

        var jsonStr = json.ToString();

        var re = SettingSvc.SaveSetting(user.TenantId, user.UserId, typeName, jsonStr);
        return R.OK();
    }

    [HttpGet, HttpPost]
    public R<ClientPackage> GetNewVersion(long tenantId)
    {
        return R.OK(new ClientPackage
        {
            Version = new Version("0.0.2504.0"),
            Url = "https://files.erp12345.com/live/星晨直播助手-0.0.2504.zip"
        });
    }
}