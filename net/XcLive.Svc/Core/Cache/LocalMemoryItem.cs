namespace SchemaBuilder.Svc.Core.Cache;

public class LocalMemoryItem
{
    public LocalMemoryItem(string key, object value, DateTime expirationTime)
    {
        this.Key = key;
        this.Value = value;
        this.ExpirationTime = expirationTime;
    }

    public string Key { get; set; }

    public object Value { get; set; }

    public DateTime ExpirationTime { get; set; }

    public LocalMemoryItem Up { get; set; }

    public LocalMemoryItem Next { get; set; }

    public override string ToString() => string.Format("[{0}:{1}]", (object)this.Key, this.Value);
}