namespace SchemaBuilder.Svc.Core.KeyLock;

public interface ILockKeyCore : IDisposable
{
    string Key { get; }
}