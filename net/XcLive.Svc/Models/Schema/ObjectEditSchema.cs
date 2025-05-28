namespace SchemaBuilder.Svc.Models.Schema;

public class ObjectEditSchema
{
    public object data { get; set; }
    public List<EditFieldSchema> fields { get; set; }
    public DetailTablesSchema? detailTablesSchema { get; set; }
}

/// <summary>
/// 对象编辑器交互结构
/// </summary>
public class ModalObjectEditSchema
{
    public ObjectEditSchema ObjectEditSchema { get; set; }
    public string SaveDataUrl { get; set; }
    public string GetDataUrl { get; set; }
    public bool Centered { get; set; }
    public string Title { get; set; }
    public int SizeMode { get; set; }
}

public class ModalDataSelectSchema
{
    public DataBrowserSchema DataBrowserSchema { get; set; }
    public string Title { get; set; }
    public int SizeMode { get; set; }
}