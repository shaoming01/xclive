using System.Data;
using SchemaBuilder.Svc.Svc;
using ServiceStack;
using ServiceStack.Configuration;
using ServiceStack.Data;
using ServiceStack.Logging;
using ServiceStack.MiniProfiler;
using ServiceStack.OrmLite;
using ServiceStack.Text;
using ServiceStack.Validation;
using ServiceStack.Web;
using FieldDefinition = ServiceStack.OrmLite.FieldDefinition;

namespace SchemaBuilder.Svc.Core.Aq;

public partial class AutoQuery : IAutoQueryDb, IAutoQueryOptions
{
    public AutoQueryFeature? Feature { get; set; }
    public string AccessRole => Feature?.AccessRole ?? RoleNames.Admin;
    public int? MaxLimit { get; set; }
    public bool IncludeTotal { get; set; }
    public bool EnableUntypedQueries { get; set; }
    public bool EnableSqlFilters { get; set; }
    public bool OrderByPrimaryKeyOnLimitQuery { get; set; }
    public string RequiredRoleForRawSqlFilters { get; set; }
    public HashSet<string> IgnoreProperties { get; set; }
    public HashSet<string> IllegalSqlFragmentTokens { get; set; }
    public Dictionary<string, QueryDbFieldAttribute> StartsWithConventions { get; set; }
    public Dictionary<string, QueryDbFieldAttribute> EndsWithConventions { get; set; }

    public string UseNamedConnection { get; set; }
    public bool UseDatabaseWriteLocks { get; set; }
    public QueryFilterDelegate GlobalQueryFilter { get; set; }
    public Dictionary<Type, QueryFilterDelegate> QueryFilters { get; set; }
    public List<Action<QueryDbFilterContext>> ResponseFilters { get; set; }

    private static Dictionary<Type, ITypedQuery> TypedQueries = new();

    public Type GetFromType(Type requestDtoType)
    {
        var intoTypeDef = requestDtoType.GetTypeWithGenericTypeDefinitionOf(typeof(IQueryDb<,>));
        if (intoTypeDef != null)
        {
            var args = intoTypeDef.GetGenericArguments();
            return args[1];
        }

        var typeDef = requestDtoType.GetTypeWithGenericTypeDefinitionOf(typeof(IQueryDb<>));
        if (typeDef != null)
        {
            var args = typeDef.GetGenericArguments();
            return args[0];
        }

        throw new NotSupportedException("Request DTO is not an AutoQuery DTO: " + requestDtoType.Name);
    }

    public ITypedQuery GetTypedQuery(Type dtoType, Type fromType)
    {
        if (TypedQueries.TryGetValue(dtoType, out var defaultValue))
            return defaultValue;

        var genericType = typeof(TypedQuery<,>).MakeGenericType(dtoType, fromType);
        defaultValue = genericType.CreateInstance<ITypedQuery>();
        defaultValue.Init(Feature);

        Dictionary<Type, ITypedQuery> snapshot, newCache;
        do
        {
            snapshot = TypedQueries;
            newCache = new Dictionary<Type, ITypedQuery>(TypedQueries)
            {
                [dtoType] = defaultValue
            };
        } while (!ReferenceEquals(
                     Interlocked.CompareExchange(ref TypedQueries, newCache, snapshot), snapshot));

        return defaultValue;
    }

    public SqlExpression<From> Filter<From>(ISqlExpression q, IQueryDb dto, IRequest req)
    {
        GlobalQueryFilter?.Invoke(q, dto, req);

        if (QueryFilters == null)
            return (SqlExpression<From>)q;

        if (!QueryFilters.TryGetValue(dto.GetType(), out var filterFn))
        {
            foreach (var type in dto.GetType().GetInterfaces())
            {
                if (QueryFilters.TryGetValue(type, out filterFn))
                    break;
            }
        }

        filterFn?.Invoke(q, dto, req);

        return (SqlExpression<From>)q;
    }

    public ISqlExpression Filter(ISqlExpression q, IQueryDb dto, IRequest req)
    {
        GlobalQueryFilter?.Invoke(q, dto, req);

        if (QueryFilters == null)
            return q;

        if (!QueryFilters.TryGetValue(dto.GetType(), out var filterFn))
        {
            foreach (var type in dto.GetType().GetInterfaces())
            {
                if (QueryFilters.TryGetValue(type, out filterFn))
                    break;
            }
        }

        filterFn?.Invoke(q, dto, req);

        return q;
    }

    public QueryResponse<Into> ResponseFilter<From, Into>(IDbConnection db, QueryResponse<Into> response,
        SqlExpression<From> expr, IQueryDb dto)
    {
        response.Meta = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        var commands = dto.Include.ParseCommands();

        var ctx = new QueryDbFilterContext
        {
            Db = db,
            Commands = commands,
            Dto = dto,
            SqlExpression = expr,
            Response = response,
        };

        var totalCommand = commands.FirstOrDefault(x => x.Name.EqualsIgnoreCase("Total"));
        if (totalCommand != null)
        {
            totalCommand.Name = "COUNT";
        }

        var totalRequested = commands.Any(x =>
            x.Name.EqualsIgnoreCase("COUNT") &&
            (x.Args.Count == 0 || x.Args.Count == 1 && x.Args[0].EqualsOrdinal("*")));

        if (IncludeTotal || totalRequested)
        {
            if (!totalRequested)
                commands.Add(new Command { Name = "COUNT", Args = { "*".AsMemory() } });

            foreach (var responseFilter in ResponseFilters)
            {
                responseFilter(ctx);
            }

            response.Total = response.Meta.TryGetValue("COUNT(*)", out var total)
                ? total.ToInt()
                : (int)db.Count(expr); //fallback if it's not populated (i.e. if stripped by custom ResponseFilter)

            //reduce payload on wire
            if (totalCommand != null || !totalRequested)
            {
                response.Meta.Remove("COUNT(*)");
                if (response.Meta.Count == 0)
                    response.Meta = null;
            }
        }
        else
        {
            foreach (var responseFilter in ResponseFilters)
            {
                responseFilter(ctx);
            }
        }

        return response;
    }

