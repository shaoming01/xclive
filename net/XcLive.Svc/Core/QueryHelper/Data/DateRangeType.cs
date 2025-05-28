namespace SchemaBuilder.Svc.Core.QueryHelper.Data;

/// <summary>
/// 日期范围类型
/// </summary>
public enum DateRangeType
{
    /// <summary>
    /// 使用范围
    /// </summary>
    None = 0,

    /// <summary>
    /// 今天
    /// </summary>
    Today = 1,

    /// <summary>
    /// 昨天
    /// </summary>
    Yesterday = 2,

    /// <summary>
    /// 3天内
    /// </summary>
    WithinThreeDays = 3,

    /// <summary>
    /// 7天内
    /// </summary>
    WithinSevenDays = 4,

    /// <summary>
    /// 本周
    /// </summary>
    ThisWeek = 5,

    /// <summary>
    /// 上周
    /// </summary>
    LastWeek = 6,

    /// <summary>
    /// 30天内
    /// </summary>
    WithinThirtyDays = 7,

    /// <summary>
    /// 本月
    /// </summary>
    ThisMonth = 8,

    /// <summary>
    /// 上月
    /// </summary>
    LastMonth = 9,

    /// <summary>
    /// 今年
    /// </summary>
    ThisYear = 10,

    /// <summary>
    /// 去年
    /// </summary>
    LastYear = 11,
}