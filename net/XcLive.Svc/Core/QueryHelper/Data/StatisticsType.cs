using System.ComponentModel;

namespace SchemaBuilder.Svc.Core.QueryHelper.Data;

/// <summary>
/// 统计类型
/// </summary>
public enum StatisticsType
{
    [Description("")] None = 0,
    [Description("COUNT(统计函数)")] Count = 1,
    [Description("SUM(求和函数)")] Sum = 2,
    [Description("AVG(求平均值函数)")] Avg = 3,
    [Description("MAX(最大值函数)")] Max = 4,
    [Description("MIN(最小值函数)")] Min = 5,
    [Description("STDEV(标准偏差值函数)")] Stdev = 6,
    [Description("VAR(方差值函数)")] Var = 7,
}