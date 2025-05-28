using ServiceStack.DataAnnotations;

namespace SchemaBuilder.Svc.Models.Table;

public class TtsModel : ITenantId, ITable
{
    [StringLength(128)] public string? Name { get; set; }
    [StringLength(256)] public string? ConfigPath { get; set; }
    [StringLength(256)] public string? ModelPath { get; set; }

    public long TenantId { get; set; }
    public long Id { get; set; }
}