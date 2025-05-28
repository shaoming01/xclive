namespace SchemaBuilder.Svc.Core.QueryHelper.Data;

/// <summary>
/// 查询元素项
/// </summary>
public class QueryElementItem
{
    /// <summary>
    /// 查询项类型
    /// </summary>
    public ConditionType Type { get; set; }

    /// <summary>
    /// 是否为复杂对象，如果是复杂对象是不会自动生成查询条件的，只供SqlBuilder进行重写复杂查询
    /// </summary>
    public bool IsComplexObject { get; set; }

    /// <summary>
    /// 查询值
    /// </summary>
    public object Value { get; set; }

    /// <summary>
    /// 查找项名称
    /// </summary>
    public string Name { get; set; }

    public override string ToString()
    {
        return string.Format("Name = {0},Type = {1},Value = {2}", Name, Type, Value);
    }
}