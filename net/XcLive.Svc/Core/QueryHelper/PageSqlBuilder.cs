using SchemaBuilder.Svc.Core.Ext;
using SchemaBuilder.Svc.Core.QueryHelper.Data;
using SchemaBuilder.Svc.Helpers;

namespace SchemaBuilder.Svc.Core.QueryHelper;

public class PageSqlBuilder
{
    private Dictionary<string, PageSqlBuilderItem> _pageSqlBuilderItems = new Dictionary<string, PageSqlBuilderItem>();

    private List<string> _where = new List<string>();

    /// <summary>
    /// 存储返回的查询条件
    /// </summary>
    public List<string> Where
    {
        get { return _where; }
    }

    public PageSqlBuilder(IEnumerable<QueryElementItem> queryElementItems)
    {
        (queryElementItems ?? new QueryElementItem[0]).GroupBy(b => b.Name).ForEach(f =>
        {
            var item = new PageSqlBuilderItem
            {
                Builder = this
            };
            f.ForEach(e => item.Add(e.Type, e)); //加入项
            _pageSqlBuilderItems[f.Key] = item;
        });
    }

    /// <summary>
    /// 是否存在此名称的项
    /// </summary>
    /// <param name="name">名称</param>
    /// <returns>是否存在</returns>
    public bool ContainsName(string name)
    {
        return _pageSqlBuilderItems.ContainsKey(name);
    }

    /// <summary>
    /// 如果存在就触发的事件
    /// </summary>
    /// <param name="name">键</param>
    /// <param name="action">回调</param>
    public void IfExist(string name, Action<PageSqlBuilderItem> action)
    {
        var item = _pageSqlBuilderItems.TryGetOrDefault(name);
        if (item != null)
            action(item);
    }

    /// <summary>
    /// 如果存在就触发的事件，并返回筛选条件
    /// </summary>
    /// <param name="name">键</param>
    /// <param name="func">回调</param>
    public void IfExist(string name, Func<PageSqlBuilderItem, string> func)
    {
        var item = _pageSqlBuilderItems.TryGetOrDefault(name);
        if (item != null)
            _where.Add(func(item));
    }
}