namespace SchemaBuilder.Svc.Core.QueryHelper.Data;

/// <summary>
/// 查询范围
/// </summary>
public struct QueryRange<T> where T : struct
{
    public QueryRange(T? begin, T? end)
    {
        _begin = begin;
        _end = end;
    }

    private T? _begin;

    /// <summary>
    /// 大于或等于
    /// </summary>
    public T? Begin
    {
        get { return _begin; }
        set { _begin = value; }
    }

    private T? _end;

    /// <summary>
    /// 小于
    /// </summary>
    public T? End
    {
        get { return _end; }
        set { _end = value; }
    }

    public override string ToString()
    {
        return string.Format("[{0},{1}]", Begin, End);
    }

    /// <summary>
    /// 是否没有查询元素
    /// </summary>
    public bool IsEmpty()
    {
        return !(Begin.HasValue || End.HasValue);
    }
}