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
public class LiveRoomController : ControllerBase
{
    [HttpGet, HttpPost]
    public R<List<LiveRoomVm>> LiveRoomQueryList(PageQueryObject<LiveRoomQuery> query)
    {
        var user = Request.GetLoginUser();
        var list = QuerySvc.QueryList<LiveRoomQuery, LiveRoom, LiveRoomVm>(query, user);
        return R.OK(list);
    }

    [HttpGet, HttpPost]
    public R<int> LiveRoomQueryCount(PageQueryObject<LiveRoomQuery> query)
    {
        var user = Request.GetLoginUser();
        var cnt = QuerySvc.QueryCount<LiveRoomQuery, LiveRoom, LiveRoomVm>(query, user);
        return R.OK(cnt);
    }

    [HttpGet, HttpPost]
    public R<LiveRoomEditVm> LiveRoomGetEditVm(long id)
    {
        using var db = Db.Open();
        var user = Request.GetLoginUser();
        var model = db.Single<LiveRoom>(a => a.Id == id && a.UserId == user.UserId && a.TenantId == user.TenantId);
        if (model == null)
        {
            return R.Faild<LiveRoomEditVm>("未查找到数据");
        }

        return R.OK(model.Adapt<LiveRoomEditVm>());
    }

    [HttpGet, HttpPost]
    public R<LiveRoomEditVm> LiveRoomSaveEditVm(LiveRoomEditVm vm)
    {
        using var db = Db.Open();

        var model = vm.Adapt<LiveRoom>();
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

        return R.OK(model.Adapt<LiveRoomEditVm>());
    }

    [HttpGet, HttpPost]
    public R LiveRoomDelete(string ids)
    {
        var idList = ids.SplitEx().Select(id => id.ToLong() ?? 0).ToList();
        using var db = Db.Open();
        var user = Request.GetLoginUser();
        db.Delete<LiveRoom>(x => idList.Contains(x.Id) && x.UserId == user.UserId && x.TenantId == user.TenantId);
        return R.OK();
    }

    [HttpGet, HttpPost]
    public R SaveDefault(LiveRoomEditVm vm)
    {
        using var db = Db.Open();

        var user = Request.GetLoginUser();
        var room = db.Single<LiveRoom>(r => r.UserId == user.UserId && r.TenantId == user.TenantId);
        if (room == null)
        {
            room = new LiveRoom()
            {
                UserId = user.UserId,
                TenantId = user.TenantId,
                ProductText = vm.ProductText,
                PersonaText = vm.PersonaText,
                Id = Id.NewId(),
                Name = vm.Name,
            };
            db.Insert(room);
        }
        else
        {
            room.Name = "默认";
            room.ProductText = vm.ProductText;
            room.PersonaText = vm.PersonaText;
            db.Update(room);
        }

        return R.OK();
    }
}