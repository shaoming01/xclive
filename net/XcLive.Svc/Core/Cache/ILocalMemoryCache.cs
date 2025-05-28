namespace SchemaBuilder.Svc.Core.Cache;

public interface ILocalMemoryCache
{
    TimeSpan ExpirationTime { get; }

    int Count { get; }

    object GetObject(string key);

    void SetObject(string key, object value);

    bool ContainsKey(string key);

    bool Remove(string key);

    int RemoveStartWith(string key);

    T GetObject<T>(string key);

    void SetObject<T>(string key, T value);

    void Clear();
}