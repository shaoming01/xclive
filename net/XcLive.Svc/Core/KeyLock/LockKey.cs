using ServiceStack.Redis;

namespace SchemaBuilder.Svc.Core.KeyLock;

public class LockKey : ILockKeyCore, IDisposable
{
    private readonly PooledRedisClientManager _clientManager;
    private readonly string _lockKey;
    private readonly string _lockValue;

    private const string UnlockScript =
        "\r\nlocal val1=redis.call('get', ARGV[1])\r\nif val1 then\r\n    if val1 and ARGV[2]==val1 then \r\n        return redis.call('del', ARGV[1])\r\n    else\r\n        return 0 \r\n    end\r\nend\r\nreturn -1\r\n";

    public LockKey(
        PooledRedisClientManager clientManager,
        string key,
        string lockValue,
        string keyPrefix = "LOCK_")
    {
        this.Key = key;
        this._lockKey = keyPrefix + key;
        this._lockValue = lockValue;
        this._clientManager = clientManager;
    }

    public override string ToString()
    {
        return string.Format("RedisDlmLock:{0}:{1}", (object)this._lockKey, (object)this._lockValue);
    }

    public void Dispose()
    {
        for (int index = 0; index < 3 && !this.ReleaseKey(this._lockKey, this._lockValue); ++index)
            Thread.Sleep(index * 100);
    }

    private bool ReleaseKey(string lockKey, string lockValue)
    {
        IRedisClient redisClient = (IRedisClient)null;
        try
        {
            redisClient = this._clientManager.GetClient();
            long num = ((RedisClient)redisClient).ExecLuaAsInt(
                "\r\nlocal val1=redis.call('get', ARGV[1])\r\nif val1 then\r\n    if val1 and ARGV[2]==val1 then \r\n        return redis.call('del', ARGV[1])\r\n    else\r\n        return 0 \r\n    end\r\nend\r\nreturn -1\r\n",
                new string[2]
                {
                    lockKey,
                    lockValue
                });
            if (num > 0L)
                ;
            return num != 0L;
        }
        catch (Exception ex)
        {
            return false;
        }
        finally
        {
            redisClient?.Dispose();
        }
    }

    public string Key { get; private set; }
}