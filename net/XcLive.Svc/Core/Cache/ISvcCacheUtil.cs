using SchemaBuilder.Svc.Core.QueryHelper.Data;

namespace SchemaBuilder.Svc.Core.Cache;

public interface ISvcCacheUtil
{
    T Get<T>(long tenantId, long id)
        where T : class, IIdObject;

    T Get<T>(long areaId, string key)
        where T : class;

    void SetKey<T>(long areaId, string key, T obj)
        where T : class;

    void Clear(long areaId);
    void Clear(long areaId, string key);
    T[] GetObjects<T>(long tenantId, string keyName);
}