using Mapster;
using Microsoft.AspNetCore.Mvc;
using SchemaBuilder.Svc.Core;
using SchemaBuilder.Svc.Core.QueryHelper.Data;
using SchemaBuilder.Svc.Models.Table;
using SchemaBuilder.Svc.Models.ViewModel;
using SchemaBuilder.Svc.Svc;
using ServiceStack.OrmLite;

namespace SchemaBuilder.Web.ApiControllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class ChatGptController : ControllerBase
{
    [HttpGet, HttpPost]
    public R<List<ChatGptSessionVm>> QueryList(PageQueryObject<ChatGptSessionQuery> query)
    {
        var user = Request.GetLoginUser();
        var list = QuerySvc.QueryList<ChatGptSessionQuery, ChatGptSession, ChatGptSessionVm>(query, user);
        return R.OK(list);
    }

    [HttpGet, HttpPost]
    public R<ChatGptSessionMessage> SendMessage(NewMessageVm msgVm)
    {
        var re = ChatGptSvc.SendMessage(msgVm);
        return re;
    }

    [HttpGet, HttpPost]
    public R<ChatGptSessionVm> CreateNewSession(NewMessageVm msgVm)
    {
        using var db = Db.Open();
        var extSession = db.Single<ChatGptSession>(s => s.Title == msgVm.Message);
        if (extSession != null)
        {
            return R.OK(extSession.Adapt<ChatGptSessionVm>());
        }

        var session = new ChatGptSession()
        {
            Id = Id.NewId(),
            Title = msgVm.Message,
        };
        db.Save(session);
        var vm = session.Adapt<ChatGptSessionVm>();
        return R.OK(vm);
    }

    [HttpGet, HttpPost]
    public R<List<ChatGptSessionMessageVm>> LoadSession(PageQueryObject<ChatGptSessionMessageQuery> query)
    {
        var user = Request.GetLoginUser();
        var list = QuerySvc
            .QueryList<ChatGptSessionMessageQuery, ChatGptSessionMessage, ChatGptSessionMessageVm>(query, user);
        return R.OK(list);
    }

    [HttpGet, HttpPost]
    public R DeleteSession(long id)
    {
        using var db = Db.Open();
        db.Delete<ChatGptSessionMessage>(m => m.HeaderId == id);
        db.Delete<ChatGptSession>(s => s.Id == id);
        return R.OK();
    }
}