    public string GetDbNamedConnection(Type fromType, IRequest req = null)
    {
        var namedConnection = UseNamedConnection;
        var attr = fromType.FirstAttribute<NamedConnectionAttribute>();
        return attr != null
            ? attr.Name
            : namedConnection ?? (req != null ? HostContext.AppHost.GetDbNamedConnection(req) : null);
    }

    public IDbConnection GetDb<From>(IRequest req = null) => GetDb(typeof(From), req);

    public IDbConnection GetDb(Type fromType, IRequest req = null)
    {
        return Db.Open();
        var namedConnection = GetDbNamedConnection(fromType, req);
        return namedConnection == null
            ? HostContext.AppHost.GetDbConnection(req)
            : HostContext.TryResolve<IDbConnectionFactory>().OpenDbConnection(namedConnection);
    }

    public SqlExpression<From> CreateQuery<From>(IQueryDb<From> dto, Dictionary<string, string> dynamicParams,
        IRequest req = null, IDbConnection db = null)
    {
        using (db == null ? db = GetDb<From>(req) : null)
        {
            var typedQuery = GetTypedQuery(dto.GetType(), typeof(From));
            var q = typedQuery.CreateQuery(db);
            return Filter<From>(typedQuery.AddToQuery(q, dto, dynamicParams, this, req), dto, req);
        }
    }

    public QueryResponse<From> Execute<From>(IQueryDb<From> model, SqlExpression<From> query, IRequest req = null,
        IDbConnection db = null)
    {
        using (db == null ? db = GetDb<From>(req) : null)
        {
            var typedQuery = GetTypedQuery(model.GetType(), typeof(From));
            return ResponseFilter(db, typedQuery.Execute<From>(db, query), query, model);
        }
    }

    public async Task<QueryResponse<From>> ExecuteAsync<From>(IQueryDb<From> model, SqlExpression<From> query,
        IRequest req = null, IDbConnection db = null)
    {
        using (db == null ? db = GetDb<From>(req) : null)
        {
            var typedQuery = GetTypedQuery(model.GetType(), typeof(From));
            return ResponseFilter(db, await typedQuery.ExecuteAsync<From>(db, query).ConfigAwait(), query, model);
        }
    }

    public SqlExpression<From> CreateQuery<From, Into>(IQueryDb<From, Into> dto,
        Dictionary<string, string> dynamicParams, IRequest req = null, IDbConnection db = null)
    {
        using (db == null ? db = GetDb<From>(req) : null)
        {
            var typedQuery = GetTypedQuery(dto.GetType(), typeof(From));
            var q = typedQuery.CreateQuery(db);
            return Filter<From>(typedQuery.AddToQuery(q, dto, dynamicParams, this, req), dto, req);
        }
    }

    public QueryResponse<Into> Execute<From, Into>(IQueryDb<From, Into> model, SqlExpression<From> query,
        IRequest req = null, IDbConnection db = null)
    {
        using (db == null ? db = GetDb<From>(req) : null)
        {
            var typedQuery = GetTypedQuery(model.GetType(), typeof(From));
            return ResponseFilter(db, typedQuery.Execute<Into>(db, query), query, model);
        }
    }

    public async Task<QueryResponse<Into>> ExecuteAsync<From, Into>(IQueryDb<From, Into> model,
        SqlExpression<From> query, IRequest req = null, IDbConnection db = null)
    {
        using (db == null ? db = GetDb<From>(req) : null)
        {
            var typedQuery = GetTypedQuery(model.GetType(), typeof(From));
            return ResponseFilter(db, await typedQuery.ExecuteAsync<Into>(db, query).ConfigAwait(), query, model);
        }
    }

    public ISqlExpression CreateQuery(IQueryDb requestDto, Dictionary<string, string> dynamicParams,
        IRequest req = null, IDbConnection db = null)
    {
        var requestDtoType = requestDto.GetType();
        var fromType = GetFromType(requestDtoType);
        using (db == null ? db = GetDb(fromType) : null)
        {
            var typedQuery = GetTypedQuery(requestDtoType, fromType);
            var q = typedQuery.CreateQuery(db);
            return Filter(typedQuery.AddToQuery(q, requestDto, dynamicParams, this, req), requestDto, req);
        }
    }

    private Dictionary<Type, GenericAutoQueryDb> genericAutoQueryCache = new();

    public IQueryResponse Execute(IQueryDb request, ISqlExpression q, IDbConnection db)
    {
        if (db == null)
            throw new ArgumentNullException(nameof(db));

        var requestDtoType = request.GetType();

        ResolveTypes(requestDtoType, out var fromType, out var intoType);

        if (genericAutoQueryCache.TryGetValue(fromType, out GenericAutoQueryDb typedApi))
            return typedApi.ExecuteObject(this, request, q, db);

        var instance = GetGenericAutoQueryDb(fromType, intoType, requestDtoType);

        return instance.ExecuteObject(this, request, q, db);
    }

