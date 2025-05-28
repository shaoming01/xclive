using System.Data;

namespace SchemaBuilder.Svc.Core.QueryHelper.Data.@interface;

/// <summary>
///     查询总数接口
/// </summary>
public interface ITotalCount
{
    /// <summary>
    ///     获取当前查询条件下的总数
    /// </summary>
    /// <param name="db">数据库连接对象</param>
    /// <returns>总数</returns>
    int TotalCount(IDbConnection db);

    /// <summary>
    ///     获取当前查询条件下的总数
    /// </summary>
    /// <returns>总数</returns>
    int TotalCount();
}