using SchemaBuilder.Svc.Core;
using SchemaBuilder.Svc.Core.Ext;
using SchemaBuilder.Svc.Helpers;
using SchemaBuilder.Svc.Models.Table;
using SchemaBuilder.Svc.Models.ViewModel;
using ServiceStack.OrmLite;

namespace SchemaBuilder.Svc.Svc;

public static class ChatGptSvc
{
    public static R<ChatGptSessionMessage> SendMessage(NewMessageVm msgVm)
    {
        if (msgVm.SessionId == 0)
        {
            return R.Faild<ChatGptSessionMessage>("Id is required");
        }

        var history = LoadHistoryMessage(msgVm.SessionId);
        if (!history.Has())
        {
            history.Add(GetSystemMessage());
        }

        history.Add(new OpenAIHelper.Message()
        {
            role = "user",
            content = msgVm.Message,
        });
        var model = new OpenAIHelper.ReqModel
        {
            model = "gpt-4", messages = history.ToArray(),
        };
        var respRe = OpenAIHelper.GetChatGptResponse(model);
        if (!respRe.Success)
        {
            return R.Faild<ChatGptSessionMessage>(respRe.Message);
        }

        var msg = respRe.Data.choices.FirstOrDefault()?.message;
        if (msg == null || msg.content.IsNullOrEmpty())
        {
            return R.Faild<ChatGptSessionMessage>("返回内容为空");
        }

        using var db = Db.Open();


        var msgModel = new ChatGptSessionMessage
        {
            Id = Id.NewId(),
            Message = msgVm.Message,
            Role = "user",
            HeaderId = msgVm.SessionId,
        };
        var assistantModel = new ChatGptSessionMessage
        {
            Id = Id.NewId(),
            Message = msg.content,
            Role = msg.role,
            HeaderId = msgVm.SessionId,
        };
        db.Save(msgModel);
        db.Save(assistantModel);
        return R.OK(assistantModel);
    }

    private static List<OpenAIHelper.Message> LoadHistoryMessage(long id)
    {
        using var db = Db.Open();
        var list = db.Select<ChatGptSessionMessage>(m => m.HeaderId == id);
        return list.Select(l => new OpenAIHelper.Message
        {
            role = l.Role,
            content = l.Message
        }).ToList();
    }

    public static OpenAIHelper.Message GetSystemMessage()
    {
        return new OpenAIHelper.Message
        {
            role = "system",
            content = "你是一个开发助手，帮助开发者根据规则生成代码。",
        };
    }
}