    public Task<IQueryResponse> ExecuteAsync(IQueryDb request, ISqlExpression q, IDbConnection db)
    {
        if (db == null)
            throw new ArgumentNullException(nameof(db));

        var requestDtoType = request.GetType();

        ResolveTypes(requestDtoType, out var fromType, out var intoType);

        if (genericAutoQueryCache.TryGetValue(fromType, out GenericAutoQueryDb typedApi))
            return typedApi.ExecuteObjectAsync(this, request, q, db);

        var instance = GetGenericAutoQueryDb(fromType, intoType, requestDtoType);

        return instance.ExecuteObjectAsync(this, request, q, db);
    }

    private GenericAutoQueryDb GetGenericAutoQueryDb(Type fromType, Type intoType, Type requestDtoType)
    {
        var genericType = typeof(GenericAutoQueryDb<,>).MakeGenericType(fromType, intoType);
        var instance = genericType.CreateInstance<GenericAutoQueryDb>();

        Dictionary<Type, GenericAutoQueryDb> snapshot, newCache;
        do
        {
            snapshot = genericAutoQueryCache;
            newCache = new Dictionary<Type, GenericAutoQueryDb>(genericAutoQueryCache)
            {
                [requestDtoType] = instance
            };
        } while (!ReferenceEquals(
                     Interlocked.CompareExchange(ref genericAutoQueryCache, newCache, snapshot), snapshot));

        return instance;
    }

    private static void ResolveTypes(Type requestDtoType, out Type fromType, out Type intoType)
    {
        var intoTypeDef = requestDtoType.GetTypeWithGenericTypeDefinitionOf(typeof(IQueryDb<,>));
        if (intoTypeDef != null)
        {
            var args = intoTypeDef.GetGenericArguments();
            fromType = args[0];
            intoType = args[1];
        }
        else
        {
            var typeDef = requestDtoType.GetTypeWithGenericTypeDefinitionOf(typeof(IQueryDb<>));
            var args = typeDef.GetGenericArguments();
            fromType = args[0];
            intoType = args[0];
        }
    }
}

public partial class AutoQuery : IAutoCrudDb
{
    public static HashSet<string> IgnoreCrudProperties { get; } = new()
    {
        nameof(IHasSessionId.SessionId),
        nameof(IHasBearerToken.BearerToken),
        nameof(IHasVersion.Version),
    };

    public static HashSet<string> IncludeCrudProperties { get; set; } = new()
    {
        Keywords.Reset,
        Keywords.RowVersion,
    };

    public object GetDbLock<From>(IRequest req = null) => GetDbLock(typeof(From), req);

    public object GetDbLock(Type fromType, IRequest req = null) => !UseDatabaseWriteLocks
        ? null
        : Locks.GetDbLock(GetDbNamedConnection(fromType, req));

    T DbExec<T>(IDbConnection db, object useLock, Func<IDbConnection, T> fn)
    {
        // When UseDatabaseWriteLocks=true to prevent concurrent writes (e.g for SQLite)
        // Primary DB Connection uses Locks.AppDb whilst Named Connections uses Locks.NamedConnections
        if (useLock != null)
        {
            lock (useLock)
            {
                return fn(db);
            }
        }

        return fn(db);
    }

    public object Create<Table>(ICreateDb<Table> dto, IRequest req, IDbConnection db = null)
    {
        //TODO: Allow Create to use Default Values
        using var newDb = db == null ? GetDb<Table>(req) : null;
        db ??= newDb;
        using var profiler = Profiler.Current.Step("AutoQuery.Create");

        var ctx = CrudContext.Create<Table>(req, db, dto, AutoCrudOperation.Create);
        Feature?.OnBeforeCreate?.Invoke(ctx);

        ctx.Response = ExecAndReturnResponse<Table>(ctx,
            ctx =>
            {
                var dtoValues = CreateDtoValues(ctx.Request, ctx.Dto);
                var pkField = ctx.ModelDef.PrimaryKey;
                var selectIdentity = ctx.IdProp != null || ctx.ResultProp != null || ctx.Events != null;

                //Use same Id if being executed from id
                if (req.Items.TryGetValue(Keywords.EventModelId, out var eventId) && eventId != null
                    && !dtoValues.ContainsKey(pkField.Name))
                {
                    dtoValues[pkField.Name] = eventId.ConvertTo(pkField.PropertyInfo.PropertyType);
                    selectIdentity = false;
                }

                var isAutoId = pkField.AutoIncrement || pkField.AutoId;
                if (!isAutoId)
                {
                    selectIdentity = false;
                    var pkValue = dtoValues.TryGetValue(pkField.Name, out var value)
                        ? value
                        : null;
                    if (pkValue == null || pkValue.Equals(pkField.FieldTypeDefaultValue))
                        throw new ArgumentException(ErrorMessages.PrimaryKeyRequired, pkField.Name);
                }

                var autoIntId = DbExec(db, GetDbLock<Table>(req), d =>
                    d.Insert<Table>(dtoValues, selectIdentity: selectIdentity));
                return CreateInternal(dtoValues, pkField, selectIdentity, autoIntId);
            });

        Feature?.OnAfterCreate?.Invoke(ctx);
        return ctx.Response;
    }

