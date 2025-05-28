using ServiceStack.Redis;

namespace SchemaBuilder.Svc.Core.KeyLock;

public static class RedisClientLockExtensions
{
    public static IDisposable AcquireLock(
        this PooledRedisClientManager clientManager,
        string key,
        TimeSpan timeOut,
        TimeSpan maxAge,
        string keyPreFix)
    {
        return (IDisposable)new RedisLocker(clientManager, key, new TimeSpan?(timeOut), new TimeSpan?(maxAge),
            keyPreFix);
    }
}