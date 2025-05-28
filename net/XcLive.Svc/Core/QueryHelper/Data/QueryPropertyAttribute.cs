namespace SchemaBuilder.Svc.Core.QueryHelper.Data;

/// <summary>
/// 查询属性
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class QueryPropertyAttribute : Attribute
{
    public QueryPropertyAttribute()
    {
    }

    /// <summary>
    /// 实例化查询对象
    /// </summary>
    /// <param name="type">查询类型</param>
    public QueryPropertyAttribute(ConditionType type)
        : this(null, type)
    {
    }

    /// <summary>
    /// 实例化查询对象
    /// </summary>
    /// <param name="name">查询项名称【如果为null就默认字段名称】</param>
    /// <param name="type">查询类型</param>
    public QueryPropertyAttribute(string name, ConditionType type = ConditionType.Eq)
    {
        Name = name;
        Type = type;
    }

    /// <summary>
    /// 查询项名称，如果没有就默认字段名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 是否为复杂对象，如果是复杂对象是不会自动生成查询条件的，只供SqlBuilder进行重写复杂查询
    /// </summary>
    public bool IsComplexObject { get; set; }

    /// <summary>
    /// 查询Type
    /// </summary>
    public ConditionType Type { get; set; }
}