    public async Task<object> CreateAsync<Table>(ICreateDb<Table> dto, IRequest req, IDbConnection db = null)
    {
        //TODO: Allow Create to use Default Values
        using var newDb = db == null ? GetDb<Table>(req) : null;
        db ??= newDb;
        if (!db.GetDialectProvider().SupportsAsync)
            return Create(dto, req, db);

        using var profiler = Profiler.Current.Step("AutoQuery.Create");

        var ctx = CrudContext.Create<Table>(req, db, dto, AutoCrudOperation.Create);
        if (Feature?.OnBeforeCreateAsync != null)
            await Feature.OnBeforeCreateAsync(ctx);

        ctx.Response = await ExecAndReturnResponseAsync<Table>(ctx,
            async ctx =>
            {
                var dtoValues = CreateDtoValues(ctx.Request, ctx.Dto);
                var pkField = ctx.ModelDef.PrimaryKey;
                var selectIdentity = ctx.IdProp != null || ctx.ResultProp != null || ctx.Events != null;

                //Use same Id if being executed from id
                if (req.Items.TryGetValue(Keywords.EventModelId, out var eventId) && eventId != null
                    && !dtoValues.ContainsKey(pkField.Name))
                {
                    dtoValues[pkField.Name] = eventId.ConvertTo(pkField.PropertyInfo.PropertyType);
                    selectIdentity = false;
                }

                var isAutoId = pkField.AutoIncrement || pkField.AutoId;
                if (!isAutoId)
                {
                    selectIdentity = false;
                    var pkValue = dtoValues.TryGetValue(pkField.Name, out var value)
                        ? value
                        : null;
                    if (pkValue == null || pkValue.Equals(pkField.FieldTypeDefaultValue))
                        throw new ArgumentException(ErrorMessages.PrimaryKeyRequired, pkField.Name);
                }

                var autoIntId = await db.InsertAsync<Table>(dtoValues, selectIdentity: selectIdentity).ConfigAwait();
                return CreateInternal(dtoValues, pkField, selectIdentity, autoIntId);
            }).ConfigAwait();

        if (Feature?.OnAfterCreateAsync != null)
            await Feature.OnAfterCreateAsync(ctx);
        return ctx.Response;
    }

    private static ExecValue CreateInternal(Dictionary<string, object> dtoValues,
        FieldDefinition pkField, bool selectIdentity, long autoIntId)
    {
        // [AutoId] Guid's populate the PK Property or return Id if provided
        var isAutoId = pkField?.AutoId == true;
        var providedId = pkField != null && dtoValues.ContainsKey(pkField.Name);
        if (isAutoId || providedId)
            return new ExecValue(pkField.GetValue(dtoValues), selectIdentity ? 1 : autoIntId);

        return selectIdentity
            ? new ExecValue(autoIntId, 1)
            : pkField != null && dtoValues.TryGetValue(pkField.Name, out var idValue)
                ? new ExecValue(idValue, autoIntId)
                : new ExecValue(null, autoIntId);
    }

    public object Update<Table>(IUpdateDb<Table> dto, IRequest req, IDbConnection db = null)
    {
        return UpdateInternal<Table>(req, dto, AutoCrudOperation.Update, db);
    }

    public Task<object> UpdateAsync<Table>(IUpdateDb<Table> dto, IRequest req, IDbConnection db = null)
    {
        return UpdateInternalAsync<Table>(req, dto, AutoCrudOperation.Update, db);
    }

    public object Patch<Table>(IPatchDb<Table> dto, IRequest req, IDbConnection db = null)
    {
        return UpdateInternal<Table>(req, dto, AutoCrudOperation.Patch, db);
    }

    public Task<object> PatchAsync<Table>(IPatchDb<Table> dto, IRequest req, IDbConnection db = null)
    {
        return UpdateInternalAsync<Table>(req, dto, AutoCrudOperation.Patch, db);
    }

    public object PartialUpdate<Table>(object dto, IRequest req, IDbConnection db = null) =>
        UpdateInternal<Table>(req, dto, AutoCrudOperation.Patch, db);

    public Task<object> PartialUpdateAsync<Table>(object dto, IRequest req, IDbConnection db = null) =>
        UpdateInternalAsync<Table>(req, dto, AutoCrudOperation.Patch, db);

    private object UpdateInternal<Table>(IRequest req, object dto, string operation, IDbConnection db = null)
    {
        var skipDefaults = operation == AutoCrudOperation.Patch;
        using var newDb = db == null ? GetDb<Table>(req) : null;
        db ??= newDb;
        using (Profiler.Current.Step("AutoQuery.Update"))
        {
            var ctx = CrudContext.Create<Table>(req, db, dto, operation);

            if (skipDefaults)
                Feature?.OnBeforePatch?.Invoke(ctx);
            else
                Feature?.OnBeforeUpdate?.Invoke(ctx);

            ctx.Response = ExecAndReturnResponse<Table>(ctx,
                ctx =>
                {
                    var dtoValues = CreateDtoValues(req, dto, skipDefaults);
                    var pkField = ctx.ModelDef?.PrimaryKey;
                    if (pkField == null)
                        throw new NotSupportedException($"Table '{typeof(Table).Name}' does not have a primary key");
                    if (!dtoValues.TryGetValue(pkField.Name, out var idValue) ||
                        AutoMappingUtils.IsDefaultValue(idValue))
                        throw new ArgumentNullException(pkField.Name);

                    // Should only update a Single Row
                    var rowsUpdated = GetAutoFilterExpressions(ctx, dtoValues, out var expr, out var exprParams)
                        ? DbExec(ctx.Db, GetDbLock<Table>(req),
                            d => d.UpdateOnly<Table>(dtoValues, expr, exprParams.ToArray()))
                        : DbExec(ctx.Db, GetDbLock<Table>(req), d => d.UpdateOnly<Table>(dtoValues));

                    if (rowsUpdated != 1)
                        throw new OptimisticConcurrencyException(
                            $"{rowsUpdated} rows were updated by '{dto.GetType().Name}'");

                    return new ExecValue(idValue, rowsUpdated);
                }); //TODO: UpdateOnly

            if (skipDefaults)
                Feature?.OnAfterPatch?.Invoke(ctx);
            else
                Feature?.OnAfterUpdate?.Invoke(ctx);

            return ctx.Response;
        }
    }

