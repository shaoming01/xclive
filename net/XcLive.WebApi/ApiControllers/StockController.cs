using Microsoft.AspNetCore.Mvc;
using SchemaBuilder.Svc.Core;
using SchemaBuilder.Svc.Core.QueryHelper.Data;
using SchemaBuilder.Svc.Models.Table;
using SchemaBuilder.Svc.Svc;

namespace SchemaBuilder.Web.ApiControllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class StockController : ControllerBase
{
    [HttpGet, HttpPost]
    public R<List<StockBillVm>> Query(PageQueryObject<StockBillQuery> query)
    {
        var re = StockQuerySvc.QueryList(query);
        return re;
    }

  
}