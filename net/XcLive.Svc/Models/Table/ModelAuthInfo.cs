using ServiceStack.DataAnnotations;

namespace SchemaBuilder.Svc.Models.Table;

public class ModelAuthInfo : ITable, ITenantId
{
    [PrimaryKey] public long Id { get; set; }
    public long TenantId { get; set; }
    public ModelPlatformType PlatformType { get; set; }
    public string? Name { get; set; }
    public string? EndPoint { get; set; }
    public string? ApiKey { get; set; }
    public string? TextModelId { get; set; }
    public string? ImageModelId { get; set; }
}

[Flags]
public enum ModelPlatformType
{
    Doubao = 1,
}