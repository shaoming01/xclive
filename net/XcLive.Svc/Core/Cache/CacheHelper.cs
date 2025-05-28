namespace SchemaBuilder.Svc.Core.Cache;

public class CacheHelper
{
    static MixCache _cache;

    public static MixCache GetCacheInstance()
    {
        return _cache;
    }

    public static void Ini(string? redisConn, int mins = 1440)
    {
        _cache = new MixCache(redisConn, mins);
    }

    public static T? Get<T>(long areaId, string key)
        where T : class
    {
        return _cache.Get<T>(areaId, key);
    }

    public static void SetKey<T>(long areaId, string key, T obj)
        where T : class
    {
        _cache.SetKey(areaId, key, obj);
    }

    public static void Clear(long areaId)
    {
        _cache.Clear(areaId);
    }

    public static void Clear(long areaId, string key)
    {
        _cache.Clear(areaId, key);
    }
}