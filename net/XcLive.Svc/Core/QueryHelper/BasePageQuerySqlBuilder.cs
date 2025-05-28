using SchemaBuilder.Svc.Core.Ext;
using SchemaBuilder.Svc.Core.QueryHelper.Data;
using SchemaBuilder.Svc.Helpers;

namespace SchemaBuilder.Svc.Core.QueryHelper;

public abstract class BasePageQuerySqlBuilder
{
    private readonly PageQueryData _queryData;

    private string _defaultOrderBy = "Id";

    /// <summary>
    ///     查询对象
    /// </summary>
    private readonly PageSqlBuilder _pageSqlBuilder;

    private List<string> _where;

    /// <summary>
    ///     子类型调用构建方法
    /// </summary>
    protected BasePageQuerySqlBuilder(PageQueryData queryData)
    {
        _queryData = queryData;
        //实例化 PageSqlBuilder 对象
        _pageSqlBuilder = new PageSqlBuilder(queryData.QueryElementItems);
        PageSize = queryData.PageSize;
        Page = queryData.Page;
        OrderBy = queryData.OrderBy;
    }

    /// <summary>
    ///     表名称
    /// </summary>
    public abstract string TableName { get; set; }

    /// <summary>
    ///     页码
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    ///     每页大小
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    ///     默认排序条件
    /// </summary>
    public virtual string DefaultOrderBy
    {
        get { return _defaultOrderBy; }
        set { _defaultOrderBy = value; }
    }

    /// <summary>
    ///     默认分组条件
    /// </summary>
    public virtual string DefaultGroupBy { get; set; }

    /// <summary>
    ///     排序字段
    /// </summary>
    public string OrderBy { get; set; }

    /// <summary>
    ///     查询条件集合
    /// </summary>
    public List<string> Wheres
    {
        get
        {
            if (_where == null) InitWhere();

            return _where;
        }
        set { _where = value; }
    }

    /// <summary>
    ///     可供重写的获取查询条件处理匹配对象【传入匿名对象】
    ///     格式：
    ///     return new {
    ///     Id = CreateQuery(q =>{
    ///     return string.Format("Id > {0}",q);
    ///     }),
    ///     XXX = CreateQuery(q =>{}) 格式
    ///     };
    /// </summary>
    protected virtual object QueryMatching
    {
        get { return new object(); }
    }

    /// <summary>
    ///     可供重写的排序字段映射【传入匿名对象】
    ///     格式：
    ///     return new {
    ///     Id = "Id,V",
    ///     XXX = "XXX,YYY"
    ///     };
    /// </summary>
    protected virtual object OrderByMatching
    {
        get { return new object(); }
    }

    /// <summary>
    ///     得到查询条件转换集合
    /// </summary>
    /// <returns>查询条件转换集合</returns>
    private Dictionary<string, Func<PageSqlBuilderItem, string>> GetQueryMatchingList()
    {
        var queryMatching = QueryMatching;
        return queryMatching.GetType().GetProperties()
            .Where(w => w.PropertyType == typeof(Func<PageSqlBuilderItem, string>))
            .ToDictionary(k => k.Name, v => (Func<PageSqlBuilderItem, string>)v.GetValue(queryMatching, null));
    }

    /// <summary>
    ///     得到查询条件匹配字典
    /// </summary>
    protected virtual Dictionary<string, string> GetQueryWhereSqlDictionary(HashSet<string> names)
    {
        return new Dictionary<string, string>();
    }

    /// <summary>
    ///     构建查询对象
    /// </summary>
    /// <param name="func">获取查询条件的匿名函数</param>
    /// <returns>查询条件的匿名函数</returns>
    protected virtual Func<PageSqlBuilderItem, string> CreateQuery(Func<PageSqlBuilderItem, string> func)
    {
        return func;
    }

    /// <summary>
    ///     初始化查询条件
    /// </summary>
    private void InitWhere()
    {
        //解析QueryMatching代入的匹配
        var queryMatchingList = GetQueryMatchingList();
        foreach (var item in queryMatchingList) _pageSqlBuilder.IfExist(item.Key, item.Value); //赋值

        //得到没有生成匹配项
        var noneQueryMatchingItems =
            _queryData.QueryElementItems.Where(w =>
                !queryMatchingList.ContainsKey(w.Name)).ToArray();

        //得到重写的查询项
        var queryWhereSqlDic = GetQueryWhereSqlDictionary(noneQueryMatchingItems.Select(x => x.Name).ToHashSet())
                               ?? new Dictionary<string, string>();

        //给这些项默认生成匹配
        var noneWheres =
            PageQueryMatchingWhereUtil.GetQueryMatchingWheres(
                noneQueryMatchingItems.Where(w => !queryWhereSqlDic.ContainsKey(w.Name)));
        //得到Wheres并赋值Wheres集合
        Wheres = _pageSqlBuilder.Where.MergeList(noneWheres, queryWhereSqlDic.Values)
            .Where(w => !w.IsNullOrWhiteSpace()).ToList(); //去除无效查询条件
    }

    /// <summary>
    ///     得到OrderBy
    /// </summary>
    /// <param name="orderBy">排序条件</param>
    /// <returns>排序条件</returns>
    protected virtual string GetOrderBy(string orderBy)
    {
        return OrderByUtil.OrderBy(orderBy,
            DefaultOrderBy.ToUpper().StartsWith("ORDER BY")
                ? DefaultOrderBy
                : string.Format("ORDER BY {0}", DefaultOrderBy), OrderByMatching, false, false);
    }

