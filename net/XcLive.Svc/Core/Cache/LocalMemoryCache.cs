namespace SchemaBuilder.Svc.Core.Cache;

public class LocalMemoryCache : ILocalMemoryCache
{
    private static readonly ReaderWriterLockSlim LockSlim = new ReaderWriterLockSlim();
    private readonly Dictionary<string, LocalMemoryItem> _memoryDictionary;
    private readonly TimeSpan _expirationTime;
    public LocalMemoryItem _top;
    public LocalMemoryItem _last;

    public TimeSpan ExpirationTime => this._expirationTime;

    public int Count => this._memoryDictionary != null ? this._memoryDictionary.Count : 0;

    public LocalMemoryCache(TimeSpan expirationTime)
    {
        this._expirationTime = expirationTime;
        this._memoryDictionary = new Dictionary<string, LocalMemoryItem>();
    }

    public object GetObject(string key)
    {
        LocalMemoryItem localMemoryItem = (LocalMemoryItem) null;
        LocalMemoryCache.LockSlim.EnterReadLock();
        try
        {
            this._memoryDictionary.TryGetValue(key, out localMemoryItem);
        }
        finally
        {
            LocalMemoryCache.LockSlim.ExitReadLock();
        }
        if (localMemoryItem == null)
            return (object) null;
        if (localMemoryItem.ExpirationTime >= DateTime.Now)
        {
            Log4.Log.DebugFormat("LocalMemoryCache:读取Key对应的值:查找到[{0}]这个Key", (object) key);
            return localMemoryItem.Value;
        }
        Log4.Log.DebugFormat("LocalMemoryCache:读取Key对应的值:查找到[{0}]这个Key已经过期", (object) key);
        this.Remove(key, true);
        return (object) null;
    }

    public void SetObject(string key, object value)
    {
        LocalMemoryCache.LockSlim.EnterWriteLock();
        try
        {
            int num = this.RemoveExpirationTimeItems();
            Log4.Log.DebugFormat("LocalMemoryCache:移除 {0} 个过期Key", (object) num);
            LocalMemoryItem localMemoryItem;
            if (this._memoryDictionary.TryGetValue(key, out localMemoryItem))
            {
                if (localMemoryItem.Up == null)
                    this._top = localMemoryItem.Next;
                else
                    localMemoryItem.Up.Next = localMemoryItem.Next;
                if (localMemoryItem.Next == null)
                    this._last = localMemoryItem.Up;
                else
                    localMemoryItem.Next.Up = localMemoryItem.Up;
                localMemoryItem.Up = (LocalMemoryItem) null;
                localMemoryItem.Next = (LocalMemoryItem) null;
                localMemoryItem.Value = value;
                localMemoryItem.ExpirationTime = DateTime.Now.Add(this._expirationTime);
                Log4.Log.DebugFormat("LocalMemoryCache:重新设置了[{0}]这个Key的值", (object) key);
            }
            else
            {
                localMemoryItem = new LocalMemoryItem(key, value, DateTime.Now.Add(this._expirationTime));
                this._memoryDictionary.Add(key, localMemoryItem);
                Log4.Log.DebugFormat("LocalMemoryCache:新增了[{0}]这个Key", (object) key);
            }
            if (this._last == null)
            {
                this._top = localMemoryItem;
                this._last = localMemoryItem;
            }
            else
            {
                localMemoryItem.Up = this._last;
                this._last.Next = localMemoryItem;
                this._last = localMemoryItem;
            }
        }
        finally
        {
            LocalMemoryCache.LockSlim.ExitWriteLock();
        }
    }

