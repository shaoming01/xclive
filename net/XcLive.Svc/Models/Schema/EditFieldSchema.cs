using SchemaBuilder.Svc.Models.ViewModel;

namespace SchemaBuilder.Svc.Models.Schema;

public class EditFieldSchema
{
    public string? label { get; set; }
    public string? field { get; set; }
    public string? fieldType { get; set; }
    public object? defaultValue { get; set; }
    public bool? require { get; set; }
    public string? tip { get; set; }
    public int? labelColSpan { get; set; }
    public int? labelColOffset { get; set; }
    public int? wrapperColSpan { get; set; }
    public int? wrapperColOffset { get; set; }
    public int? span { get; set; }
    public int? offset { get; set; }
    public string? groupName { get; set; }
    public bool? disabled { get; set; }
    public bool? allowClear { get; set; }
    public ModuleVm? module { get; set; }
}