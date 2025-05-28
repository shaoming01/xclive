using Mapster;
using SchemaBuilder.Svc.Models.Table;
using SchemaBuilder.Svc.Models.ViewModel;
using ServiceStack.OrmLite;

namespace SchemaBuilder.Svc.Svc;

public abstract class MenuSvc
{
    /*private static List<MenuVm> GetChildMenus(List<Menu> list, long parentId)
    {
        var subList = list.Where(l => l.ParentId == parentId).ToList();
        var reList = subList.Select(sub => new MenuVm
        {
            id = sub.Id.ToString(),
            desc = sub.Desc,
            icon = sub.Icon,
            title = sub.Title,
            url = sub.Url,
            subMenus = GetChildMenus(list, sub.Id)
        }).ToList();
        return reList;
    }*/
    public static MenuEditVm Save(MenuEditVm vm)
    {
        var model = vm.Adapt<Menu>();
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

        return model.Adapt<MenuEditVm>();
    }
}