    public bool ContainsKey(string key)
    {
        LocalMemoryItem localMemoryItem = (LocalMemoryItem) null;
        LocalMemoryCache.LockSlim.EnterReadLock();
        try
        {
            this._memoryDictionary.TryGetValue(key, out localMemoryItem);
        }
        finally
        {
            LocalMemoryCache.LockSlim.ExitReadLock();
        }
        if (localMemoryItem == null)
        {
            Log4.Log.DebugFormat("LocalMemoryCache:判断Key是否存在:没有查找到[{0}]这个Key", (object) key);
            return false;
        }
        if (localMemoryItem.ExpirationTime >= DateTime.Now)
        {
            Log4.Log.DebugFormat("LocalMemoryCache:判断Key是否存在:查找到[{0}]这个Key", (object) key);
            return true;
        }
        Log4.Log.DebugFormat("LocalMemoryCache:判断Key是否存在:查找到[{0}]这个Key已经过期", (object) key);
        this.Remove(key, true);
        return false;
    }

    private int RemoveExpirationTimeItems()
    {
        int num = 0;
        LocalMemoryItem localMemoryItem = this._top;
        while (localMemoryItem != null && localMemoryItem.ExpirationTime < DateTime.Now)
        {
            ++num;
            this._memoryDictionary.Remove(localMemoryItem.Key);
            localMemoryItem = localMemoryItem.Next;
            if (localMemoryItem == null)
                this._last = (LocalMemoryItem) null;
            else
                localMemoryItem.Up = (LocalMemoryItem) null;
        }
        this._top = localMemoryItem;
        return num;
    }

    public bool Remove(string key) => this.Remove(key, false);

    public int RemoveStartWith(string key)
    {
        LocalMemoryCache.LockSlim.EnterWriteLock();
        try
        {
            LocalMemoryItem localMemoryItem1 = this._top;
            List<string> values = new List<string>();
            while (localMemoryItem1 != null)
            {
                LocalMemoryItem localMemoryItem2 = localMemoryItem1;
                localMemoryItem1 = localMemoryItem1.Next;
                if (localMemoryItem2.Key.StartsWith(key))
                {
                    if (localMemoryItem2.Up == null)
                        this._top = localMemoryItem2.Next;
                    else
                        localMemoryItem2.Up.Next = localMemoryItem2.Next;
                    if (localMemoryItem2.Next == null)
                        this._last = localMemoryItem2.Up;
                    else
                        localMemoryItem2.Next.Up = localMemoryItem2.Up;
                    values.Add(localMemoryItem2.Key);
                    this._memoryDictionary.Remove(localMemoryItem2.Key);
                }
            }
            Log4.Log.DebugFormat("LocalMemoryCache:移除键开头完全匹配[{0}]的Key,总共{1}个，分别为[{2}]", (object) key, (object) values.Count, (object) string.Join(",", (IEnumerable<string>) values));
            return values.Count;
        }
        finally
        {
            LocalMemoryCache.LockSlim.ExitWriteLock();
        }
    }

    private bool Remove(string key, bool expirationTime)
    {
        LocalMemoryCache.LockSlim.EnterWriteLock();
        try
        {
            LocalMemoryItem localMemoryItem;
            if (!this._memoryDictionary.TryGetValue(key, out localMemoryItem) || expirationTime && localMemoryItem.ExpirationTime > DateTime.Now)
                return false;
            if (localMemoryItem.Up == null)
                this._top = localMemoryItem.Next;
            else
                localMemoryItem.Up.Next = localMemoryItem.Next;
            if (localMemoryItem.Next == null)
                this._last = localMemoryItem.Up;
            else
                localMemoryItem.Next.Up = localMemoryItem.Up;
            Log4.Log.DebugFormat("LocalMemoryCache:移除[{0}]这个Key", (object) key);
            return this._memoryDictionary.Remove(localMemoryItem.Key);
        }
        finally
        {
            LocalMemoryCache.LockSlim.ExitWriteLock();
        }
    }

    public T GetObject<T>(string key)
    {
        object obj = this.GetObject(key);
        return obj == null ? default (T) : (T) obj;
    }

    public void SetObject<T>(string key, T value) => this.SetObject(key, (object) value);

    public void Clear()
    {
        LocalMemoryCache.LockSlim.EnterWriteLock();
        try
        {
            this._top = (LocalMemoryItem) null;
            this._last = (LocalMemoryItem) null;
            this._memoryDictionary.Clear();
        }
        finally
        {
            LocalMemoryCache.LockSlim.ExitWriteLock();
        }
    }
}