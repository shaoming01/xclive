using System.Collections;
using System.Collections.Concurrent;
using System.Data;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using ServiceStack;
using ServiceStack.Configuration;
using ServiceStack.Data;
using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;
using ServiceStack.Text;
using ServiceStack.Web;

namespace SchemaBuilder.Svc.Core.Aq;

public partial class AutoQueryFeature
{
    public List<Action<AutoCrudMetadata>> AutoCrudMetadataFilters { get; set; } =
    [
        AuditAutoCrudMetadataFilter
    ];

    public string AccessRole { get; set; } = RoleNames.Admin;

    public Dictionary<Type, string[]> ServiceRoutes { get; set; } = new()
    {
        [typeof(GetCrudEventsService)] = ["/" + "crudevents".Localize() + "/{Model}"],
        [typeof(CheckCrudEventService)] = ["/" + "crudevents".Localize() + "/check"],
    };

    /// <summary>
    /// Which CRUD operations to implement AutoBatch implementations for 
    /// </summary>
    public List<string> GenerateAutoBatchImplementationsFor { get; set; } = Crud.Write.ToList();

    public Action<CrudContext> OnBeforeCreate { get; set; }
    public Func<CrudContext, Task> OnBeforeCreateAsync { get; set; }
    public Action<CrudContext> OnAfterCreate { get; set; }
    public Func<CrudContext, Task> OnAfterCreateAsync { get; set; }

    public Action<CrudContext> OnBeforePatch { get; set; }
    public Func<CrudContext, Task> OnBeforePatchAsync { get; set; }
    public Action<CrudContext> OnAfterPatch { get; set; }
    public Func<CrudContext, Task> OnAfterPatchAsync { get; set; }

    public Action<CrudContext> OnBeforeUpdate { get; set; }
    public Func<CrudContext, Task> OnBeforeUpdateAsync { get; set; }
    public Action<CrudContext> OnAfterUpdate { get; set; }
    public Func<CrudContext, Task> OnAfterUpdateAsync { get; set; }

    public Action<CrudContext> OnBeforeDelete { get; set; }
    public Func<CrudContext, Task> OnBeforeDeleteAsync { get; set; }
    public Action<CrudContext> OnAfterDelete { get; set; }
    public Func<CrudContext, Task> OnAfterDeleteAsync { get; set; }

    protected void OnRegister(IServiceCollection services)
    {
        if (AccessRole != null && services.Exists<ICrudEvents>())
        {
            services.RegisterServices(ServiceRoutes);
        }
    }

    public static void AuditAutoCrudMetadataFilter(AutoCrudMetadata meta)
    {
        foreach (var applyAttr in meta.AutoApplyAttrs)
        {
            switch (applyAttr.Name)
            {
                case Behavior.AuditQuery:
                    meta.Add(new AutoFilterAttribute(
                        QueryTerm.Ensure, nameof(AuditBase.DeletedDate), SqlTemplate.IsNull));
                    break;
                case Behavior.AuditCreate:
                case Behavior.AuditModify:
                    if (applyAttr.Name == Behavior.AuditCreate)
                    {
                        meta.Add(new AutoPopulateAttribute(nameof(AuditBase.CreatedDate))
                        {
                            Eval = "utcNow"
                        });
                        meta.Add(new AutoPopulateAttribute(nameof(AuditBase.CreatedBy))
                        {
                            Eval = "userAuthName"
                        });
                    }

                    meta.Add(new AutoPopulateAttribute(nameof(AuditBase.ModifiedDate))
                    {
                        Eval = "utcNow"
                    });
                    meta.Add(new AutoPopulateAttribute(nameof(AuditBase.ModifiedBy))
                    {
                        Eval = "userAuthName"
                    });
                    break;
                case Behavior.AuditDelete:
                case Behavior.AuditSoftDelete:
                    if (applyAttr.Name == Behavior.AuditSoftDelete)
                        meta.SoftDelete = true;

                    meta.Add(new AutoPopulateAttribute(nameof(AuditBase.DeletedDate))
                    {
                        Eval = "utcNow"
                    });
                    meta.Add(new AutoPopulateAttribute(nameof(AuditBase.DeletedBy))
                    {
                        Eval = "userAuthName"
                    });
                    break;
            }
        }
    }
}

