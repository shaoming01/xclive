namespace SchemaBuilder.Svc.Core.KeyLock;

public class KeyLockItem : IDisposable
{
    private readonly string _key;
    private readonly KeyLock _lockContainer;

    public KeyLockItem(KeyLock lockContainer, string key)
    {
        this._lockContainer = lockContainer;
        this._key = key;
    }

    public void Dispose() => this._lockContainer.UnLock(this._key);
}