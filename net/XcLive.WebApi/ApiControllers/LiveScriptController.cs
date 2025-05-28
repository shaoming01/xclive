using Mapster;
using Microsoft.AspNetCore.Mvc;
using SchemaBuilder.Svc.Core;
using SchemaBuilder.Svc.Core.Ext;
using SchemaBuilder.Svc.Core.QueryHelper.Data;
using SchemaBuilder.Svc.Helpers;
using SchemaBuilder.Svc.Models.Query;
using SchemaBuilder.Svc.Models.Table;
using SchemaBuilder.Svc.Svc;
using ServiceStack.OrmLite;

namespace SchemaBuilder.Web.ApiControllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class LiveScriptController : ControllerBase
{
    [HttpGet, HttpPost]
    public R<List<LiveScriptVm>> LiveScriptQueryList(PageQueryObject<LiveScriptQuery> query)
    {
        var user = Request.GetLoginUser();
        var list = QuerySvc.QueryList<LiveScriptQuery, LiveScript, LiveScriptVm>(query, user);
        return R.OK(list);
    }

    [HttpGet, HttpPost]
    public R<List<LiveScriptVoiceDetailVm>> LiveScriptVoiceDetailQueryList(
        PageQueryObject<LiveScriptVoiceDetailQuery> query)
    {
        var user = Request.GetLoginUser();
        var list =
            QuerySvc.QueryList<LiveScriptVoiceDetailQuery, LiveScriptVoiceDetail, LiveScriptVoiceDetailVm>(query, user);
        return R.OK(list);
    }

    [HttpGet, HttpPost]
    public R<int> LiveScriptQueryCount(PageQueryObject<LiveScriptQuery> query)
    {
        var user = Request.GetLoginUser();
        var cnt = QuerySvc.QueryCount<LiveScriptQuery, LiveScript, LiveScriptVm>(query, user);
        return R.OK(cnt);
    }

    [HttpGet, HttpPost]
    public R<LiveScriptEditVm> LiveScriptGetEditVm(long id)
    {
        using var db = Db.Open();
        var user = Request.GetLoginUser();
        var model = db.Single<LiveScript>(a => a.Id == id && a.UserId == user.UserId && a.TenantId == user.TenantId);
        if (model == null)
        {
            return R.Faild<LiveScriptEditVm>("未查找到数据");
        }

        return R.OK(model.Adapt<LiveScriptEditVm>());
    }

    [HttpGet, HttpPost]
    public R<LiveScriptEditVm> LiveScriptSaveEditVm(LiveScriptEditVm vm)
    {
        using var db = Db.Open();

        var model = vm.Adapt<LiveScript>();
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

        return R.OK(model.Adapt<LiveScriptEditVm>());
    }

    [HttpGet, HttpPost]
    public R LiveScriptDelete(string ids)
    {
        var idList = ids.SplitEx().Select(id => id.ToLong() ?? 0).ToList();
        using var db = Db.Open();
        var user = Request.GetLoginUser();
        db.Delete<LiveScript>(x => idList.Contains(x.Id) && x.UserId == user.UserId && x.TenantId == user.TenantId);
        return R.OK();
    }


    [HttpGet, HttpPost]
    public R BuildScript(long id)
    {
        var user = Request.GetLoginUser();
        return LiveScriptSvc.BuildScript(id);
    }

    /// <summary>
    /// 生成直播话术
    /// </summary>
    /// <returns></returns>
    [HttpGet, HttpPost]
    public R<LiveScriptVoiceDetail[]> BuildScriptToLive()
    {
        var user = Request.GetLoginUser();
        return LiveScriptSvc.BuildLiveScript(user);
    }

    [HttpGet, HttpPost]
    public R<LiveScriptVoiceDetail[]> BuildAiVerticalAnchorScript(long aiVerticalAnchorId, string? chatText,
        string? interactText)
    {
        var user = Request.GetLoginUser();
        return LiveScriptSvc.BuildAiVerticalAnchorScript(aiVerticalAnchorId, chatText, interactText, user);
    }

    [HttpGet, HttpPost]
    public R<LiveScriptVoiceDetail[]> BuildScriptByTemplate(long templateId, string? inputText)
    {
        var user = Request.GetLoginUser();
        return LiveScriptSvc.BuildScriptByTemplate(templateId, inputText, user);
    }

    [HttpGet, HttpPost]
    public R<VoiceBuildResponseVm> BuildVoice(VoiceBuildRequestVm vm)
    {
        var user = Request.GetLoginUser();
        return LiveScriptSvc.BuildVoiceByVm(user, vm);
    }

    [HttpGet, HttpPost]
    public R<VoiceBuildResponseVm> ListenVoice(long modelId)
    {
        var user = Request.GetLoginUser();
        return TtsModelSvc.ListenVoice(modelId);
    }
}