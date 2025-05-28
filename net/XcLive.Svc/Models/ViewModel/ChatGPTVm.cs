using SchemaBuilder.Svc.Core.QueryHelper.Data;
using SchemaBuilder.Svc.Models.Attr;
using SchemaBuilder.Svc.Models.Table;
using ServiceStack;

namespace SchemaBuilder.Svc.Models.ViewModel;

[SearchContainer(nameof(ChatGptSessionVm))]
public class ChatGptSessionQuery : QueryDbEx<ChatGptSession, ChatGptSessionVm>
{
    [FieldEditor("创建时间", LabelColSpan = 6, WrapperColSpan = 18, Span = 6)]
    public DateQuery? Created { get; set; }
}

public class ChatGptSessionVm
{
    public long Id { get; set; }
    public string? Title { get; set; }
    public DateTime Created => Svc.Id.GetDate(Id);
}

public class ChatGptSessionMessageQuery : QueryDbEx<ChatGptSessionMessage, ChatGptSessionMessageVm>
{
    public long HeaderId { get; set; }
}

public class ChatGptSessionMessageVm
{
    public long Id { get; set; }
    public long HeaderId { get; set; }
    public string Role { get; set; }
    public string Message { get; set; }
}

public class NewMessageVm
{
    public long SessionId { get; set; }
    public string Message { get; set; }
}