    /// <summary>
    ///     查询当前页码的Id
    /// </summary>
    /// <returns>查询语句</returns>
    public virtual string ToSelectIdByPageSql()
    {
        var page = Math.Max(1, Page);
        var pageSize = Math.Max(1, PageSize);

        var orderBy = GetOrderBy(OrderBy);

        var sql = string.Format(@"
            SELECT t1.Id FROM
                (SELECT row_number() OVER ({0}) n,Id 
                {1}) t1 
                WHERE t1.n BETWEEN {2} AND {3}
            ", orderBy, CvtTableWhereSql(), (page - 1) * pageSize + 1, pageSize * page);
        return sql;
    }

    /// <summary>
    ///     查询当前筛选条件下的所有匹配Id
    /// </summary>
    /// <returns>查询语句</returns>
    public virtual string ToSelectAllIdSql(bool sort = false)
    {
        var orderBy = sort ? GetOrderBy(OrderBy) : string.Empty;
        return string.Format("SELECT Id {0} {1}", CvtTableWhereSql(), orderBy);
    }

    /// <summary>
    ///     查询当前筛选条件下的所有匹配Id
    /// </summary>
    /// <returns>查询语句</returns>
    public virtual string ToSelectAllIdSqlNoOrderBy()
    {
        return string.Format("SELECT Id {0}", CvtTableWhereSql());
    }

    /// <summary>
    ///     查询当前符合条件的记录条数SQL
    /// </summary>
    /// <returns>查询语句</returns>
    public virtual string ToCountSql()
    {
        return string.Format("SELECT COUNT(*) {0}", CvtTableWhereSql());
    }

    /// <summary>
    ///     查询当前页码数据
    /// </summary>
    /// <returns>查询语句</returns>
    public virtual string ToSelectByPageSql()
    {
        var page = Math.Max(1, Page);
        var pageSize = Math.Max(1, PageSize);

        var orderBy = GetOrderBy(OrderBy);

        var sql = string.Format(@"
            SELECT t1.* FROM
                (SELECT row_number() OVER ({0}) n,* 
                {1}) t1 
                WHERE t1.n BETWEEN {2} AND {3}
            ", orderBy, CvtTableWhereSql(), (page - 1) * pageSize + 1, pageSize * page);
        return sql;
    }

    private string CvtTableWhereSql()
    {
        var where = Wheres.Has() ? string.Format("WHERE {0}", string.Join(" AND ", Wheres)) : string.Empty;
        var groupBy = StringExtent.IsNullOrEmpty(DefaultGroupBy) ? string.Empty : string.Format("GROUP BY {0}", DefaultGroupBy);

        var result = StringExtent.IsNullOrEmpty(groupBy)
            ? string.Format("{0} {1}", TableName, where)
            : string.Format("FROM (SELECT {0} {1} {2}) o", TableName, where, groupBy);
        return result;
    }

    /// <summary>
    ///     查询所有数据
    /// </summary>
    /// <returns>查询语句</returns>
    public virtual string ToSelectAll()
    {
        var orderBy = GetOrderBy(OrderBy);
        return string.Format("SELECT * {0} {1}", CvtTableWhereSql(), orderBy);
    }


    public virtual string ToTableWhereSql()
    {
        return CvtTableWhereSql();
    }

    #region 创建统计SQL

    /// <summary>
    ///     创建统计SQL
    /// </summary>
    /// <param name="dtoType">统计类型</param>
    /// <returns>统计SQL</returns>
    public virtual string ToStatisticsSql(Type dtoType)
    {
        if (dtoType == null) return null;
        var fields = GetStatisticsFields(dtoType);
        if (!fields.Has()) return null;
        return string.Format("SELECT {0} {1}", string.Join(",", fields), CvtTableWhereSql());
    }

    private string[] GetStatisticsFields(Type dtoType)
    {
        return dtoType.GetProperties().Select(pro =>
        {
            var statisticsField = pro.ReadAttributes<StatisticsFieldAttribute>().FirstOrDefault();
            if (statisticsField == null) return null;
            var field = GetStatisticsField(statisticsField.Field ?? string.Empty, statisticsField.Type);
            return string.Format("{0} AS {1}", field, pro.Name);
        }).Where(w => !StringExtent.IsNullOrEmpty(w)).ToArray();
    }

    private string GetStatisticsField(string field, StatisticsType type)
    {
        switch (type)
        {
            case StatisticsType.Count:
                return string.Format("COUNT({0})", field.Trim());
            case StatisticsType.Sum:
                return string.Format("SUM({0})", field.Trim());
            case StatisticsType.Avg:
                return string.Format("AVG({0})", field.Trim());
            case StatisticsType.Max:
                return string.Format("MAX({0})", field.Trim());
            case StatisticsType.Min:
                return string.Format("MIN({0})", field.Trim());
            case StatisticsType.Stdev:
                return string.Format("STDEV({0})", field.Trim());
            case StatisticsType.Var:
                return string.Format("VAR({0})", field.Trim());
            default:
                return field.Trim();
        }
    }

    #endregion
}