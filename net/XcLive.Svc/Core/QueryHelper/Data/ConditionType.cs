using System.ComponentModel;

namespace SchemaBuilder.Svc.Core.QueryHelper.Data
{
    [Flags]
    public enum ConditionType
    {
        [Description("等于")] Eq = 1,
        [Description("不等于")] Not = 2,
        [Description("大于或等于")] Ge = 4,
        [Description("大于")] Gt = 8,
        [Description("小于等于")] Le = 16,
        [Description("小于")] Lt = 32,
        [Description("包含")] In = 64,
        [Description("相似")] Like = 128,
        [Description("区间")] Range = 256,
        [Description("不包含")] NotIn = 512,
        [Description("不相似")] NotLike = 1024,
        [Description("为空")] IsNullOrEmpty = 2048,
        [Description("不为空")] NotNullOrEmpty = 4096,
    }
}