    private async Task<object> UpdateInternalAsync<Table>(IRequest req, object dto, string operation,
        IDbConnection db = null)
    {
        var skipDefaults = operation == AutoCrudOperation.Patch;
        using var newDb = db == null ? GetDb<Table>(req) : null;
        db ??= newDb;
        if (!db.GetDialectProvider().SupportsAsync)
            return UpdateInternal<Table>(req, dto, operation, db);

        using (Profiler.Current.Step("AutoQuery.Update"))
        {
            var ctx = CrudContext.Create<Table>(req, db, dto, operation);

            if (skipDefaults)
            {
                if (Feature?.OnBeforePatchAsync != null)
                    await Feature.OnBeforePatchAsync(ctx);
            }
            else
            {
                if (Feature?.OnBeforeUpdateAsync != null)
                    await Feature.OnBeforeUpdateAsync(ctx);
            }

            ctx.Response = await ExecAndReturnResponseAsync<Table>(ctx,
                async ctx =>
                {
                    var dtoValues = CreateDtoValues(req, dto, skipDefaults);
                    var pkField = ctx.ModelDef?.PrimaryKey;
                    if (pkField == null)
                        throw new NotSupportedException($"Table '{typeof(Table).Name}' does not have a primary key");
                    if (!dtoValues.TryGetValue(pkField.Name, out var idValue) ||
                        AutoMappingUtils.IsDefaultValue(idValue))
                        throw new ArgumentNullException(pkField.Name);

                    // Should only update a Single Row
                    var rowsUpdated = GetAutoFilterExpressions(ctx, dtoValues, out var expr, out var exprParams)
                        ? await ctx.Db.UpdateOnlyAsync<Table>(dtoValues, expr, exprParams.ToArray()).ConfigAwait()
                        : await ctx.Db.UpdateOnlyAsync<Table>(dtoValues).ConfigAwait();

                    if (rowsUpdated != 1)
                        throw new OptimisticConcurrencyException(
                            $"{rowsUpdated} rows were updated by '{dto.GetType().Name}'");

                    return new ExecValue(idValue, rowsUpdated);
                }).ConfigAwait(); //TODO: UpdateOnly

            if (skipDefaults)
            {
                if (Feature?.OnAfterPatchAsync != null)
                    await Feature.OnAfterPatchAsync(ctx);
            }
            else
            {
                if (Feature?.OnAfterUpdateAsync != null)
                    await Feature.OnAfterUpdateAsync(ctx);
            }

            return ctx.Response;
        }
    }

    public object Delete<Table>(IDeleteDb<Table> dto, IRequest req, IDbConnection db = null)
    {
        using var newDb = db == null ? GetDb<Table>(req) : null;
        db ??= newDb;
        using var profiler = Profiler.Current.Step("AutoQuery.Delete");

        var meta = AutoCrudMetadata.Create(dto.GetType(), Feature);
        if (meta.SoftDelete)
            return PartialUpdate<Table>(dto, req, db);

        var ctx = CrudContext.Create<Table>(req, db, dto, AutoCrudOperation.Delete);
        Feature.OnBeforeDelete?.Invoke(ctx);

        ctx.Response = ExecAndReturnResponse<Table>(ctx,
            ctx =>
            {
                var dtoValues = CreateDtoValues(req, dto, skipDefaults: true);
                var idValue = ctx.ModelDef.PrimaryKey != null &&
                              dtoValues.TryGetValue(ctx.ModelDef.PrimaryKey.Name, out var oId)
                    ? oId
                    : null;
                var q = DeleteInternal<Table>(ctx, dtoValues);

                return DbExec(ctx.Db, GetDbLock<Table>(req), d =>
                    q != null
                        ? new ExecValue(idValue, d.Delete(q))
                        : new ExecValue(idValue, d.Delete<Table>(dtoValues)));
            });

        Feature?.OnAfterDelete?.Invoke(ctx);
        return ctx.Response;
    }

