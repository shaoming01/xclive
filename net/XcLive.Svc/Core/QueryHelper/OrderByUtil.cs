using SchemaBuilder.Svc.Core.Ext;

namespace SchemaBuilder.Svc.Core.QueryHelper;

public class OrderByUtil
{
    /// <summary>
    /// 解析生成ORDER BY 内容
    /// </summary>
    /// <param name="orderBy">待解析的OrderBy内容</param>
    /// <param name="defaultOrderBy">默认的OrderBy内容</param>
    /// <param name="mapObj">解析的对象【值就直接字段名称如果是多个字段就 字段一,字段二】</param>
    /// <param name="onlyField">只是转换成字段不加入ORDER BY</param>
    /// <param name="onlyMatching">只进行匹配映射,如果映射未匹配就返回默认排序字段</param>
    /// <returns></returns>
    public static string OrderBy(string orderBy, string defaultOrderBy, object mapObj, bool onlyField = false,
        bool onlyMatching = true)
    {
        return OrderBy(orderBy, defaultOrderBy,
            mapObj.GetType().GetProperties().ToDictionary(k => k.Name, v => v.GetValue(mapObj, null).ToString()),
            onlyField, onlyMatching);
    }

    /// <summary>
    /// 解析生成ORDER BY 内容
    /// </summary>
    /// <param name="orderBy">待解析的OrderBy内容</param>
    /// <param name="defaultOrderBy">默认的OrderBy内容</param>
    /// <param name="orderMaps">解析的集合</param>
    /// <param name="onlyField">只是转换成字段不加入ORDER BY</param>
    /// <param name="onlyMatching">只进行匹配映射,如果映射未匹配就返回默认排序字段</param>
    /// <returns></returns>
    public static string OrderBy(string orderBy, string defaultOrderBy, Dictionary<string, string> orderMaps,
        bool onlyField = false, bool onlyMatching = true)
    {
        if (orderBy.IsNullOrEmpty()) return defaultOrderBy;
        var field = GetField(orderBy).ToLower();
        var isDesc = GetIsDesc(orderBy);
        var orderByField = orderMaps.FirstOrDefault(f => f.Key.ToLower() == field).Value;
        if (orderByField.IsNullOrEmpty())
        {
            //未找到匹配就直接返回默认字段
            if (onlyMatching)
                return defaultOrderBy;
            orderByField = field; //赋值为当前查询字段
        }

        if (onlyField) return GetOrderByField(orderByField, isDesc);
        return string.Format("ORDER BY {0}", GetOrderByField(orderByField, isDesc));
    }

    private static string GetOrderByField(string orderByField, bool isDesc)
    {
        return string.Join(",",
            orderByField.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => string.Format("{0} {1}", s, isDesc ? "DESC" : string.Empty)));
    }

    private static bool GetIsDesc(string orderBy)
    {
        if (orderBy.IsNullOrEmpty()) return false;
        var cells = orderBy.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
        if (cells.Length < 2) return false;
        return cells[1].ToLower() == "desc";
    }

    private static string GetField(string orderBy)
    {
        if (orderBy.IsNullOrEmpty()) return orderBy;
        return orderBy.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)[0];
    }
}