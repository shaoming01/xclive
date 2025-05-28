namespace SchemaBuilder.Svc.Core.KeyLock;

public interface IKeyLock
{
    IDisposable Lock(string key);

    /// <summary>Lock失败返回NULL</summary>
    /// <param name="key"></param>
    /// <returns>失败返回NULL</returns>
    ILockKeyCore TryLock(string key);
}