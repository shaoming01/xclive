using Newtonsoft.Json;
using SchemaBuilder.Svc.Models.Table;
using SchemaBuilder.Svc.Svc;
using ServiceStack.OrmLite;

namespace SchemaBuilder.Svc.Core.Cache;

/// <summary>
/// 用于没有Redis临时使用情况下
/// </summary>
public class DbRedisCached : ICache
{
    public bool SetObject<T>(T obj, string id, TimeSpan lifeSpan = default(TimeSpan))
    {
        using var db = Db.Open();
        db.Delete<SimulateRedis>(c => c.Key == id);
        var model = new SimulateRedis
        {
            Key = id,
            Value = JsonConvert.SerializeObject(obj),
        };
        db.Save(model);

        return true;
    }

    public T GetObject<T>(string id) where T : class
    {
        using var db = Db.Open();
        var model = db.Select<SimulateRedis>(c => c.Key == id).FirstOrDefault();
        if (model == null)
        {
            return null;
        }

        return JsonConvert.DeserializeObject<T>(model.Value);
    }

    public int Clear(string keyName)
    {
        using var db = Db.Open();
        return db.Delete<SimulateRedis>(c => c.Key == keyName);
    }
}