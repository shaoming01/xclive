using System.Data;

namespace SchemaBuilder.Svc.Core.QueryHelper.Data.@interface;

/// <summary>
///     统计接口
/// </summary>
public interface IStatistics
{
    /// <summary>
    ///     获得统计SQL
    /// </summary>
    /// <typeparam name="TS">统计类型</typeparam>
    /// <returns>统计SQL</returns>
    string ToStatisticsSql<TS>();

    /// <summary>
    ///     得到统计数据
    /// </summary>
    /// <param name="db">连接对象</param>
    /// <typeparam name="TS">统计对象</typeparam>
    /// <returns>统计对象</returns>
    TS Statistics<TS>(IDbConnection db);

    /// <summary>
    ///     得到统计数据
    /// </summary>
    /// <typeparam name="TS">统计对象</typeparam>
    /// <returns>统计对象</returns>
    TS Statistics<TS>();
}