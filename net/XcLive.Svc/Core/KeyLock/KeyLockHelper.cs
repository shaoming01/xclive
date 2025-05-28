using ServiceStack.Redis;

namespace SchemaBuilder.Svc.Core.KeyLock
{
    /// <summary>
    /// 存储所有业务键锁
    /// </summary>
    public class KeyLockHelper
    {
        public static IKeyLock ModelUseLock;
        public static IKeyLock ModelLoadLock;
        public static IKeyLock ModelUsageUpdatelock;


        public static void IniRedisKeyLock(string redis)
        {
            var fac = new PooledRedisClientManager(100 /*连接池个数*/, 10 /*连接池超时时间*/, redis);
            RedisConfig.VerifyMasterConnections = false; //需要设置
            var timeout = new TimeSpan(0, 3, 0);
            var age = new TimeSpan(0, 2, 0);


            ModelUseLock =
                new RedisKeyLock(fac, timeout, new TimeSpan(0, 3, 0), "ModelUseLock:"); //使用锁，同个模型，同时只允许1人使用，省得争
            ModelLoadLock = new RedisKeyLock(fac, timeout, age, "ModelLoadLock:");
            ModelUsageUpdatelock = new RedisKeyLock(fac, timeout, age, "ModelUsageUpdateLock:");
        }

        public static void IniMemKeyLock()
        {
            ModelUseLock = new KeyLock();
            ModelLoadLock = new KeyLock();
            ModelUsageUpdatelock = new KeyLock();
        }
    }
}