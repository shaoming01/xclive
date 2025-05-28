namespace SchemaBuilder.Svc.Models.Schema;

public class FullTableSchema
{
    public string? queryDataUrl { get; set; }
    public string? queryCountUrl { get; set; }
    public string? deleteIdsUrl { get; set; }
    public string? pageSizeOptions { get; set; }
    public string? rowSelection { get; set; }
    public bool showPageBar { get; set; }
    public int pageSize { get; set; }
    public int page { get; set; }
    public string? primaryKey { get; set; }
    public string? headerKey { get; set; }
    public bool? autoQuery { get; set; }
    public List<TableColumnSchema> columns { get; set; }
    public List<TableToolBarItemSchema> tableTools { get; set; }
}