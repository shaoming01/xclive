using SchemaBuilder.Svc.Core.QueryHelper.Data;
using ServiceStack.DataAnnotations;

namespace SchemaBuilder.Svc.Models.Table;

public interface ITable : IIdObject
{
}

public interface ISettingObject
{
}

public interface IDetailTable : ITable
{
    [Index] long HeaderId { get; set; }
}

/// <summary>
/// 归属用户，实现该接口查询数据时会自动添加条件：UserId = @UserId
/// </summary>
public interface IUserId
{
    long UserId { get; set; }
}

/// <summary>
/// 归属租户，实现该接口查询数据时会自动添加条件：TenantId = @TenantId
/// </summary>
public interface ITenantId
{
    long TenantId { get; set; }
}

/// <summary>
/// 查询归属用户，实现该接口查询数据时会自动添加条件：UserId = @UserId
/// </summary>
public interface IUserIdQuery
{
    long? UserId { get; set; }
}

/// <summary>
/// 查询归属租户，实现该接口查询数据时会自动添加条件：TenantId = @TenantId
/// </summary>
public interface ITenantIdQuery
{
    long? TenantId { get; set; }
}