namespace SchemaBuilder.Svc.Core.QueryHelper.Data;

/// <summary>
/// Web查询统计字段特性
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class StatisticsFieldAttribute : Attribute
{
    /// <summary>
    /// 实例化统计特性
    /// </summary>
    /// <param name="field">字段</param>
    /// <param name="type">类型</param>
    public StatisticsFieldAttribute(string field, StatisticsType type = StatisticsType.None)
    {
        Field = field;
        Type = type;
    }

    /// <summary>
    /// 统计函数
    /// </summary>
    public StatisticsType Type { get; set; }

    /// <summary>
    /// 字段
    /// </summary>
    public string Field { get; set; }
}