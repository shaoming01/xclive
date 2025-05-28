using ServiceStack.Redis;

namespace SchemaBuilder.Svc.Core.Cache;

public class RedisCached : ICache
{
    private readonly PooledRedisClientManager _redisPoolManager;
    public TimeSpan LifeSpan = new TimeSpan(0, 5, 0);

    public RedisCached(string connStr)
    {
        _redisPoolManager = new PooledRedisClientManager(30, 10, new string[1]
        {
            connStr
        });
        RedisConfig.VerifyMasterConnections = false;
        Log4.Log.Info((object)("初始化缓存实例:" + connStr));
    }

    public bool SetObject<T>(T obj, string id, TimeSpan lifeSpan = default(TimeSpan))
    {
        if (lifeSpan == new TimeSpan())
            lifeSpan = this.LifeSpan;
        using (IRedisClient client = this._redisPoolManager.GetClient())
        {
            bool flag = client.Set(id, obj, lifeSpan);
            Log4.Log.Debug("RedisSet:" + id);
            return flag;
        }
    }

    public T GetObject<T>(string id) where T : class
    {
        using (IRedisClient client = this._redisPoolManager.GetClient())
        {
            T obj = client.Get<T>(id);
            Log4.Log.Debug("RedisGet:" + id);
            return obj;
        }
    }

    public int Clear(string keyName)
    {
        using (IRedisClient client = this._redisPoolManager.GetClient())
        {
            List<string> keys = client.SearchKeys(keyName);
            client.RemoveAll(keys);
            Log4.Log.Debug("RedisClear:" + keyName);
            return keys.Count;
        }
    }
}