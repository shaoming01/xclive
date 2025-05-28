using SchemaBuilder.Svc.Core.QueryHelper.Data;
using SchemaBuilder.Svc.Svc;

namespace SchemaBuilder.Svc.Core.QueryHelper;

public class PageQuerySqlBuilderNoLock<TQ, TR> : PageQuerySqlBuilder<TQ, TR>
    where TR : IIdObject
{
    public PageQuerySqlBuilderNoLock(PageQueryObject<TQ> pageQueryObject) : base(pageQueryObject)
    {
    }


    public override PageResult<TR> RetrievePage()
    {
        return Db.OpenDbWithNoLock(db => RetrievePage(db));
    }

    public override int TotalCount()
    {
        return Db.OpenDbWithNoLock(db => TotalCount(db));
    }

    public override TS Statistics<TS>()
    {
        return Db.OpenDbWithNoLock(db => Statistics<TS>(db));
    }
}