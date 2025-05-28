namespace SchemaBuilder.Svc.Core.KeyLock;

/// <summary>
///     以键为单位的锁
///     控制域为对象的生存域
/// </summary>
public class KeyLock : IKeyLock
{
    private static readonly ReaderWriterLockSlim RwLock = new ReaderWriterLockSlim();
    private readonly Dictionary<string, int> _lockedKeys = new Dictionary<string, int>();

    /// <summary>
    ///     锁键,直到得到锁才返回
    ///     同一线程支持子锁
    /// </summary>
    /// <param name="key"></param>
    public IDisposable Lock(string key)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));
        int managedThreadId = Thread.CurrentThread.ManagedThreadId;
        bool flag;
        while (true)
        {
            KeyLock.RwLock.EnterWriteLock();
            int num;
            flag = this._lockedKeys.TryGetValue(key, out num);
            if (flag && num != managedThreadId)
            {
                KeyLock.RwLock.ExitWriteLock();
                Thread.Sleep(10);
            }
            else
                break;
        }

        if (!flag)
        {
            this._lockedKeys.Add(key, managedThreadId);
            KeyLock.RwLock.ExitWriteLock();
            return (IDisposable)new KeyLockItem(this, key);
        }

        KeyLock.RwLock.ExitWriteLock();
        return (IDisposable)null;
    }

    public ILockKeyCore TryLock(string key) => throw new NotImplementedException();

    /// <summary>
    ///     尝试锁
    ///     3秒内无法锁定,则返回false
    /// </summary>
    /// <param name="key"></param>
    /// <param name="timeoutSeconds">超时秒数</param>
    /// <param name="cycleCheckMiniSecond">循环检查是否已经可重新锁定的毫秒数</param>
    /// <returns></returns>
    public bool TryLock(string key, int timeoutSeconds = 3, int cycleCheckMiniSecond = 100)
    {
        int num1 = 0;
        int managedThreadId = Thread.CurrentThread.ManagedThreadId;
        bool flag;
        while (true)
        {
            KeyLock.RwLock.EnterWriteLock();
            int num2;
            flag = this._lockedKeys.TryGetValue(key, out num2);
            if (flag && num2 != managedThreadId)
            {
                KeyLock.RwLock.ExitWriteLock();
                if (num1++ < timeoutSeconds * 10)
                    Thread.Sleep(cycleCheckMiniSecond);
                else
                    goto label_5;
            }
            else
                break;
        }

        if (!flag)
        {
            this._lockedKeys.Add(key, managedThreadId);
            KeyLock.RwLock.ExitWriteLock();
            return true;
        }

        KeyLock.RwLock.ExitWriteLock();
        return true;
        label_5:
        return false;
    }

    /// <summary>解除锁</summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool UnLock(string key)
    {
        KeyLock.RwLock.EnterWriteLock();
        bool flag = this._lockedKeys.Remove(key);
        KeyLock.RwLock.ExitWriteLock();
        if (!flag)
            throw new Exception(string.Format("解锁KEY:{0}失败,程序逻辑有问题,如果该锁是由本线程加的,则只能本线程才能解锁,本线程使用完成必须解除锁", (object)key));
        return true;
    }
}