namespace SchemaBuilder.Svc.Models.Attr;

/// <summary>
/// 标记某个DTO类是某个Vue容器组件的显示对象，该对象的字段会生成该VUE容器组件的显示元素
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class FullTableAttribute : Attribute
{
    private static int DefaultPageSize = 20;
    public string SysModuleName { get; set; }
    public TableType ElType { get; set; }

    public string QueryDataUrl { get; set; }
    public string QueryCountUrl { get; set; }

    public string DeleteIdsUrl { get; set; }

    public string PageSizeOptions { get; set; } = "20,50,100";
    public int PageSize { get; set; } = 20;
    public bool AutoQuery { get; set; } = true;

    public FullTableAttribute(string sysModuleName, TableType elType)
    {
        SysModuleName = sysModuleName;
        ElType = elType;
        if (elType == TableType.MainTable)
        {
        }
    }

    public int Index { get; set; }
    public string Title { get; set; }

    public bool MultiSelection { get; set; }
    public string PrimaryKey { get; set; } = "id";
    public string HeaderIdKey { get; set; } = "headerId";
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true)]
public class DetailTableAttribute : FullTableAttribute
{
    /// <summary>
    /// 在字段上面标记
    /// </summary>
    /// <param name="title"></param>
    public DetailTableAttribute(string title) : base("", TableType.DetailTable)
    {
        Title = title;
    }

    public DetailTableAttribute(string sysModuleName, string title) : base(sysModuleName, TableType.DetailTable)
    {
        Title = title;
    }
}