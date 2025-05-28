namespace SchemaBuilder.Svc.Models.Attr;

/// <summary>
/// 必须基于FullTable
/// </summary>
/// <param name="sysModuleName"></param>
/// <param name="title"></param>
/// <param name="sizeMode"></param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class ModalDataSelectAttribute(
    string sysModuleName,
    string title,
    int sizeMode = 0)
    : Attribute
{
    public string SysModuleName { get; set; } = sysModuleName;
    public string Title { get; set; } = title;
    public int SizeMode { get; set; } = sizeMode;
}