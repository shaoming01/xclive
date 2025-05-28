using ServiceStack.DataAnnotations;

namespace SchemaBuilder.Svc.Models.Table;

public class SimulateRedis : ITable
{
    [PrimaryKey, Index] public string Key { get; set; }
    [StringLength(16000)] public string Value { get; set; }
    public long Id { get; set; }
}