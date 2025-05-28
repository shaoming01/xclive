using SchemaBuilder.Svc.Helpers;

namespace SchemaBuilder.Svc.Core.QueryHelper.Data;

public class QueryMatchingItem<T, TP> : QueryMatchingItem<T>
{
    /// <summary>
    /// 得到属性委托
    /// </summary>
    public Func<T, TP> GetProperty { get; set; }

    /// <summary>
    /// 得到查询SQL委托
    /// </summary>
    public Func<Parameter, string> GetWhereSql { get; set; }

    /// <summary>
    /// 得到查询条件SQL
    /// </summary>
    /// <returns>查询条件</returns>
    public override string ToWhereSql(T obj)
    {
        var queryPropertyAttribute =
            typeof(T).GetProperty(Name).ReadAttributes<QueryPropertyAttribute>().FirstOrDefault();
        return GetWhereSql(new Parameter
        {
            Name = Name,
            Value = GetProperty(obj),
            Attribute = queryPropertyAttribute
        });
    }

    /// <summary>
    /// 参数
    /// </summary>
    public class Parameter
    {
        /// <summary>
        /// 属性名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 查询属性值
        /// </summary>
        public TP Value { get; set; }

        /// <summary>
        /// 查询字段特性
        /// </summary>
        public QueryPropertyAttribute Attribute { get; set; }
    }
}

public abstract class QueryMatchingItem<T>
{
    /// <summary>
    /// 查询属性字段名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 得到查询条件SQL
    /// </summary>
    /// <returns>查询条件</returns>
    public abstract string ToWhereSql(T obj);
}