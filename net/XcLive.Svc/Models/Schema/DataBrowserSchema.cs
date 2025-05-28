namespace SchemaBuilder.Svc.Models.Schema;

public class DataBrowserSchema
{
    public SearchContainerSchema? searchContainer { get; set; }
    public FullTableSchema? mainTable { get; set; }
    public DetailTablesSchema? detailTablesSchema { get; set; }
}

public class DataBrowserProp
{
    public DataBrowserSchema schema { get; set; }
}

public class ObjectEditorProp
{
    public ObjectEditSchema schema { get; set; }
}

public class ModalObjectEditorProp
{
    public ModalObjectEditSchema schema { get; set; }
}

public class ModalDataSelectProp
{
    public ModalDataSelectSchema schema { get; set; }
}