[DefaultRequest(typeof(GetCrudEvents))]
public partial class GetCrudEventsService(IAutoQueryDb autoQuery, IDbConnectionFactory dbFactory) : Service
{
    public async Task<object> Any(GetCrudEvents request)
    {
        var appHost = HostContext.AppHost;
        await RequestUtils.AssertAccessRoleAsync(base.Request, accessRole: autoQuery.AccessRole,
            authSecret: request.AuthSecret);

        if (string.IsNullOrEmpty(request.Model))
            throw new ArgumentNullException(nameof(request.Model));

        var dto = appHost.Metadata.FindDtoType(request.Model);
        var namedConnection = dto?.FirstAttribute<NamedConnectionAttribute>()?.Name;

        using var useDb = namedConnection != null
            ? await dbFactory.OpenDbConnectionAsync(namedConnection).ConfigAwait()
            : await dbFactory.OpenDbConnectionAsync().ConfigAwait();

        var q = autoQuery.CreateQuery(request, Request, useDb);
        var response = await autoQuery.ExecuteAsync(request, q, Request, useDb).ConfigAwait();

        // EventDate is populated in UTC but in some RDBMS (SQLite) it doesn't preserve UTC Kind, so we set it here
        foreach (var result in response.Results)
        {
            if (result.EventDate.Kind == DateTimeKind.Unspecified)
                result.EventDate = DateTime.SpecifyKind(result.EventDate, DateTimeKind.Utc);
        }

        return response;
    }
}

[DefaultRequest(typeof(CheckCrudEvents))]
public partial class CheckCrudEventService(IAutoQueryDb autoQuery, IDbConnectionFactory dbFactory) : Service
{
    public async Task<object> Any(CheckCrudEvents request)
    {
        var appHost = HostContext.AppHost;
        await RequestUtils.AssertAccessRoleAsync(base.Request, accessRole: autoQuery.AccessRole,
            authSecret: request.AuthSecret);

        if (string.IsNullOrEmpty(request.Model))
            throw new ArgumentNullException(nameof(request.Model));

        var ids = request.Ids?.Count > 0
            ? request.Ids
            : throw new ArgumentNullException(nameof(request.Ids));

        var dto = appHost.Metadata.FindDtoType(request.Model);
        var namedConnection = dto?.FirstAttribute<NamedConnectionAttribute>()?.Name;

        using var useDb = namedConnection != null
            ? await dbFactory.OpenDbConnectionAsync(namedConnection).ConfigAwait()
            : await dbFactory.OpenDbConnectionAsync().ConfigAwait();

        var q = useDb.From<CrudEvent>()
            .Where(x => x.Model == request.Model)
            .And(x => ids.Contains(x.ModelId))
            .SelectDistinct(x => x.ModelId);

        var results = await useDb.ColumnAsync<string>(q).ConfigAwait();
        return new CheckCrudEventsResponse
        {
            Results = results.ToList(),
        };
    }
}

public class CrudContext
{
    public ServiceStackHost AppHost { get; private set; }
    public IRequest Request { get; private set; }
    public IDbConnection Db { get; private set; }
    public ICrudEvents Events { get; private set; }
    public string Operation { get; set; }
    public object Dto { get; private set; }
    public Type ModelType { get; private set; }
    public Type RequestType { get; private set; }
    public Type ResponseType { get; private set; }
    public ModelDefinition ModelDef { get; private set; }
    public PropertyAccessor IdProp { get; private set; }
    public PropertyAccessor ResultProp { get; private set; }
    public PropertyAccessor CountProp { get; private set; }
    public PropertyAccessor RowVersionProp { get; private set; }

    public object Id { get; set; }

    public object Response { get; set; }

    public long? RowsUpdated { get; set; }

    public string NamedConnection { get; set; }

    internal void SetResult(AutoQuery.ExecValue result)
    {
        Id = result.Id;
        RowsUpdated = result.RowsUpdated;
    }

    internal GetMemberDelegate RequestIdGetter() =>
        TypeProperties.Get(RequestType).GetPublicGetter(ModelDef.PrimaryKey.Name);

    internal void ThrowPrimaryKeyRequiredForRowVersion() =>
        throw new NotSupportedException(
            $"Could not resolve Primary Key from '{RequestType.Name}' to be able to resolve RowVersion");

