using SchemaBuilder.Svc.Core.Ext;
using SchemaBuilder.Svc.Core.QueryHelper.Data;

namespace SchemaBuilder.Svc.Core.QueryHelper;

public class PageSqlBuilderItem
{
    private readonly Dictionary<ConditionType, QueryElementItem> _dictionary =
        new Dictionary<ConditionType, QueryElementItem>();

    /// <summary>
    /// SQL构建对象
    /// </summary>
    public PageSqlBuilder Builder { get; set; }

    public object this[ConditionType type]
    {
        get { return (_dictionary.TryGetOrDefault(type) ?? new QueryElementItem()).Value; }
    }

    public void Add(ConditionType type, QueryElementItem item)
    {
        _dictionary.Add(type, item);
    }

    /// <summary>
    /// 得到值
    /// </summary>
    public string Value
    {
        get { return string.Join(",", _dictionary.OrderBy(b => b.Key).Select(s => s.Value.Value)); }
    }

    public override string ToString()
    {
        return Value;
    }

    /// <summary>
    /// 转换成数据库值
    /// </summary>
    public string ToSqlPar(string field)
    {
        return string.Join(" AND ", _dictionary.Values.Select(x => PageQueryMatchingWhereUtil.ToWhereSql(field, x)));
    }

    #region 得到指定类型

    /// <summary>
    /// 大于或等于
    /// </summary>
    public object Ge
    {
        get { return this[ConditionType.Ge]; }
    }

    /// <summary>
    /// 小于
    /// </summary>
    public object Lt
    {
        get { return this[ConditionType.Lt]; }
    }

    #endregion

    /// <summary>
    /// 设置隐式转换成字符串
    /// </summary>
    /// <param name="item">对象</param>
    /// <returns>转换成的字符串</returns>
    public static implicit operator string(PageSqlBuilderItem item)
    {
        return item.Value;
    }
}