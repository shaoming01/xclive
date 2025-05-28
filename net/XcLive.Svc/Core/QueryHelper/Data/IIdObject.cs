using ServiceStack.DataAnnotations;

namespace SchemaBuilder.Svc.Core.QueryHelper.Data;

public interface IIdObject
{
    [PrimaryKey] long Id { get; set; }
}