    public static CrudContext Create<Table>(IRequest request, IDbConnection db, object dto, string operation) =>
        Create(typeof(Table), request, db, dto, operation);

    public static CrudContext Create(Type tableType, IRequest request, IDbConnection db, object dto, string operation)
    {
        var appHost = HostContext.AppHost;
        var requestType = dto?.GetType() ?? throw new ArgumentNullException(nameof(dto));
        var responseType = appHost.Metadata.GetOperation(requestType)?.ResponseType;
        var responseProps = responseType == null ? null : TypeProperties.Get(responseType);
        return new CrudContext
        {
            AppHost = appHost,
            Operation = operation,
            Request = request ?? throw new ArgumentNullException(nameof(request)),
            Db = db ?? throw new ArgumentNullException(nameof(db)),
            NamedConnection = appHost.TryResolve<IAutoQueryDb>().GetDbNamedConnection(tableType),
            Events = appHost.TryResolve<ICrudEvents>(),
            Dto = dto,
            ModelType = tableType,
            RequestType = requestType,
            ModelDef = tableType.GetModelMetadata(),
            ResponseType = responseType,
            IdProp = responseProps?.GetAccessor(Keywords.Id),
            CountProp = responseProps?.GetAccessor(Keywords.Count),
            ResultProp = responseProps?.GetAccessor(Keywords.Result),
            RowVersionProp = responseProps?.GetAccessor(Keywords.RowVersion),
        };
    }
}

