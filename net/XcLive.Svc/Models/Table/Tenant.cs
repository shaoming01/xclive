namespace SchemaBuilder.Svc.Models.Table;

public class Tenant : ITable
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public string? ProductName { get; set; }
    public string? ProductVersion { get; set; }
}