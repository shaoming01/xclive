namespace SchemaBuilder.Svc.Models.Attr;

/// <summary>
/// 编辑类型默认值
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class ObjectEditorAttribute(string sysModuleName, string title) : Attribute
{
    public string SysModuleName { get; set; } = sysModuleName;
    public string Title { get; set; } = title;

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