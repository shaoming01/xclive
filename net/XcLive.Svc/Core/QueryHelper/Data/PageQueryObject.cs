namespace SchemaBuilder.Svc.Core.QueryHelper.Data;

/// <summary>
/// 查询对象
/// </summary>
/// <typeparam name="T">查询对象</typeparam>
public class PageQueryObject<T>
{
    /// <summary>
    /// 页码
    /// </summary>
    public int? Page { get; set; } = 1;

    /// <summary>
    /// 每页大小
    /// </summary>
    public int? PageSize { get; set; } = 20;

    /// <summary>
    /// 排序字段
    /// </summary>
    public string? OrderBy { get; set; }

    /// <summary>
    /// 查询对象
    /// </summary>
    public T? QueryObject { get; set; }

    /// <summary>
    /// 将当前查询对象克隆一份
    /// </summary>
    /// <param name="isCopyQueryObject">是否克隆查询对象</param>
    /// <returns>查询对象</returns>
    public PageQueryObject<T> Clone(bool isCopyQueryObject = true)
    {
        return new PageQueryObject<T>
        {
            Page = Page,
            PageSize = PageSize,
            OrderBy = OrderBy,
            QueryObject = isCopyQueryObject
                ? QueryObject
                : default(T)
        };
    }
}