using System.Globalization;
using SchemaBuilder.Svc.Core.QueryHelper.Data;
using SchemaBuilder.Svc.Helpers;

namespace SchemaBuilder.Svc.Core.Cache;

/// <summary>
/// 本地缓存作为二级缓存
/// </summary>
public class MixCache : ISvcCacheUtil
{
    private readonly int _mins;
    private readonly ICache _cache;
    private readonly LocalMemoryCache _memoryCache;

    public MixCache(string? redisConn, int mins = 5, int memorySeconds = 5)
    {
        _mins = mins;
        if (redisConn.Has())
        {
            _cache = new RedisCached(redisConn);
        }
        else
        {
            _cache = new DbRedisCached();
        }

        _memoryCache = new LocalMemoryCache(new TimeSpan(0, 0, 0, memorySeconds)); //初始化内存缓存
    }

    public T Get<T>(long tenantId, long id)
        where T : class, IIdObject
    {
        string key = id.ToString(CultureInfo.InvariantCulture).ToLower();
        var fullKey = CalcFullKey<T>(tenantId, key);
        var obj = _memoryCache.GetObject<T>(fullKey); //尝试在内存获取
        if (obj != null) return obj;
        obj = _cache.GetObject<T>(fullKey);
        /*if (obj == null)
        {
            DetachedCriteria dc = DetachedCriteria.For(typeof(T));
            dc.Add(Restriction.Eq("Id", id));
            using (var db = DbManager.OpenDb(tenantId))
            {
                obj = db.SingleById<T>(id);
                _cache.SetObject(obj, fullKey, new TimeSpan(0, _mins, 0));
            }
        }

        _memoryCache.SetObject(fullKey, obj);//将当前值存入内存缓存*/
        return obj;
    }

    private string CalcFullKey(long tenantId, string key)
    {
        return tenantId + ":" + key;
    }

    private string CalcFullKey<T>(long tenantId, string key)
    {
        return tenantId + ":" + key + ":" + typeof(T).FullName;
    }

    private string CalcFullKey<T>(long tenantId)
    {
        return tenantId + ":" + typeof(T).FullName;
    }

    public T Get<T>(long areaId, string key)
        where T : class
    {
        var fullKey = CalcFullKey<T>(areaId, key);
        var obj = _memoryCache.GetObject<T>(fullKey); //尝试在内存获取
        if (obj != null) return obj;
        obj = _cache.GetObject<T>(fullKey);
        if (obj != null)
            _memoryCache.SetObject(fullKey, obj); //将当前值存入内存缓存
        return obj;
    }

    public void SetKey<T>(long areaId, string key, T obj)
        where T : class
    {
        var fullKey = CalcFullKey<T>(areaId, key);
        _cache.SetObject(obj, fullKey, new TimeSpan(0, _mins, 0));
        _memoryCache.SetObject(fullKey, obj); //将当前值存入内存缓存
    }

    public void Clear(long areaId)
    {
        var fullKey = areaId + ":*";
        _cache.Clear(fullKey);
        _memoryCache.RemoveStartWith(string.Format("{0}:", areaId)); //清除在内存中的这些租户的Key
    }

    public void Clear(long areaId, string key)
    {
        var fullKey = CalcFullKey(areaId, key == null || key.EndsWith("*") ? key : key + "*");
        _cache.Clear(fullKey);
        _memoryCache.RemoveStartWith(fullKey.TrimEnd('*')); //清除在内存中的这些租户的Key
    }

    public T[] GetObjects<T>(long tenantId, string keyName)
    {
        var fullKey = CalcFullKey<T>(tenantId, keyName);
        T[] objs = _memoryCache.GetObject<T[]>(fullKey); //尝试在内存获取
        if (objs != null) return objs;
        objs = _cache.GetObject<T[]>(fullKey);
        if (objs != null)
            _memoryCache.SetObject(fullKey, objs);
        return objs;
    }
}