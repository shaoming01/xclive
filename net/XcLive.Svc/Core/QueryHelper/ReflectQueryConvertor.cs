using System.Collections;
using System.Reflection;
using SchemaBuilder.Svc.Core.QueryHelper.Data;
using SchemaBuilder.Svc.Core.QueryHelper.Data.@interface;
using SchemaBuilder.Svc.Helpers;

namespace SchemaBuilder.Svc.Core.QueryHelper;

/// <summary>
/// 基于反射的查询转换
/// </summary>
public class ReflectQueryConvertor : IQueryConvertor
{
    /// <summary>
    /// 将分页查询对象转换分页查询数据
    /// </summary>
    /// <typeparam name="T">查询对象</typeparam>
    /// <param name="queryObject">分页查询对象</param>
    /// <returns>分页查询数据</returns>
    public PageQueryData Convertor<T>(PageQueryObject<T>? queryObject)
    {
        return new PageQueryData
        {
            Page = queryObject?.Page ?? 1,
            PageSize = queryObject?.PageSize ?? 20,
            OrderBy = queryObject?.OrderBy ?? "",
            QueryElementItems = Convertor(queryObject.QueryObject)
        };
    }

    /// <summary>
    /// 将查询对象转换成查询项数组
    /// </summary>
    /// <param name="obj">需要转换的查询对象实例</param>
    /// <param name="names">需要进行转换的公共属性如果为null代表全部转换</param>
    /// <returns>转换完成的查询项</returns>
    public QueryElementItem[] Convertor<T>(T obj, HashSet<string> names = null)
    {
        if (obj == null) return new QueryElementItem[0];
        //得到公共属性
        var properties = typeof(T).GetProperties();
        var items = new List<QueryElementItem>(Math.Max(properties.Length, 4));

        //循环根据属性得到转换对象
        properties.ForEach(propertyInfo =>
        {
            //判断是否支持读
            if (!propertyInfo.CanRead) return;
            //判断是否需要转换
            if (names != null && !names.Contains(propertyInfo.Name)) return;
            var propertyAttribute = propertyInfo.ReadAttributes<QueryPropertyAttribute>().FirstOrDefault();
            //没有属性就返回
            if (propertyAttribute == null) return;

            items.AddRange(ConvertQueryElementItem(propertyAttribute, propertyInfo, obj));
        });
        //返回转换对象
        return items.ToArray();
    }

    /// <summary>
    /// 转换一个属性为查询对象
    /// </summary>
    /// <param name="propertyAttribute">查询转换特性</param>
    /// <param name="propertyInfo">属性对象</param>
    /// <param name="obj">实例</param>
    /// <returns>查询</returns>
    private IEnumerable<QueryElementItem> ConvertQueryElementItem(QueryPropertyAttribute propertyAttribute,
        PropertyInfo propertyInfo, object obj)
    {
        var value = propertyInfo.GetValue(obj, null);
        var name = propertyAttribute.Name ?? propertyInfo.Name;
        var propertyType = propertyInfo.PropertyType;
        if (value == null) return new QueryElementItem[0];
        //判断是否为复杂对象
        if (propertyAttribute.IsComplexObject)
        {
            return new[]
            {
                new QueryElementItem
                {
                    IsComplexObject = true,
                    Name = name,
                    Type = ConditionType.Eq,
                    Value = value
                }
            };
        }

        //判断是否为QueryString或者String类型
        if (value is StringQuery)
        {
            var queryString = (StringQuery)value;
            if (queryString.IsEmpty()) return new QueryElementItem[0];
            return ToQueryElementItem(name, queryString,
                propertyAttribute.Type == ConditionType.Like);
        }

        //判断是否为QueryLikeString或者String类型
        if (value is StringQuery)
        {
            var queryString = (StringQuery)value;
            if (queryString.IsEmpty()) return new QueryElementItem[0];
            return ToQueryElementItem(name, queryString);
        }

        //判断是否为QueryDateRange
        if (value is QueryDateRange)
        {
            return ToQueryElementItem(name, (QueryDateRange)value);
        }

        //判断是否为 QueryRange<T>
        if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(QueryRange<>))
        {
            var begin = propertyType.GetProperty("Begin").GetValue(value, null);
            var end = propertyType.GetProperty("End").GetValue(value, null);
            return ToQueryElementItem(name, begin, end);
        }

        //判断是否实现自IEnumerable<T>
        if (value is IEnumerable)
        {
            return ToQueryElementItem(name, (IEnumerable)value, propertyAttribute.Type);
        }

