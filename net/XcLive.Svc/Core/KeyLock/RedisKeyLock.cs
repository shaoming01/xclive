using SchemaBuilder.Svc.Core.Cache;
using ServiceStack.Redis;

namespace SchemaBuilder.Svc.Core.KeyLock;

public class RedisKeyLock : IKeyLock
{
    private readonly PooledRedisClientManager _clientManager;
    private readonly TimeSpan _timeOut;
    private readonly TimeSpan _lockAge;
    private readonly string _preFix;

    public RedisKeyLock(
        PooledRedisClientManager clientManager,
        TimeSpan? timeOut,
        TimeSpan? lockAge,
        string preFix = "")
    {
        this._clientManager = clientManager;
        this._timeOut = timeOut ?? TimeSpan.FromSeconds(30.0);
        this._lockAge = lockAge ?? TimeSpan.FromSeconds(30.0);
        this._preFix = preFix;
        Log4.Log.Info((object)("初始化RedisKeyLock实例:" + preFix));
    }

    public IDisposable Lock(string key)
    {
        try
        {
            IDisposable disposable = this._clientManager.AcquireLock(key, this._timeOut, this._lockAge, this._preFix);
            Log4.Log.Debug((object)("RedisLock:" + key));
            return disposable;
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("timeout"))
                return (IDisposable)null;
            throw;
        }
    }

    public ILockKeyCore TryLock(string key)
    {
        ILockKeyCore lockKeyCore = this._clientManager.TryLock(key, new TimeSpan?(this._lockAge), this._preFix);
        Log4.Log.Debug((object)("RedisTryLock:" + key + ":" + lockKeyCore?.ToString()));
        return lockKeyCore;
    }
}