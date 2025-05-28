namespace SchemaBuilder.Svc.Models.Schema;

public class TableColumnSchema
{
    public string field { get; set; }
    public string headerName { get; set; }
    public int width { get; set; }
    public string? tip { get; set; }
    public bool editable { get; set; }
    public string propertyType { get; set; }
    public CellRender? cellRender { get; set; }
    public ValueGetter? valueGetter { get; set; }
}

public class CellRender
{
    public string ComPath { get; set; }
    public bool CanEdit { get; set; }
    public Dictionary<string, object>? Props { get; set; }
}

public class ValueGetter
{
    public string FuncName { get; set; }
    public Dictionary<string, object>? Params { get; set; }
}