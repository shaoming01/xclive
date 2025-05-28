namespace SchemaBuilder.Svc.Models.Schema;

public class SearchContainerSchema
{
    public SearchGroupSchema? searchGroup { get; set; }
    public List<EditFieldSchema> fields { get; set; }
}