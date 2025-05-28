using Microsoft.AspNetCore.Mvc;
using SchemaBuilder.Svc.Core;
using SchemaBuilder.Svc.Models.ViewModel;
using SchemaBuilder.Svc.Svc;

namespace SchemaBuilder.Web.ApiControllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class SignController : ControllerBase
{
    [HttpGet, HttpPost]
    public R<string> GetByReqUrl(ByUrlGetReqVm vm)
    {
        return SignSvc.GetByReqUrl(vm);
    }

    [HttpGet, HttpPost]
    public R<string> GetWssUrl(string roomId, string uniqueId)
    {
        return SignSvc.GetWssUrl(roomId, uniqueId);
    }
}