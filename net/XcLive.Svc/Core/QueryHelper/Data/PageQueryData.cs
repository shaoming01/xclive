namespace SchemaBuilder.Svc.Core.QueryHelper.Data;

/// <summary>
/// 查询中间层对象
/// </summary>
public class PageQueryData
{
    /// <summary>
    /// 页码
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// 每页大小
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// 排序字段
    /// </summary>
    public string OrderBy { get; set; }

    /// <summary>
    /// 查询项
    /// </summary>
    public QueryElementItem[] QueryElementItems { get; set; }
}