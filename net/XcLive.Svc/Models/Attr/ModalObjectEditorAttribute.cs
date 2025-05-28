namespace SchemaBuilder.Svc.Models.Attr;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class ModalObjectEditorAttribute(
    string sysModuleName,
    string title,
    string getDataUrl,
    string saveDataUrl,
    int sizeMode = 0)
    : Attribute
{
    public string SysModuleName { get; set; } = sysModuleName;
    public string Title { get; set; } = title;

    public string GetDataUrl { get; set; } = getDataUrl;
    public string SaveDataUrl { get; set; } = saveDataUrl;
    public int SizeMode { get; set; } = sizeMode;
    public bool Centered { get; set; }
}