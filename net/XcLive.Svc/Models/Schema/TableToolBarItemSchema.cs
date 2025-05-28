using SchemaBuilder.Svc.Models.ViewModel;

namespace SchemaBuilder.Svc.Models.Schema;

public class TableToolBarItemSchema
{
    public string? Name { get; set; }
    public string? Icon { get; set; }
    public int Index { get; set; }

    /// <summary>
    /// 'primary' | 'normal' | 'divider'
    /// </summary>
    public string? Type { get; set; }

    public ModuleVm? Module { get; set; }
    public List<TableToolBarItemSchema> Children { get; set; }
}