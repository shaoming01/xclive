using SchemaBuilder.Svc.Models.Schema;
using SchemaBuilder.Svc.Models.ViewModel;

namespace SchemaBuilder.Svc.Models.Attr;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class FieldEditorAttribute : Attribute
{
    /// <summary>
    /// 实例化查询对象
    /// </summary>
    public FieldEditorAttribute(string title, string groupTitle = "")
    {
        Title = title;
        GroupTitle = groupTitle;
    }

    public string PropJson { get; set; }


    /// <summary>
    /// 编辑字段类型，一般的自动推断就行
    /// </summary>
    public FieldEditorType EditorType { get; set; }

    public String Title { get; set; }
    public String GroupTitle { get; set; }
    public int GroupIndex { get; set; }
    public bool GroupActive { get; set; }
    public object? Default { get; set; }
    public bool Require { get; set; }
    public bool AllowClear { get; set; }
    public string Tip { get; set; }
    public int Index { get; set; }
    public int Rows { get; set; } = 1;
    public int Span { get; set; } = 8;
    public int Offset { get; set; }
    public int LabelColSpan { get; set; } = 8;
    public int LabelColOffset { get; set; }
    public int WrapperColSpan { get; set; } = 12;
    public int WrapperColOffset { get; set; }
    public bool Disabled { get; set; }

    /// <summary>
    /// 设置了该字段会自动推导编辑控件为列表选择控件
    /// </summary>
    public ValueDisplayType ValueListType { get; set; }
}

public enum FieldEditorType
{
    /// <summary>
    /// 自动推断
    /// </summary>
    Undefined = 0,
    IconSelectInput = 1,

    /// <summary>
    /// 模块编辑控件，可以选择一个组件，在下方渲染该组件的属性编辑字段
    /// </summary>
    ModuleEditor = 2,
    VueComSelectInput = 3,
}