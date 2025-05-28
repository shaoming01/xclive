using System.Data;
using System.Linq.Expressions;
using SchemaBuilder.Svc.Core.Ext;
using SchemaBuilder.Svc.Core.QueryHelper.Data;
using SchemaBuilder.Svc.Core.QueryHelper.Data.@interface;
using SchemaBuilder.Svc.Helpers;
using SchemaBuilder.Svc.Svc;
using ServiceStack.OrmLite;

namespace SchemaBuilder.Svc.Core.QueryHelper;

public class PageQuerySqlBuilder<TQ, TR> : BasePageQuerySqlBuilder, IRetrievePage<TR>, ITotalCount, IStatistics
    where TR : IIdObject
{
    private string _tableName = string.Format("FROM {0} t", typeof(TR).Name);

    protected PageQuerySqlBuilder(PageQueryObject<TQ> pageQueryObject, bool isNoLock = false) : base(
        QueryConvertorUtil.QueryConvertor.Convertor(pageQueryObject))
    {
        PageQueryObject = pageQueryObject;
        IsNoLock = isNoLock;
    }

    /// <summary>
    ///     查询对象
    /// </summary>
    private PageQueryObject<TQ> PageQueryObject { get; set; }

    /// <summary>
    ///     是否NoLock查询
    /// </summary>
    private bool IsNoLock { get; set; }

    /// <summary>
    ///     等待重写的查询匹配【基于强类型的查询条件重写】
    /// </summary>
    protected virtual QueryMatchingItem<TQ>[] QueryMatchingItems
    {
        get { return Array.Empty<QueryMatchingItem<TQ>>(); }
    }


    /// <summary>
    ///     表名称
    /// </summary>
    public override string TableName
    {
        get { return _tableName; }
        set { _tableName = value; }
    }

    /// <summary>
    ///     查询当前页数据
    /// </summary>
    /// <param name="db">数据库连接对象</param>
    /// <returns>数据</returns>
    public virtual PageResult<TR> RetrievePage(IDbConnection db)
    {
        var pageSql = ToSelectByPageSql();
        var items = db.Select<TR>(pageSql);
        var totalCount = items.Count < PageSize
            ? (Page - 1) * PageSize + items.Count
            : db.Single<int>(ToCountSql());
        return new PageResult<TR>
        {
            Items = items,
            TotalCount = totalCount
        };
    }

    /// <summary>
    ///     查询当前页数据
    /// </summary>
    /// <returns>数据</returns>
    public virtual PageResult<TR> RetrievePage()
    {
        return IsNoLock ? Db.OpenDbWithNoLock(db => RetrievePage(db)) : Db.Open(db => { return RetrievePage(db); });
    }

    /// <summary>
    ///     查询当前数据l列表
    /// </summary>
    public virtual List<TR> RetrieveItems(IDbConnection db)
    {
        var pageSql = ToSelectByPageSql();
        var items = db.Select<TR>(pageSql);
        return items;
    }

    /// <summary>
    ///     查询当前页数据
    /// </summary>
    /// <returns>数据</returns>
    public virtual List<TR> RetrieveItems()
    {
        return IsNoLock ? Db.OpenDbWithNoLock(db => RetrieveItems(db)) : Db.Open(db => RetrieveItems(db));
    }

    /// <summary>
    ///     获得统计SQL
    /// </summary>
    /// <typeparam name="TS">统计类型</typeparam>
    /// <returns>统计SQL</returns>
    public virtual string ToStatisticsSql<TS>()
    {
        return base.ToStatisticsSql(typeof(TS));
    }

    /// <summary>
    ///     得到统计数据
    /// </summary>
    /// <param name="db">连接对象</param>
    /// <typeparam name="TS">统计对象</typeparam>
    /// <returns>统计对象</returns>
    public virtual TS Statistics<TS>(IDbConnection db)
    {
        var sql = ToStatisticsSql<TS>();
        if (StringExtent.IsNullOrEmpty(sql)) return default(TS);
        return db.Single<TS>(sql);
    }

    /// <summary>
    ///     得到统计数据
    /// </summary>
    /// <typeparam name="TS">统计对象</typeparam>
    /// <returns>统计对象</returns>
    public virtual TS Statistics<TS>()
    {
        return IsNoLock ? Db.OpenDbWithNoLock(db => Statistics<TS>(db)) : Db.Open(db => Statistics<TS>(db));
    }

    /// <summary>
    ///     获取当前查询条件下的总数
    /// </summary>
    /// <param name="db">数据库连接对象</param>
    /// <returns>总数</returns>
    public virtual int TotalCount(IDbConnection db)
    {
        return db.Single<int>(base.ToCountSql());
    }

    /// <summary>
    ///     获取当前查询条件下的总数
    /// </summary>
    /// <returns>总数</returns>
    public virtual int TotalCount()
    {
        return IsNoLock ? Db.OpenDbWithNoLock(db => TotalCount(db)) : Db.Open(db => TotalCount(db));
    }

    /// <summary>
    ///     得到重写的查询项
    /// </summary>
    protected override Dictionary<string, string> GetQueryWhereSqlDictionary(HashSet<string> names)
    {
        if (!QueryMatchingItems.Has()) return null;
        return QueryMatchingItems.Where(w => names.Contains(w.Name)).GroupBy(b => b.Name)
            .ToDictionary(k => k.Key, v => v.First().ToWhereSql(PageQueryObject.QueryObject));
    }

    /// <summary>
    ///     创建查询匹配
    /// </summary>
    /// <typeparam name="TP">属性类型</typeparam>
    /// <param name="getProperty">得到属性</param>
    /// <param name="getWhereSql">得到查询SQL</param>
    protected QueryMatchingItem<TQ> CreateQueryMatchingItem<TP>(Expression<Func<TQ, TP>> getProperty,
        Func<QueryMatchingItem<TQ, TP>.Parameter, string> getWhereSql)
    {
        return new QueryMatchingItem<TQ, TP>
        {
            Name = MemberVisit<TQ>.GetMemberStrings(getProperty.Body)[0],
            GetProperty = getProperty.Compile(),
            GetWhereSql = getWhereSql
        };
    }

    /// <summary>
    ///     分页查询数据
    /// </summary>
    /// <param name="db">数据库连接对象</param>
    /// <param name="queryData">查询数据</param>
    /// <returns>分页查询结果</returns>
    public static PageResult<TR> RetrievePage(IDbConnection db, PageQueryObject<TQ> queryData)
    {
        return new PageQuerySqlBuilder<TQ, TR>(queryData).RetrievePage(db);
    }

    /// <summary>
    ///     分页查询数据
    /// </summary>
    /// <param name="queryData">查询数据</param>
    /// <returns>分页查询结果</returns>
    public static PageResult<TR> RetrievePage(PageQueryObject<TQ> queryData)
    {
        return new PageQuerySqlBuilder<TQ, TR>(queryData).RetrievePage();
    }

    /// <summary>
    ///     得到统计数据
    /// </summary>
    /// <param name="db">数据库连接对象</param>
    /// <typeparam name="TS">统计对象</typeparam>
    /// <param name="queryData">查询数据</param>
    /// <returns>统计对象</returns>
    public static TS Statistics<TS>(IDbConnection db, PageQueryObject<TQ> queryData)
    {
        return new PageQuerySqlBuilder<TQ, TR>(queryData).Statistics<TS>(db);
    }

    /// <summary>
    ///     得到统计数据
    /// </summary>
    /// <typeparam name="TS">统计对象</typeparam>
    /// <param name="queryData">查询数据</param>
    /// <returns>统计对象</returns>
    public static TS Statistics<TS>(PageQueryObject<TQ> queryData)
    {
        return new PageQuerySqlBuilder<TQ, TR>(queryData).Statistics<TS>();
    }


    /// <summary>
    ///     获取当前查询条件下的总数
    /// </summary>
    /// <param name="db">数据库连接对象</param>
    /// <param name="queryData">查询数据</param>
    /// <returns>总数</returns>
    public static int TotalCount(IDbConnection db, PageQueryObject<TQ> queryData)
    {
        return new PageQuerySqlBuilder<TQ, TR>(queryData).TotalCount(db);
    }

    /// <summary>
    ///     获取当前查询条件下的总数
    /// </summary>
    /// <param name="queryData">查询数据</param>
    /// <returns>总数</returns>
    public static int TotalCount(PageQueryObject<TQ> queryData)
    {
        return new PageQuerySqlBuilder<TQ, TR>(queryData).TotalCount();
    }
}