    public async Task<object> DeleteAsync<Table>(IDeleteDb<Table> dto, IRequest req, IDbConnection db = null)
    {
        using var newDb = db == null ? GetDb<Table>(req) : null;
        db ??= newDb;
        if (!db.GetDialectProvider().SupportsAsync)
            return Delete(dto, req, db);

        using var profiler = Profiler.Current.Step("AutoQuery.Delete");

        var meta = AutoCrudMetadata.Create(dto.GetType(), Feature);
        if (meta.SoftDelete)
            return await UpdateInternalAsync<Table>(req, dto, AutoCrudOperation.Patch, db).ConfigAwait();

        var ctx = CrudContext.Create<Table>(req, db, dto, AutoCrudOperation.Delete);
        if (Feature?.OnBeforeDeleteAsync != null)
            await Feature.OnBeforeDeleteAsync(ctx);

        ctx.Response = await ExecAndReturnResponseAsync<Table>(ctx,
            async ctx =>
            {
                var dtoValues = CreateDtoValues(req, dto, skipDefaults: true);
                var idValue = ctx.ModelDef.PrimaryKey != null &&
                              dtoValues.TryGetValue(ctx.ModelDef.PrimaryKey.Name, out var oId)
                    ? oId
                    : null;
                var q = DeleteInternal<Table>(ctx, dtoValues);
                return q != null
                    ? new ExecValue(idValue, await ctx.Db.DeleteAsync(q).ConfigAwait())
                    : new ExecValue(idValue, await ctx.Db.DeleteAsync<Table>(dtoValues).ConfigAwait());
            }).ConfigAwait();

        if (Feature?.OnAfterDeleteAsync != null)
            await Feature.OnAfterDeleteAsync(ctx);

        return ctx.Response;
    }

    internal SqlExpression<Table> DeleteInternal<Table>(CrudContext ctx, Dictionary<string, object> dtoValues)
    {
        //Should have at least 1 non-default filter
        if (dtoValues.Count == 0)
            throw new NotSupportedException($"'{ctx.RequestType.Name}' did not contain any filters");

        // Should only update a Single Row
        if (GetAutoFilterExpressions(ctx, dtoValues, out var expr, out var exprParams))
        {
            //If there were Auto Filters, construct filter expression manually by adding any remaining DTO values
            foreach (var entry in dtoValues)
            {
                var fieldDef = ctx.ModelDef.GetFieldDefinition(entry.Key);
                if (fieldDef == null)
                    throw new NotSupportedException(
                        $"Unknown '{entry.Key}' Field in '{ctx.RequestType.Name}' IDeleteDb<{typeof(Table).Name}> Request");

                if (expr.Length > 0)
                    expr += " AND ";

                var quotedColumn = ctx.Db.GetDialectProvider().GetQuotedColumnName(ctx.ModelDef, fieldDef);

                expr += quotedColumn + " = {" + exprParams.Count + "}";
                exprParams.Add(entry.Value);
            }

            var q = ctx.Db.From<Table>();
            q.Where(expr, exprParams.ToArray());
            return q;
        }

        return null;
    }

    public object Save<Table>(ISaveDb<Table> dto, IRequest req, IDbConnection db = null)
    {
        using var newDb = db == null ? GetDb<Table>(req) : null;
        db ??= newDb;
        using var profiler = Profiler.Current.Step("AutoQuery.Save");

        var row = dto.ConvertTo<Table>();
        var response = ExecAndReturnResponse<Table>(CrudContext.Create<Table>(req, db, dto, AutoCrudOperation.Save),
            ctx =>
            {
                DbExec(ctx.Db, GetDbLock<Table>(req), d => d.Save(row));
                return SaveInternal(dto, ctx);
            });

        return response;
    }

    public async Task<object> SaveAsync<Table>(ISaveDb<Table> dto, IRequest req, IDbConnection db = null)
    {
        using var newDb = db == null ? GetDb<Table>(req) : null;
        db ??= newDb;
        if (!db.GetDialectProvider().SupportsAsync)
            return Save(dto, req, db);
        using var profiler = Profiler.Current.Step("AutoQuery.Save");

        var row = dto.ConvertTo<Table>();
        var response = await ExecAndReturnResponseAsync<Table>(
            CrudContext.Create<Table>(req, db, dto, AutoCrudOperation.Save),
            async ctx =>
            {
                await ctx.Db.SaveAsync(row).ConfigAwait();
                return SaveInternal(dto, ctx);
            }).ConfigAwait();

        return response;
    }

    private static ExecValue SaveInternal<Table>(ISaveDb<Table> dto, CrudContext ctx)
    {
        //TODO: Use Upsert when available
        object idValue = null;
        var pkField = ctx.ModelDef.PrimaryKey;
        if (pkField != null)
        {
            var propGetter = TypeProperties.Get(dto.GetType()).GetPublicGetter(pkField.Name);
            if (propGetter != null)
                idValue = propGetter(dto);
        }

        return new ExecValue(idValue, 1);
    }

    internal struct ExecValue
    {
        internal object Id;
        internal long? RowsUpdated;

        public ExecValue(object id, long? rowsUpdated)
        {
            Id = id;
            RowsUpdated = rowsUpdated;
        }
    }

    private object ExecAndReturnResponse<Table>(CrudContext context, Func<CrudContext, ExecValue> fn)
    {
        var ignoreEvent = context.Request.Items.ContainsKey(Keywords.IgnoreEvent);
        var trans = context.Events != null && !ignoreEvent && !context.Db.InTransaction()
            ? context.Db.OpenTransaction()
            : null;

        using (trans)
        {
            context.SetResult(fn(context));
            if (context.Events != null && !ignoreEvent)
                context.Events?.Record(context);

            trans?.Commit();
        }

        if (context.ResponseType == null)
            return null;

        object idValue = null;

