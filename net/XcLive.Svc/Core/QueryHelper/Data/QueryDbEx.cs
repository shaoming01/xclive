using ServiceStack;

namespace SchemaBuilder.Svc.Core.QueryHelper.Data;

public abstract class QueryDbEx<T> : QueryDb<T>
{
    public long[]? Ids { get; set; }
}

public abstract class QueryDbEx<From, Into> : QueryDb<From, Into>
{
    public long[]? Ids { get; set; }
}