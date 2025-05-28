using SchemaBuilder.Svc.Models.Table;
using ServiceStack.OrmLite;

namespace SchemaBuilder.Svc.Svc;

public abstract class CommonSvc
{
    public static T Save<T>(T item)
        where T : class, ITable
    {
        using var db = Db.Open();
        if (item.Id == 0)
        {
            item.Id = Id.NewId();
            db.Insert(item);
        }

        db.Save(item);
        return item;
    }

    public static bool Delete<T>(long id)
        where T : class, ITable
    {
        using var db = Db.Open();
        db.DeleteById<T>(id);
        return true;
    }

    public static T Get<T>(long id)
    {
        using var db = Db.Open();
        var re = db.SingleById<T>(id);
        return re;
    }
}