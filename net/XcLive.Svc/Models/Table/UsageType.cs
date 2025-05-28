using System.ComponentModel;

namespace SchemaBuilder.Svc.Models.Table;

public enum UsageType
{
    [Description("未定义")] None = 0,
    [Description("话术生成")] LiveScript = 1,
    [Description("聊天回复")] Chat = 2,
    [Description("互动回复")] InteractScriptTemplate = 3,
}