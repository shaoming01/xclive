// ReSharper disable InconsistentNaming

using SchemaBuilder.Svc.Core.QueryHelper.Data;
using SchemaBuilder.Svc.Models.Attr;
using SchemaBuilder.Svc.Models.Schema;
using SchemaBuilder.Svc.Models.Table;

namespace SchemaBuilder.Svc.Models.ViewModel;

[SearchContainer(nameof(Module))]
public class ModuleQuery : QueryDbEx<Module, ModuleVm>
{
    [FieldEditor("模块名称", WrapperColSpan = 16, Span = 4)]
    public StringQuery? ModuleName { get; set; }

    [FieldEditor("模块路径", WrapperColSpan = 16, Span = 4)]
    public StringQuery? ComPath { get; set; }
}

[FullTable(nameof(Module), TableType.MainTable, QueryDataUrl = "/api/module/queryList",
    QueryCountUrl = "/api/module/queryCount", DeleteIdsUrl = "/api/module/deleteIds", PageSize = 100)]
[TableTool(ToolType.Delete, 0, 30)]
public class ModuleVm
{
    /// <summary>
    /// 模块实例Id
    /// </summary>
    [TableColumn("Id")]
    public long Id { get; set; }

    [TableColumn("SysModuleId")] public string? SysModuleId { get; set; }

    /// <summary>
    /// 模块名称，仅用于人识别
    /// </summary>
    [TableColumn("模块名称")]
    public string? ModuleName { get; set; }

    [TableColumn("系统模块")] public string? SysModuleName { get; set; }

    /// <summary>
    /// 使用的组件路径
    /// </summary>
    [TableColumn("模块路径", 200)]
    public string? ComPath { get; set; }

    /// <summary>
    /// 主分类:二级分类:三级分类
    /// </summary>
    [TableColumn("分类")]
    public string? CategoryPath { get; set; }

    [TableColumn("属性")] public Dictionary<string, object>? Props { get; set; }
}

[ObjectEditor(nameof(ModuleEditVm), "模块")]
public class ModuleEditVm
{
    /// <summary>
    /// 模块实例Id
    /// </summary>
    [FieldEditor("Id", Disabled = true, Span = 12)]
    public long Id { get; set; }

    [FieldEditor("系统模块Id", Span = 12, Disabled = true)]
    public string? SysModuleId { get; set; }

    /// <summary>
    /// 模块名称，仅用于人识别
    /// </summary>
    [FieldEditor("模块名称", Span = 12, Require = true)]
    public string? ModuleName { get; set; }

    /// <summary>
    /// 主分类:二级分类:三级分类
    /// </summary>
    [FieldEditor("分类", Span = 12)]
    public string? CategoryPath { get; set; }

    [FieldEditor("系统模块", Span = 12, ValueListType = ValueDisplayType.SysModule)]
    public string? SysModuleName { get; set; }


    /// <summary>
    /// 使用的组件路径
    /// </summary>
    [FieldEditor("Vue组件", Span = 24, LabelColSpan = 4, WrapperColSpan = 20, Offset = 0,
        EditorType = FieldEditorType.VueComSelectInput)]
    public string? ComPath { get; set; }


    public Dictionary<string, object>? Props { get; set; }
}