using System.Collections;
using SchemaBuilder.Svc.Core.Ext;
using SchemaBuilder.Svc.Core.QueryHelper.Data;

namespace SchemaBuilder.Svc.Core.QueryHelper;

/// <summary>
/// 分页查询默认查询条件生成
/// </summary>
public static class PageQueryMatchingWhereUtil
{
    /// <summary>
    /// 得到查询项
    /// </summary>
    public static IEnumerable<string> GetQueryMatchingWheres(IEnumerable<QueryElementItem> noneQueryMatchingItems)
    {
        //剔除复杂对象【不给复杂对象生成查询条件】
        return noneQueryMatchingItems.Where(x => !x.IsComplexObject).Select(w => ToWhereSql(w.Name, w));
    }


    /// <summary>
    /// 将 QueryElementItem 转换成查询条件
    /// </summary>
    /// <param name="field">查询字段</param>
    /// <param name="item">查询项</param>
    /// <returns>查询SQL</returns>
    public static string ToWhereSql(string field, QueryElementItem item)
    {
        switch (item.Type)
        {
            case ConditionType.Not:
                return string.Format("{0} <> {1}", field, ToValueByType(item.Value));
            case ConditionType.Ge:
                return string.Format("{0} >= {1}", field, ToValueByType(item.Value));
            case ConditionType.Gt:
                return string.Format("{0} > {1}", field, ToValueByType(item.Value));
            case ConditionType.Le:
                return string.Format("{0} <= {1}", field, ToValueByType(item.Value));
            case ConditionType.Lt:
                return string.Format("{0} < {1}", field, ToValueByType(item.Value));
            case ConditionType.In:
                return string.Format("{0} IN ({1})", field, ToInSql(item.Value));
            case ConditionType.Like:
                return string.Format("{0} LIKE {1}", field, ToLikeValue((item.Value ?? string.Empty).ToString()));
            case ConditionType.NotIn:
                return string.Format("{0} NOT IN ({1})", field, ToInSql(item.Value));
            case ConditionType.NotLike:
                return string.Format("{0} NOT LIKE {1}", field, ToLikeValue((item.Value ?? string.Empty).ToString()));
            case ConditionType.IsNullOrEmpty:
                return string.Format("ISNULL({0},'') = ''", field);
            case ConditionType.NotNullOrEmpty:
                return string.Format("ISNULL({0},'') <> ''", field);
            default:
                return string.Format("{0} = {1}", field, ToValueByType(item.Value));
        }
    }

    private static string ToLikeValue(string value)
    {
        return string.Format("'%{0}%'",
            value.Replace("'", "''")
                .Replace("[", "[[]")
                .Replace("%", "[%]")
                .Replace("_", "[_]"));
    }

    /// <summary>
    /// 转换成IN SQL
    /// </summary>
    private static string ToInSql(object value)
    {
        var enumerator = value as IEnumerable;
        if (enumerator == null) return string.Empty;
        return string.Join(",", enumerator.Cast<object>().Select(ToValueByType));
    }


    /// <summary>
    /// 根据类型转换成SQL识别的数据格式
    /// </summary>
    /// <param name="value">数据</param>
    /// <returns>SQL识别的数据格式</returns>
    public static string ToValueByType(object value)
    {
        if (value == null) return string.Empty;
        var type = value.GetType();
        //判断是否为Bool
        if (value is bool)
        {
            return ((bool)value) ? "1" : "0";
        }

        //判断是否为枚举
        if (type.IsEnum)
        {
            return ((int)value).ToString();
        }

        //判断是否为日期
        if (value is DateTime)
        {
            return ((DateTime)value).ToSqlPar();
        }

        //统统按照字符串处理
        return value.ToString().ToSqlPar();
    }
}