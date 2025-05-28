namespace SchemaBuilder.Svc.Models.Schema;

public class DetailTableSchema
{
    public string tab { get; set; }
    public string field { get; set; }
    public FullTableSchema tableSchema { get; set; }
}