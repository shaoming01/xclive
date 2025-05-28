namespace SchemaBuilder.Svc.Models.Attr;

/// <summary>
/// 搜索字段
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class SearchContainerAttribute(string sysModuleName) : Attribute
{
    /**
     * 标识这是由代码生成的系统模块
     */
    public string SysModuleName { get; set; } = sysModuleName;

    /// <summary>
    /// 
    /// </summary>
    public int Span { get; set; } = 8;

    /// <summary>
    /// 
    /// </summary>
    public int OffSet { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int LabelColSpan { get; set; } = 8;

    /// <summary>
    /// 
    /// </summary>
    public int LabelColOffset { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int WrapperColSpan { get; set; } = 12;

    /// <summary>
    /// 
    /// </summary>
    public int WrapperColOffset { get; set; }
}