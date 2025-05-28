namespace SchemaBuilder.Svc.Core.QueryHelper.Data;

/// <summary>
/// 查询日期范围
/// </summary>
public struct QueryDateRange
{
    public QueryDateRange(DateRangeType type)
    {
        _type = type;
        _begin = new DateTime();
        _end = new DateTime();
    }

    public QueryDateRange(DateTime? begin, DateTime? end)
    {
        _begin = begin;
        _end = end;
        _type = DateRangeType.None;
    }

    //日期开始
    private DateTime? _begin;

    //日期结束
    private DateTime? _end;

    /// <summary>
    /// 类型
    /// </summary>
    private DateRangeType _type;

    /// <summary>
    /// 日期开始
    /// </summary>
    public DateTime? Begin
    {
        get
        {
            switch (_type)
            {
                case DateRangeType.Today:
                    return DateTime.Today;
                case DateRangeType.Yesterday:
                    return DateTime.Today.AddDays(-1);
                case DateRangeType.WithinThreeDays:
                    return DateTime.Today.AddDays(-3);
                case DateRangeType.WithinSevenDays:
                    return DateTime.Today.AddDays(-7);
                case DateRangeType.ThisWeek:
                    return DateTime.Today.AddDays(-(DateTime.Today.DayOfWeek == DayOfWeek.Sunday
                        ? 6
                        : (int)DateTime.Today.DayOfWeek - 1));
                case DateRangeType.LastWeek:
                    return DateTime.Today.AddDays(-(DateTime.Today.DayOfWeek == DayOfWeek.Sunday
                        ? 6
                        : (int)DateTime.Today.DayOfWeek - 1) - 7);
                case DateRangeType.WithinThirtyDays:
                    return DateTime.Today.AddDays(-30);
                case DateRangeType.ThisMonth:
                    return DateTime.Today.AddDays(-DateTime.Now.Day + 1);
                case DateRangeType.LastMonth:
                    return DateTime.Today.AddDays(-DateTime.Now.Day + 1).AddMonths(-1);
                case DateRangeType.ThisYear:
                    return DateTime.Today.AddDays(-DateTime.Today.DayOfYear + 1);
                case DateRangeType.LastYear:
                    return DateTime.Today.AddDays(-DateTime.Today.DayOfYear + 1).AddYears(-1);
                default:
                    return _begin;
            }
        }
        set { _begin = value; }
    }

    /// <summary>
    /// 日期结束
    /// </summary>
    public DateTime? End
    {
        get
        {
            switch (_type)
            {
                case DateRangeType.Today:
                    return DateTime.Today.AddDays(1);
                case DateRangeType.Yesterday:
                    return DateTime.Today;
                case DateRangeType.WithinThreeDays:
                    return DateTime.Today.AddDays(1);
                case DateRangeType.WithinSevenDays:
                    return DateTime.Today.AddDays(1);
                case DateRangeType.ThisWeek:
                    return DateTime.Today.AddDays(1);
                case DateRangeType.LastWeek:
                    return DateTime.Today.AddDays(-(DateTime.Today.DayOfWeek == DayOfWeek.Sunday
                        ? 6
                        : (int)DateTime.Today.DayOfWeek - 1));
                case DateRangeType.WithinThirtyDays:
                    return DateTime.Today.AddDays(1);
                case DateRangeType.ThisMonth:
                    return DateTime.Today.AddDays(1);
                case DateRangeType.LastMonth:
                    return DateTime.Today.AddDays(-DateTime.Now.Day + 1);
                case DateRangeType.ThisYear:
                    return DateTime.Today.AddDays(1);
                case DateRangeType.LastYear:
                    return DateTime.Today.AddDays(-DateTime.Today.DayOfYear + 1);
                default:
                    return _end;
            }
        }
        set { _end = value; }
    }

    /// <summary>
    /// 是否没有查询元素
    /// </summary>
    public bool IsEmpty()
    {
        return !(Begin.HasValue || End.HasValue);
    }

    public override string ToString()
    {
        return string.Format("[{0},{1}]", Begin, End);
    }
}