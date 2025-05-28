using SchemaBuilder.Svc.Models.Schema;
using SchemaBuilder.Svc.Models.ViewModel;

namespace SchemaBuilder.Svc.Models.Attr;

[AttributeUsage(AttributeTargets.Property)]
public class TableColumnAttribute : Attribute
{
    /// <summary>
    /// 实例化属性列
    /// </summary>
    /// <param name="title">显示标题</param>
    /// <param name="width">宽度</param>
    /// <param name="sourceType"></param>
    public TableColumnAttribute(string title, int width = 120,
        ValueDisplayType sourceType = ValueDisplayType.Undefined)
    {
        Title = title;
        Width = width;
        ValueListType = sourceType;
    }

    /// <summary>
    /// 字段名称【不填写默认字段名称】
    /// </summary>
    public string Field { get; set; }

    /// <summary>
    /// 显示列标题
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// 导出别名
    /// </summary>
    public string ExportTitle { get; set; }

    /// <summary>
    /// 提示
    /// </summary>
    public string Tip { get; set; }

    /// <summary>
    /// 列分组标题
    /// </summary>
    public string GroupTitle { get; set; }

    /// <summary>
    /// 宽度
    /// </summary>
    public int Width { get; set; }

    //排序索引
    public int Index { get; set; }

    /// <summary>
    /// 字段类型【不填写默认字段类型】
    /// </summary>
    public Type Type { get; set; }

    /// <summary>
    /// 是否不可调整
    /// </summary>
    public bool IsFixed { get; set; }

    /// <summary>
    /// 是否隐藏
    /// </summary>
    public bool IsHide { get; set; }

    /// <summary>
    /// 是否可编辑
    /// </summary>
    public bool Editable { get; set; }

    /// <summary>
    /// 大图字段名称
    /// 此字段有值代表：当前字段是小图URL
    /// </summary>
    public string LargeImageField { get; set; }

    /// <summary>
    /// 数据源
    /// </summary>
    public ValueDisplayType ValueListType { get; set; }

    public CellRenderType RenderType { get; set; }
}

public enum CellRenderType
{
    Undefined = 0,
    IconRender = 1,

    /// <summary>
    /// 下拉列表选择编辑器
    /// </summary>
    ListSelectRender = 2,
    VoicePlayerRender = 3,
    LongStringRender = 4,
    ImageRender = 5,
}