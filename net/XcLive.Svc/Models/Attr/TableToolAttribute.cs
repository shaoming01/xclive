using SchemaBuilder.Svc.Core.Ext;

namespace SchemaBuilder.Svc.Models.Attr;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true)]
public class TableToolAttribute : Attribute
{
    public ToolType Type { get; set; }

    /**
     * 必须是ModalObjectEdit或兼容模块
     */
    public long ModuleId { get; set; }

    public string SysModuleId { get; set; }

    public string Name { get; set; }
    public int Index { get; set; }

    /// <summary>
    /// 'primary' | 'normal' | 'divider'
    /// </summary>
    public string ButtonType { get; set; }

    /// <summary>
    /// 图标名称
    /// </summary>
    public string ButtonIcon { get; set; }

    public string ComPath { get; set; }

    public TableToolAttribute(ToolType type, long moduleId = 0, int index = 0, string name = "")
    {
        Type = type;
        Name = name;
        Index = index;
        ModuleId = moduleId;
        if (name.IsNullOrEmpty())
        {
            switch (type)
            {
                case ToolType.Add:
                    Name = "新增";
                    ButtonIcon = "icon-tianjia";
                    break;
                case ToolType.Edit:
                    Name = "修改";
                    ButtonIcon = "icon-edit2";
                    break;
                case ToolType.Delete:
                    Name = "删除";
                    ButtonIcon = "icon-delete2";
                    break;
                case ToolType.LocalDelete:
                    Name = "删除";
                    ButtonIcon = "icon-delete2";
                    break;
            }
        }
    }

    public TableToolAttribute(ToolType type, string sysModuleId, int index = 0, string name = "")
    {
        Type = type;
        Name = name;
        Index = index;
        SysModuleId = sysModuleId;
        if (name.IsNullOrEmpty())
        {
            switch (type)
            {
                case ToolType.Add:
                    Name = "新增";
                    ButtonIcon = "icon-tianjia";
                    break;
                case ToolType.Edit:
                    Name = "修改";
                    ButtonIcon = "icon-edit2";
                    break;
                case ToolType.Delete:
                    Name = "删除";
                    ButtonIcon = "icon-delete2";
                    break;
                case ToolType.LocalDelete:
                    Name = "删除";
                    ButtonIcon = "icon-delete2";
                    break;
            }
        }
    }

    public TableToolAttribute(string comPath, int index = 0, string name = "")
    {
        Name = name;
        Index = index;
        Name = name;
    }
}

public enum ToolType
{
    Add,
    Edit,
    Delete,
    LocalDelete,
    Custom,
}