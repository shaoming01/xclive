using System.ComponentModel;
using SchemaBuilder.Svc.Core.Aq;

namespace SchemaBuilder.Svc.Core.QueryHelper.Data;

public struct StringQuery : IQueryField
{
    /// <summary>
    /// 字符串查询类型
    /// </summary>
    public StringQueryType Type { get; set; }

    /// <summary>
    /// 查询的字符串
    /// </summary>
    public string Value { get; set; }

    public override string ToString()
    {
        return GetSqlExpression("{ColName}");
    }

    public string GetSqlExpression(string colName, string provider = "")
    {
        switch (Type)
        {
            case StringQueryType.IsNullOrEmpty:
                return string.Format("ISNULL({0},'')=''", colName);
            case StringQueryType.NotNullOrEmpty:
                return string.Format("ISNULL({0},'')<>''", colName);
            case StringQueryType.Contains:
                return string.Format("{0} like '%{1}%'", colName, Value);
            case StringQueryType.NotContains:
                return string.Format("{0} not like '%{1}%'", colName, Value);
            case StringQueryType.Not:
                return string.Format("{0} <> '{1}'", colName, Value);
            default:
                return string.Format("{0}='{1}'", colName, Value);
        }
    }

    /// <summary>
    /// 是否没有查询元素
    /// </summary>
    public bool IsEmpty()
    {
        return Type == StringQueryType.None;
    }
}

/// <summary>
/// 查询类型
/// </summary>
[Flags]
public enum StringQueryType
{
    /// <summary>
    /// 默认状态，表示没有查询条件
    /// </summary>
    [Description("")] None = 0,
    [Description("等于")] Eq = 1,
    [Description("不等于")] Not = 2,
    [Description("包含")] Contains = 3,
    [Description("不包含")] NotContains = 5,
    [Description("无内容")] IsNullOrEmpty = 7,
    [Description("有内容")] NotNullOrEmpty = 8,
}