namespace SchemaBuilder.Svc.Core.Cache;

public interface ICache
{
    bool SetObject<T>(T obj, string id, TimeSpan lifeSpan = default(TimeSpan));

    T GetObject<T>(string id) where T : class;

    int Clear(string keyName);
}