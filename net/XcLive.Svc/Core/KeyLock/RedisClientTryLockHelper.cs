using ServiceStack.Redis;

namespace SchemaBuilder.Svc.Core.KeyLock;

public static class RedisClientTryLockHelper
{
    public static ILockKeyCore TryLock(
        this PooledRedisClientManager clientManager,
        string key,
        TimeSpan? maxAge,
        string keyPreFix = "LOCK_")
    {
        IRedisClient redisClient = (IRedisClient)null;
        try
        {
            redisClient = clientManager.GetClient();
            DateTime now = DateTime.Now;
            string key1 = keyPreFix + key;
            string lockValue = now.Ticks.ToString();
            TimeSpan timeSpan = maxAge ?? new TimeSpan(1, 0, 0, 0);
            if (redisClient.SetValueIfNotExists(key1, lockValue, new TimeSpan(0, 0, (int)timeSpan.TotalSeconds)))
                return (ILockKeyCore)new LockKey(clientManager, key, lockValue, keyPreFix);
        }
        finally
        {
            redisClient?.Dispose();
        }

        return (ILockKeyCore)null;
    }
}