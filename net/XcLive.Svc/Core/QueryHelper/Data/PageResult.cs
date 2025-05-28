namespace SchemaBuilder.Svc.Core.QueryHelper.Data;

public class PageResult<T>
{
    private List<T> _items = new();

    public List<T> Items
    {
        get => _items;
        set => _items = value;
    }

    public int TotalCount { get; set; }
}