public class AutoCrudMetadata
{
    public Type DtoType { get; set; }
    public Type ModelType { get; set; }
    public ModelDefinition ModelDef { get; set; }
    public TypeProperties DtoProps { get; set; }
    public List<AutoPopulateAttribute> PopulateAttrs { get; set; } = new();
    public List<AutoFilterAttribute> AutoFilters { get; set; } = new();
    public List<QueryDbFieldAttribute> AutoFiltersDbFields { get; set; } = new();
    public List<AutoApplyAttribute> AutoApplyAttrs { get; set; } = new();
    public Dictionary<string, AutoUpdateAttribute> UpdateAttrs { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, AutoDefaultAttribute> DefaultAttrs { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, AutoMapAttribute> MapAttrs { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, ValidateAttribute> ValidateAttrs { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, InputInfo> MapInputs { get; set; } = new();
    public HashSet<string> NullableProps { get; set; } = new();
    public List<string> RemoveDtoProps { get; set; } = new();
    public GetMemberDelegate RowVersionGetter { get; set; }
    public bool SoftDelete { get; set; }
    public HashSet<string> DenyReset { get; set; } = new();

    static readonly ConcurrentDictionary<Type, AutoCrudMetadata> cache = new();

    internal static AutoCrudMetadata Create(Type dtoType, AutoQueryFeature feature)
    {
        if (cache.TryGetValue(dtoType, out var to))
            return to;

        to = new AutoCrudMetadata
        {
            DtoType = dtoType,
            ModelType = AutoCrudOperation.GetModelType(dtoType),
            DtoProps = TypeProperties.Get(dtoType),
        };
        if (to.ModelType != null)
            to.ModelDef = to.ModelType.GetModelMetadata();

        to.RowVersionGetter = to.DtoProps.GetPublicGetter(Keywords.RowVersion);

        var dtoAttrs = dtoType.AllAttributes();
        foreach (var dtoAttr in dtoAttrs)
        {
            if (dtoAttr is AutoPopulateAttribute populateAttr)
            {
                to.Add(populateAttr);
            }
            else if (dtoAttr is AutoFilterAttribute filterAttr)
            {
                to.Add(filterAttr);
            }
            else if (dtoAttr is AutoApplyAttribute applyAttr)
            {
                to.AutoApplyAttrs.Add(applyAttr);
            }
        }

        foreach (var pi in to.DtoProps.PublicPropertyInfos)
        {
            var allAttrs = pi.AllAttributes();
            var propName = pi.Name;

            if (allAttrs.FirstOrDefault(x => x is AutoMapAttribute) is AutoMapAttribute mapAttr)
            {
                to.Set(propName, mapAttr);
                propName = mapAttr.To;
            }

            if (allAttrs.FirstOrDefault(x => x is AutoUpdateAttribute) is AutoUpdateAttribute updateAttr)
            {
                to.Set(propName, updateAttr);
            }

            if (allAttrs.FirstOrDefault(x => x is AutoDefaultAttribute) is AutoDefaultAttribute defaultAttr)
            {
                to.Set(propName, defaultAttr);
            }

            if (allAttrs.FirstOrDefault(x => x is InputAttribute) is InputAttribute inputAttr)
            {
                to.Set(propName, inputAttr);
            }

            if (allAttrs.FirstOrDefault(x => x is ValidateAttribute) is ValidateAttribute validateAttr)
            {
                to.Set(propName, validateAttr);
            }

            // Deny resetting all properties with [Validate*] attrs without an explicit [AllowReset] attr  
            var allowReset = allAttrs.FirstOrDefault(x => x is AllowResetAttribute);
            var denyReset = allAttrs.FirstOrDefault(x => x is DenyResetAttribute) != null ||
                            (allowReset == null && allAttrs.Any(x => x is ValidateAttribute));
            if (denyReset)
            {
                to.DenyReset.Add(propName);
            }

            if (pi.PropertyType.IsNullableType())
            {
                to.AddNullableProperty(propName);
            }

            if (!AutoQuery.IncludeCrudProperties.Contains(propName))
            {
                var hasProp = to.ModelDef.GetFieldDefinition(propName) != null;
                if (!hasProp)
                {
                    var modelProp = to.ModelType.GetPublicProperties()
                        .FirstOrDefault(x => x.Name.Equals(propName, StringComparison.OrdinalIgnoreCase));
                    hasProp = modelProp?.FirstAttribute<ReferenceAttribute>() != null;
                }

                if (!hasProp
                    // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                    || (AutoQuery.IgnoreCrudProperties.Contains(pi.Name) && !hasProp)
                    || pi.HasAttribute<AutoIgnoreAttribute>())
                {
                    to.AddDtoPropertyToRemove(pi);
                }
            }
        }

        foreach (var fn in feature?.AutoCrudMetadataFilters.Safe())
        {
            fn(to);
        }

        return cache[dtoType] = to;
    }

    public bool HasAutoApply(string name) =>
        AutoApplyAttrs != null && AutoApplyAttrs.Any(x => x.Name == name);

    public void AddDtoPropertyToRemove(PropertyInfo pi)
    {
        RemoveDtoProps.Add(pi.Name);
    }

    public void AddNullableProperty(string propName)
    {
        NullableProps.Add(propName);
    }

    public void Set(string propName, AutoMapAttribute mapAttr)
    {
        MapAttrs[propName] = mapAttr;
    }

    public void Set(string propName, AutoDefaultAttribute defaultAttr)
    {
        DefaultAttrs[propName] = defaultAttr;
    }

    public void Set(string propName, AutoUpdateAttribute updateAttr)
    {
        UpdateAttrs[propName] = updateAttr;
    }

    public void Set(string propName, InputAttribute inputAttr)
    {
        MapInputs[propName] = inputAttr.ToInput();
    }

    public void Set(string propName, ValidateAttribute validateAttr)
    {
        ValidateAttrs[propName] = validateAttr;
    }

    public void Add(AutoPopulateAttribute populateAttr)
    {
        PopulateAttrs.Add(populateAttr);
    }

    public void Add(AutoFilterAttribute filterAttr)
    {
        AutoFilters.Add(filterAttr);
        AutoFiltersDbFields.Add(ExprResult.ToDbFieldAttribute(filterAttr));
    }
}

public abstract partial class AutoQueryServiceBase
{
    public virtual object Create<Table>(ICreateDb<Table> dto) => AutoQuery.Create(dto, Request);

    public virtual Task<object> CreateAsync<Table>(ICreateDb<Table> dto) => AutoQuery.CreateAsync(dto, Request);

    private static ConcurrentDictionary<Type, ObjectActivator> genericListCache = new();

    private static IList CreateGenericList<T>(Type responseType)
    {
        if (responseType == typeof(object))
            return new List<object>();

        var activator = genericListCache.GetOrAdd(responseType, type =>
            typeof(List<>).MakeGenericType(type).GetConstructor(Type.EmptyTypes).GetActivator());
        return (IList)activator(Array.Empty<object>());
    }

    private static Type GetResponseType(Type requestType)
    {
        if (requestType == null)
            return null;
        var responseType = requestType.GetInterfaces()
            .FirstOrDefault(x => x.IsOrHasGenericInterfaceTypeOf(typeof(IReturn<>)))?.GenericTypeArguments[0];
        responseType ??= HostContext.Metadata.GetResponseTypeByRequest(requestType);
        return responseType;
    }

    public virtual async Task<object> BatchCreateAsync<T>(IEnumerable<ICreateDb<T>> requests)
    {
        using var db = AutoQuery.GetDb<T>(Request);
        using var dbTrans = db.OpenTransaction();

        var list = requests.ToList();
        var results = CreateGenericList<T>(GetResponseType(list.FirstOrDefault()?.GetType()) ?? typeof(object));
        foreach (var request in list)
        {
            var response = await AutoQuery.CreateAsync(request, Request, db);
            results.Add(response);
        }

        dbTrans.Commit();
        return results;
    }

    public virtual object Update<Table>(IUpdateDb<Table> dto) => AutoQuery.Update(dto, Request);

    public virtual Task<object> UpdateAsync<Table>(IUpdateDb<Table> dto) => AutoQuery.UpdateAsync(dto, Request);

    public virtual async Task<object> BatchUpdateAsync<T>(IEnumerable<IUpdateDb<T>> requests)
    {
        using var db = AutoQuery.GetDb<T>(Request);
        using var dbTrans = db.OpenTransaction();

        var list = requests.ToList();
        var results = CreateGenericList<T>(GetResponseType(list.FirstOrDefault()?.GetType()) ?? typeof(object));
        foreach (var request in list)
        {
            var response = await AutoQuery.UpdateAsync(request, Request, db);
            results.Add(response);
        }

        dbTrans.Commit();
        return results;
    }

    public virtual object Patch<Table>(IPatchDb<Table> dto) => AutoQuery.Patch(dto, Request);

    public virtual Task<object> PatchAsync<Table>(IPatchDb<Table> dto) => AutoQuery.PatchAsync(dto, Request);

    public virtual async Task<object> BatchPatchAsync<T>(IEnumerable<IPatchDb<T>> requests)
    {
        using var db = AutoQuery.GetDb<T>(Request);
        using var dbTrans = db.OpenTransaction();

        var list = requests.ToList();
        var results = CreateGenericList<T>(GetResponseType(list.FirstOrDefault()?.GetType()) ?? typeof(object));
        foreach (var request in list)
        {
            var response = await AutoQuery.PartialUpdateAsync<T>(request, Request, db);
            results.Add(response);
        }

        dbTrans.Commit();
        return results;
    }

    public virtual object Delete<Table>(IDeleteDb<Table> dto) => AutoQuery.Delete(dto, Request);

    public virtual Task<object> DeleteAsync<Table>(IDeleteDb<Table> dto) => AutoQuery.DeleteAsync(dto, Request);

    public virtual async Task<object> BatchDeleteAsync<T>(IEnumerable<IDeleteDb<T>> requests)
    {
        using var db = AutoQuery.GetDb<T>(Request);
        using var dbTrans = db.OpenTransaction();

        var list = requests.ToList();
        var results = CreateGenericList<T>(GetResponseType(list.FirstOrDefault()?.GetType()) ?? typeof(object));
        foreach (var request in list)
        {
            var response = await AutoQuery.DeleteAsync(request, Request, db);
            results.Add(response);
        }

        dbTrans.Commit();
        return results;
    }

    public virtual object Save<Table>(ISaveDb<Table> dto) => AutoQuery.Save(dto, Request);

    public virtual Task<object> SaveAsync<Table>(ISaveDb<Table> dto) => AutoQuery.SaveAsync(dto, Request);

    public virtual async Task<object> BatchSaveAsync<T>(IEnumerable<ISaveDb<T>> requests)
    {
        using var db = AutoQuery.GetDb<T>(Request);
        using var dbTrans = db.OpenTransaction();

        var list = requests.ToList();
        var results = CreateGenericList<T>(GetResponseType(list.FirstOrDefault()?.GetType()) ?? typeof(object));
        foreach (var request in list)
        {
            var response = await AutoQuery.SaveAsync(request, Request, db);
            results.Add(response);
        }

        dbTrans.Commit();
        return results;
    }
}