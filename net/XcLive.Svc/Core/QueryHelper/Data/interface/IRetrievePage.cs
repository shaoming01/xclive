using System.Data;

namespace SchemaBuilder.Svc.Core.QueryHelper.Data.@interface;

/// <summary>
///     分页查询接口
/// </summary>
public interface IRetrievePage<TR> where TR : IIdObject
{
    /// <summary>
    ///     查询当前页数据
    /// </summary>
    /// <param name="db">数据库连接对象</param>
    /// <returns>数据</returns>
    PageResult<TR> RetrievePage(IDbConnection db);

    /// <summary>
    ///     查询当前页数据
    /// </summary>
    /// <returns>数据</returns>
    PageResult<TR> RetrievePage();
}