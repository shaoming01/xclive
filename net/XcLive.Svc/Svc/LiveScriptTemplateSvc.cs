using SchemaBuilder.Svc.Helpers;
using SchemaBuilder.Svc.Models.Table;

namespace SchemaBuilder.Svc.Svc;

public static class LiveScriptTemplateSvc
{
    private const string PersonaDescription = "{{个性描述}}";
    private const string ProductDescription = "{{直播间描述}}";
    private const string GuestMessage = "{{互动消息}}";

    public static string? BuildText(UsageType usage, bool isSystem, string templateText, string personaDescription,
        string productDescription, string guestMessage)
    {
        if (!templateText.Has())
        {
            return "";
        }

        var text = ReplaceTemplate(templateText, personaDescription, productDescription, guestMessage);
        if ((usage == UsageType.Chat || usage == UsageType.InteractScriptTemplate) && isSystem)
        {
            text += "\\n回复的内容要放在<answer>标签里，如果内容有多人对话，以[主播][助播]开头表示这是谁的台词。";
        }
        else if (usage == UsageType.LiveScript && !isSystem)
        {
            text += "\\n回复的内容要放在<answer>标签里，如果内容有多人对话，以[主播][助播]开头表示这是谁的台词。";
        }

        return text;
    }

    private static string ReplaceTemplate(string templateSystemTemplate, string personaDescription,
        string productDescription, string guestMessage)
    {
        var text = templateSystemTemplate;
        text = text.Replace(PersonaDescription, personaDescription);
        text = text.Replace(ProductDescription, productDescription);
        text = text.Replace(GuestMessage, guestMessage);
        return text;
    }
}