using SchemaBuilder.Svc.Core.QueryHelper.Data;
using SchemaBuilder.Svc.Helpers;
using SchemaBuilder.Svc.Models.Dto;
using SchemaBuilder.Svc.Models.Table;
using ServiceStack;
using ServiceStack.Data;
using AutoQueryFeature = SchemaBuilder.Svc.Core.Aq.AutoQueryFeature;

namespace SchemaBuilder.Svc.Svc;

public static class QuerySvc
{
    public static QueryResponse<TInto> Query<TQuery, TInto>(TQuery queryModel)
        where TQuery : IQueryDb<TInto>
    {
        var queryFeature = new AutoQueryFeature();
        var autoQuery = queryFeature.CreateAutoQueryDb(GetDbFactory());
        var exp = autoQuery.CreateQuery(queryModel, new Dictionary<string, string>());
        var result = autoQuery.Execute(queryModel, exp);
        return result;
    }

    public static QueryResponse<TInto> Query<TQuery, TFrom, TInto>(TQuery queryModel)
        where TQuery : IQueryDb<TFrom, TInto>
    {
        var queryFeature = new AutoQueryFeature();
        var autoQuery = queryFeature.CreateAutoQueryDb(GetDbFactory());
        var exp = autoQuery.CreateQuery(queryModel, new Dictionary<string, string>());
        var result = autoQuery.Execute(queryModel, exp);
        return result;
    }

    private static IDbConnectionFactory GetDbFactory()
    {
        return Db._dbFactory;
    }

    public static List<TInto> QueryList<TQuery, TInto>(PageQueryObject<TQuery> query) where TQuery : IQueryDb<TInto>
    {
        var queryModel = query.QueryObject;
        if (queryModel == null)
        {
            queryModel = typeof(TQuery).CreateInstance<TQuery>();
            query.QueryObject = queryModel;
        }

        queryModel.Include = "";
        queryModel.Take = query.PageSize;
        queryModel.Skip = query.PageSize * (query.Page - 1);

        var result = Query<TQuery, TInto>(queryModel);
        return result.Results;
    }

    public static List<TInto> QueryList<TQuery, TFrom, TInto>(PageQueryObject<TQuery> query, UserLoginInfo user)
        where TQuery : IQueryDb<TFrom, TInto>
    {
        var queryModel = query.QueryObject;
        if (queryModel == null)
        {
            queryModel = typeof(TQuery).CreateInstance<TQuery>();
            query.QueryObject = queryModel;
        }

        if (typeof(TQuery).ImplementsInterface(typeof(IUserIdQuery)))
        {
            var q = (IUserIdQuery)query.QueryObject!;
            q.UserId = user.UserId;
        }
        else if (typeof(TQuery).ImplementsInterface(typeof(IUserId)))
        {
            var q = (IUserId)query.QueryObject!;
            q.UserId = user.UserId;
        }

        if (typeof(TQuery).ImplementsInterface(typeof(ITenantIdQuery)))
        {
            var q = (ITenantIdQuery)query.QueryObject!;
            q.TenantId = user.TenantId;
        }
        else if (typeof(TQuery).ImplementsInterface(typeof(ITenantId)))
        {
            var q = (ITenantId)query.QueryObject!;
            q.TenantId = user.TenantId;
        }

        queryModel.Include = "";
        queryModel.Take = query.PageSize;
        queryModel.Skip = query.PageSize * (query.Page - 1);

        var result = Query<TQuery, TFrom, TInto>(queryModel);
        return result.Results;
    }


    public static int QueryCount<TQuery, TInto>(PageQueryObject<TQuery> query) where TQuery : IQueryDb<TInto>
    {
        var queryModel = query.QueryObject;
        if (queryModel == null)
        {
            queryModel = typeof(TQuery).CreateInstance<TQuery>();
            query.QueryObject = queryModel;
        }

        queryModel.Include = "count";
        queryModel.Take = 0;
        queryModel.Skip = 0;

        var result = Query<TQuery, TInto>(queryModel);
        return result.Total;
    }

    public static int QueryCount<TQuery, TFrom, TInto>(PageQueryObject<TQuery> query, UserLoginInfo user)
        where TQuery : IQueryDb<TFrom, TInto>
    {
        var queryModel = query.QueryObject;
        if (queryModel == null)
        {
            queryModel = typeof(TQuery).CreateInstance<TQuery>();
            query.QueryObject = queryModel;
        }

        if (typeof(TQuery).ImplementsInterface(typeof(IUserId)))
        {
            var q = (IUserId)query.QueryObject!;
            q.UserId = user.UserId;
        }

        if (typeof(TQuery).ImplementsInterface(typeof(ITenantId)))
        {
            var q = (ITenantId)query.QueryObject!;
            q.TenantId = user.TenantId;
        }

        queryModel.Include = "count";
        queryModel.Take = 0;
        queryModel.Skip = 0;

        var result = Query<TQuery, TFrom, TInto>(queryModel);
        return result.Total;
    }
}