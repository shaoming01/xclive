using System.Diagnostics;
using System.Text.Json;
using SchemaBuilder.Svc.Core.Ext;
using SchemaBuilder.Svc.Helpers;
using SchemaBuilder.Svc.Models.Table;
using SchemaBuilder.Svc.Models.ViewModel;
using ServiceStack.OrmLite;

namespace SchemaBuilder.Svc.Svc;

public static class ModuleSvc
{
    public static ModuleVm Save(ModuleVm vm)
    {
        var model = ToModel(vm);
        var isInsert = model.Id == 0;

        using var db = Db.Open();
        if (isInsert)
        {
            model.Id = Id.NewId();
            db.Insert(model);
        }
        else
        {
            db.Update(model);
        }

        return ToVm(model);
    }

    public static ModuleVm? GetId(long moduleId)
    {
        using var db = Db.Open();
        var model = db.SingleById<Module>(moduleId);
        if (model == null)
        {
            return null;
        }

        return ToVm(model);
    }

    public static ModuleVm? GetSysModuleId(string? sysModuleId)
    {
        using var db = Db.Open();
        var model = db.Single<Module>(m => m.SysModuleId == sysModuleId);
        if (model == null)
        {
            return null;
        }

        return ToVm(model);
    }
    
    private static ModuleVm ToVm(Module model)
    {
        var json = model.PropsJson.IsNullOrEmpty() ? "{}" : model.PropsJson;
        Debug.Assert(json != null, nameof(json) + " != null");
        var props = JsonSerializer.Deserialize<Dictionary<string, Object>>(json);
        return new ModuleVm
        {
            Id = model.Id,
            SysModuleId = model.SysModuleId,
            Props = props,
            ModuleName = model.Name,
            SysModuleName = model.SysModuleName,
            ComPath = model.ComPath,
            CategoryPath = model.CategoryPath
        };
    }

    private static Module ToModel(ModuleVm vm)
    {
        var json = JsonSerializer.Serialize(vm.Props ?? new Dictionary<string, object>());
        return new Module
        {
            Id = vm.Id,
            SysModuleId = vm.SysModuleId,
            Name = vm.ModuleName,
            SysModuleName = vm.SysModuleName,
            ComPath = vm.ComPath,
            CategoryPath = vm.CategoryPath,
            PropsJson = json
        };
    }
}