        var response = context.ResponseType.CreateInstance();
        if (context.IdProp != null && context.Id != null)
        {
            idValue = context.Id.ConvertTo(context.IdProp.PropertyInfo.PropertyType);
            context.IdProp.PublicSetter(response, idValue);
        }

        if (context.CountProp != null && context.RowsUpdated != null)
        {
            context.CountProp.PublicSetter(response,
                context.RowsUpdated.ConvertTo(context.CountProp.PropertyInfo.PropertyType));
        }

        if (idValue != null && context.ResponseType == typeof(Table))
        {
            var result = context.Db.SingleById<Table>(idValue);
            response = result.ConvertTo(context.ResponseType);
        }
        else if (context.ResultProp != null && context.Id != null)
        {
            var result = context.Db.SingleById<Table>(context.Id);
            context.ResultProp.PublicSetter(response, result.ConvertTo(context.ResultProp.PropertyInfo.PropertyType));
        }

        if (context.RowVersionProp != null)
        {
            if (AutoMappingUtils.IsDefaultValue(idValue))
            {
                var dtoIdGetter = context.RequestIdGetter();
                if (dtoIdGetter != null)
                    idValue = dtoIdGetter(context.Dto);
            }

            if (AutoMappingUtils.IsDefaultValue(idValue))
                context.ThrowPrimaryKeyRequiredForRowVersion();

            var rowVersion = context.Db.GetRowVersion<Table>(idValue);
            context.RowVersionProp.PublicSetter(response,
                rowVersion.ConvertTo(context.RowVersionProp.PropertyInfo.PropertyType));
        }

        return response;
    }

    private async Task<object> ExecAndReturnResponseAsync<Table>(CrudContext context,
        Func<CrudContext, Task<ExecValue>> fn)
    {
        var ignoreEvent = context.Request.Items.ContainsKey(Keywords.IgnoreEvent);
        var trans = context.Events != null && !ignoreEvent && !context.Db.InTransaction()
            ? context.Db.OpenTransaction()
            : null;

        using (trans)
        {
            context.SetResult(await fn(context).ConfigAwait());
            if (context.Events != null && !ignoreEvent)
                await context.Events.RecordAsync(context).ConfigAwait();

            trans?.Commit();
        }

        if (context.ResponseType == null)
            return null;

        object idValue = null;

        var response = context.ResponseType.CreateInstance();
        if (context.IdProp != null && context.Id != null)
        {
            idValue = context.Id.ConvertTo(context.IdProp.PropertyInfo.PropertyType);
            context.IdProp.PublicSetter(response, idValue);
        }

        if (context.CountProp != null && context.RowsUpdated != null)
        {
            context.CountProp.PublicSetter(response,
                context.RowsUpdated.ConvertTo(context.CountProp.PropertyInfo.PropertyType));
        }

        if (idValue != null && context.ResponseType == typeof(Table))
        {
            var result = await context.Db.SingleByIdAsync<Table>(idValue).ConfigAwait();
            response = result.ConvertTo(context.ResponseType);
        }
        else if (context.ResultProp != null && context.Id != null)
        {
            var result = await context.Db.SingleByIdAsync<Table>(context.Id).ConfigAwait();
            context.ResultProp.PublicSetter(response, result.ConvertTo(context.ResultProp.PropertyInfo.PropertyType));
        }


        if (context.RowVersionProp != null)
        {
            if (AutoMappingUtils.IsDefaultValue(idValue))
            {
                var dtoIdGetter = context.RequestIdGetter();
                if (dtoIdGetter != null)
                    idValue = dtoIdGetter(context.Dto);
            }

            if (AutoMappingUtils.IsDefaultValue(idValue))
                context.ThrowPrimaryKeyRequiredForRowVersion();

            var rowVersion = await context.Db.GetRowVersionAsync<Table>(idValue).ConfigAwait();
            context.RowVersionProp.PublicSetter(response,
                rowVersion.ConvertTo(context.RowVersionProp.PropertyInfo.PropertyType));
        }

        return response;
    }

    internal bool GetAutoFilterExpressions(CrudContext ctx, Dictionary<string, object> dtoValues, out string expr,
        out List<object> exprParams)
    {
        var meta = AutoCrudMetadata.Create(ctx.RequestType, Feature);
        if (meta.AutoFilters.Count > 0)
        {
            var dialectProvider = ctx.Db.GetDialectProvider();
            var sb = StringBuilderCache.Allocate();
            var exprParamsList = new List<object>();

            //Update's require PK's, Delete's don't need to
            if (dtoValues.TryRemove(meta.ModelDef.PrimaryKey.Name, out var idValue))
            {
                var idColumn = dialectProvider.GetQuotedColumnName(meta.ModelDef, meta.ModelDef.PrimaryKey);
                sb.Append(idColumn + " = {0}");
                exprParamsList.Add(idValue);
            }

            var appHost = HostContext.AppHost;
            for (var i = 0; i < meta.AutoFilters.Count; i++)
            {
                var filter = meta.AutoFilters[i];
                var dbAttr = meta.AutoFiltersDbFields[i];

                var fieldDef = meta.ModelDef.GetFieldDefinition(filter.Field);
                if (fieldDef == null)
                    throw new NotSupportedException(
                        $"{ctx.RequestType.Name} '{filter.Field}' AutoFilter was not found on '{ctx.ModelType.Name}'");

                var quotedColumn = dialectProvider.GetQuotedColumnName(meta.ModelDef, fieldDef);

                var value = appHost.EvalScriptValue(filter, ctx.Request);

                var ret = ExprResult.CreateExpression("AND", quotedColumn, value, dbAttr);

                if (ret != null)
                {
                    if (sb.Length > 0)
                        sb.Append(" AND ");

                    var exprResult = ret.Value;
                    if (exprResult.Format.IndexOf("{1}", StringComparison.Ordinal) >= 0)
                        throw new NotSupportedException(
                            $"SQL Template '{exprResult.Format}' with multiple arguments is not supported");

                    if (exprResult.Values != null)
                    {
                        for (var index = 0; index < exprResult.Values.Length; index++)
                        {
                            sb.Append(exprResult.Format.Replace("{" + index + "}", "{" + exprParamsList.Count + "}"));
                            exprParamsList.Add(exprResult.Values[index]);
                        }
                    }
                }
            }

            expr = StringBuilderCache.ReturnAndFree(sb);
            exprParams = exprParamsList;
            return true;
        }

        expr = null;
        exprParams = null;
        return false;
    }