        //其他查询对象都是Eq
        return new[]
        {
            new QueryElementItem
            {
                Name = name,
                Type = ConditionType.Eq,
                Value = value
            }
        };
    }

    /// <summary>
    /// 转换 QueryString 为 QueryElementItem
    /// </summary>
    /// <param name="name">查询字段名称</param>
    /// <param name="stringQuery">查询字符串</param>
    /// <param name="like"></param>
    /// <returns>查询项</returns>
    private IEnumerable<QueryElementItem> ToQueryElementItem(string name, StringQuery stringQuery, bool like)
    {
        switch (stringQuery.Type)
        {
            case StringQueryType.Not:
                return new[]
                {
                    new QueryElementItem
                    {
                        Name = name,
                        Type = like ? ConditionType.NotLike : ConditionType.Not,
                        Value = stringQuery.Value
                    }
                };
            case StringQueryType.IsNullOrEmpty:
                return new[]
                {
                    new QueryElementItem
                    {
                        Name = name,
                        Type = ConditionType.IsNullOrEmpty
                    }
                };
            case StringQueryType.NotNullOrEmpty:
                return new[]
                {
                    new QueryElementItem
                    {
                        Name = name,
                        Type = ConditionType.NotNullOrEmpty
                    }
                };
            default:
                return new[]
                {
                    new QueryElementItem
                    {
                        Name = name,
                        Type = like ? ConditionType.Like : ConditionType.Eq,
                        Value = stringQuery.Value
                    }
                };
        }
    }

    /// <summary>
    /// 转换 QueryString 为 QueryElementItem
    /// </summary>
    /// <param name="name">查询字段名称</param>
    /// <param name="stringQuery">查询字符串</param>
    /// <returns>查询项</returns>
    private IEnumerable<QueryElementItem> ToQueryElementItem(string name, StringQuery stringQuery)
    {
        switch (stringQuery.Type)
        {
            case StringQueryType.Not:
                return new[]
                {
                    new QueryElementItem
                    {
                        Name = name,
                        Type = ConditionType.NotLike,
                        Value = stringQuery.Value
                    }
                };
            case StringQueryType.IsNullOrEmpty:
                return new[]
                {
                    new QueryElementItem
                    {
                        Name = name,
                        Type = ConditionType.IsNullOrEmpty
                    }
                };
            case StringQueryType.NotNullOrEmpty:
                return new[]
                {
                    new QueryElementItem
                    {
                        Name = name,
                        Type = ConditionType.NotNullOrEmpty
                    }
                };
            default:
                return new[]
                {
                    new QueryElementItem
                    {
                        Name = name,
                        Type = ConditionType.Like,
                        Value = stringQuery.Value
                    }
                };
        }
    }

    /// <summary>
    /// 转换 QueryDateRange 为 QueryElementItem
    /// </summary>
    /// <param name="name">查询字段名称</param>
    /// <param name="queryDateRange">查询日期范围</param>
    /// <returns>查询项</returns>
    private IEnumerable<QueryElementItem> ToQueryElementItem(string name, QueryDateRange queryDateRange)
    {
        var result = new List<QueryElementItem>();
        if (queryDateRange.Begin.HasValue)
        {
            result.Add(new QueryElementItem
            {
                Name = name,
                Type = ConditionType.Ge,
                Value = queryDateRange.Begin.Value
            });
        }

        if (queryDateRange.End.HasValue)
        {
            result.Add(new QueryElementItem
            {
                Name = name,
                Type = ConditionType.Lt,
                Value = queryDateRange.End.Value
            });
        }

        return result;
    }

    /// <summary>
    /// 转换 查询范围 为 QueryElementItem
    /// </summary>
    /// <param name="name">查询字段名称</param>
    /// <param name="begin">查询范围开始</param>
    /// <param name="end">查询范围结束</param>
    /// <returns>查询项</returns>
    private IEnumerable<QueryElementItem> ToQueryElementItem(string name, object begin, object end)
    {
        var result = new List<QueryElementItem>();
        if (begin != null)
        {
            result.Add(new QueryElementItem
            {
                Name = name,
                Type = ConditionType.Ge,
                Value = begin
            });
        }

        if (end != null)
        {
            result.Add(new QueryElementItem
            {
                Name = name,
                Type = ConditionType.Lt,
                Value = end
            });
        }

        return result;
    }

    /// <summary>
    /// 转换 查询范围 为 QueryElementItem
    /// </summary>
    /// <param name="name">查询字段名称</param>
    /// <param name="value">查询项</param>
    /// <param name="type">查询类型</param>
    /// <returns>查询项</returns>
    private IEnumerable<QueryElementItem> ToQueryElementItem(string name, IEnumerable value, ConditionType type)
    {
        if (!value.Any()) return new QueryElementItem[0];
        return new[]
        {
            new QueryElementItem
            {
                Name = name,
                Type = type == ConditionType.NotIn
                    ? ConditionType.NotIn
                    : ConditionType.In,
                Value = value
            }
        };
    }
}