using System.Text.Json;
using SchemaBuilder.Svc.Core.Ext;
using SchemaBuilder.Svc.Models.Table;

namespace SchemaBuilder.Svc.Models.ViewModel;

public class SearchGroupEditVm
{
    public static UserSearchGroup ToModel(SearchGroupEditVm vm, long userId)
    {
        return new UserSearchGroup
        {
            Id = vm.Id,
            Index = vm.Index,
            Path = vm.Path.ClearPath(),
            Name = vm.Name,
            UserId = userId,
            ConditionsJson = JsonSerializer.Serialize(vm.Conditions ?? new Dictionary<string, object>())
        };
    }

    public static SearchGroupEditVm FromModel(UserSearchGroup model)
    {
        return new SearchGroupEditVm
        {
            Id = model.Id,
            Index = model.Index,
            Path = model.Path,
            Name = model.Name,
            Conditions =
                JsonSerializer.Deserialize<Dictionary<string, object>>(model.ConditionsJson ?? "{}")
        };
    }

    public long Id { get; set; }
    public int Index { get; set; }
    public string? Name { get; set; }
    public string? Path { get; set; }
    public Dictionary<string, object>? Conditions { get; set; }
}