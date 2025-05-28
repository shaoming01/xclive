using SchemaBuilder.Svc.Core.Cache;
using ServiceStack.Redis;

namespace SchemaBuilder.Svc.Core.KeyLock;

public class RedisLocker : IDisposable
{
    public static readonly TimeSpan DefaultLockAcquisitionTimeout = TimeSpan.FromSeconds(30.0);
    public static readonly TimeSpan DefaultLockMaxAge = TimeSpan.FromHours(2.0);
    public static Random R = new Random();
    private readonly IRedisClient _client;
    private readonly string _lockKey;
    private readonly string _lockValue;
    private readonly bool _lockSuccess;

    private const string UnlockScript =
        "\r\nlocal val1=redis.call('get', ARGV[1])\r\nif val1 then\r\n    if val1 and ARGV[2]==val1 then \r\n        return redis.call('del', ARGV[1])\r\n    else\r\n        return 0 \r\n    end\r\nend\r\nreturn -1\r\n";

    public RedisLocker(
        PooledRedisClientManager clientManager,
        string key,
        TimeSpan? acquisitionTimeOut = null,
        TimeSpan? lockMaxAge = null,
        string keyPrefix = "LOCK_")
    {
        try
        {
            this.Key = key;
            DateTime now = DateTime.Now;
            this._lockKey = keyPrefix + key;
            this._lockValue = now.Ticks.ToString();
            lockMaxAge = new TimeSpan?(lockMaxAge ?? RedisLocker.DefaultLockMaxAge);
            TimeSpan timeSpan = acquisitionTimeOut ?? RedisLocker.DefaultLockAcquisitionTimeout;
            this._client = clientManager.GetClient();
            int num = 1;
            do
            {
                this._lockSuccess = this._client.SetValueIfNotExists(this._lockKey, this._lockValue,
                    new TimeSpan(0, 0, (int)lockMaxAge.Value.TotalSeconds));
                if (!this._lockSuccess)
                {
                    ++num;
                    Thread.Sleep(RedisLocker.R.Next(1, 100));
                }
                else
                    break;
            } while (!(DateTime.Now - now > timeSpan));

            if (num <= 1)
                return;
            Log4.Log.Warn((object)string.Format("锁{0}重试了{1}耗时{2}ms", (object)this._lockKey, (object)num,
                (object)(DateTime.Now - now).TotalMilliseconds));
        }
        catch (Exception ex)
        {
            if (this._client != null)
                this._client.Dispose();
            throw;
        }
    }

    public override string ToString()
    {
        return string.Format("RedisDlmLock:{0}:{1}", (object)this._lockKey, (object)this._lockValue);
    }

    public void Dispose()
    {
        try
        {
            if (!this._lockSuccess)
                return;
            for (int index = 0; index < 3 && !this.ReleaseKey(this._lockKey, this._lockValue); ++index)
                Thread.Sleep(index * 100);
        }
        finally
        {
            if (this._client != null)
                this._client.Dispose();
        }
    }

    private bool ReleaseKey(string lockKey, string lockValue)
    {
        try
        {
            long num = ((RedisClient)this._client).ExecLuaAsInt(
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
    }

    public string Key { get; private set; }
}