    public Dictionary<string, object> CreateDtoValues(IRequest req, object dto, bool skipDefaults = false)
    {
        var meta = AutoCrudMetadata.Create(dto.GetType(), Feature);
        var dtoValues = ResolveDtoValues(meta, req, dto, skipDefaults);
        return dtoValues;
    }

    private Dictionary<string, object> ResolveDtoValues(AutoCrudMetadata meta, IRequest req, object dto,
        bool skipDefaults = false)
    {
        ILog log = null;
        var dtoValues = dto.ToObjectDictionary();

        foreach (var entry in meta.MapAttrs)
        {
            if (dtoValues.TryRemove(entry.Key, out var value))
            {
                dtoValues[entry.Value.To] = value;
            }
        }

        List<string> removeKeys = null;
        foreach (var removeDtoProp in meta.RemoveDtoProps)
        {
            removeKeys ??= [];
            removeKeys.Add(removeDtoProp);
        }

        var appHost = HostContext.AppHost;
        if (skipDefaults || meta.UpdateAttrs.Count > 0 || meta.DefaultAttrs.Count > 0)
        {
            Dictionary<string, object> replaceValues = null;

            foreach (var entry in dtoValues)
            {
                var isNullable = meta.NullableProps?.Contains(entry.Key) == true;
                var isDefaultValue =
                    entry.Value == null || (!isNullable && AutoMappingUtils.IsDefaultValue(entry.Value));
                if (isDefaultValue)
                {
                    var handled = false;
                    if (meta.DefaultAttrs.TryGetValue(entry.Key, out var defaultAttr))
                    {
                        handled = true;
                        replaceValues ??= new Dictionary<string, object>();
                        replaceValues[entry.Key] = appHost.EvalScriptValue(defaultAttr, req);
                    }

                    if (!handled)
                    {
                        if (skipDefaults ||
                            (meta.UpdateAttrs.TryGetValue(entry.Key, out var attr) &&
                             attr.Style == AutoUpdateStyle.NonDefaults))
                        {
                            removeKeys ??= new List<string>();
                            removeKeys.Add(entry.Key);
                        }
                    }
                }
            }

            if (replaceValues != null)
            {
                foreach (var entry in replaceValues)
                {
                    dtoValues[entry.Key] = entry.Value;
                }
            }
        }

        if (removeKeys != null)
        {
            foreach (var key in removeKeys)
            {
                dtoValues.RemoveKey(key);
            }
        }

        var resetField = meta.ModelDef.GetFieldDefinition(Keywords.Reset);
        var reset = resetField == null
            ? (dtoValues.TryRemove(Keywords.Reset, out var oReset)
                  ? ValidationFilters.GetResetFields(oReset)
                  : dtoValues.TryRemove(Keywords.reset, out oReset)
                      ? ValidationFilters.GetResetFields(oReset)
                      : null)
              ?? req.GetResetFields()
            : null;

        if (reset != null)
        {
            foreach (var fieldName in reset)
            {
                var field = meta.ModelDef.GetFieldDefinition(fieldName);
                if (field == null)
                    throw new NotSupportedException($"Reset field '{fieldName}' does not exist");
                if (field.IsPrimaryKey)
                    throw new NotSupportedException($"Cannot reset primary key field '{fieldName}'");

                //Note: validation rules for omitted PATCH values that aren't reset ignored in ValidationFilters.RequestFilterAsync
                if (meta.DenyReset.Contains(field.Name))
                {
                    if (meta.ValidateAttrs.ContainsKey(fieldName))
                    {
                        log ??= LogManager.GetLogger(GetType());
                        log.Warn(
                            $"Reset of {field.Name} property containing validators is denied. Use [AllowReset] to override.");
                    }

                    continue;
                }

                dtoValues[field.Name] = field.FieldTypeDefaultValue;
            }
        }

        foreach (var populateAttr in meta.PopulateAttrs)
        {
            dtoValues[populateAttr.Field] = appHost.EvalScriptValue(populateAttr, req);
        }

        var populatorFn = AutoMappingUtils.GetPopulator(typeof(Dictionary<string, object>), meta.DtoType);
        populatorFn?.Invoke(dtoValues, dto);

        // Ensure RowVersion is always populated if defined on Request DTO
        if (meta.RowVersionGetter != null && !dtoValues.ContainsKey(Keywords.RowVersion))
            dtoValues[Keywords.RowVersion] = default(uint);

        return dtoValues;
    }
}