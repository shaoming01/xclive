using SchemaBuilder.Svc.Helpers;
using SchemaBuilder.Svc.Models.Dto;
using SchemaBuilder.Svc.Models.Schema;
using SchemaBuilder.Svc.Models.Table;
using SchemaBuilder.Svc.Svc.SchemaBuild;
using ServiceStack.OrmLite;

namespace SchemaBuilder.Svc.Svc;

public static class SysSvc
{
    public static List<ValueDisplay> ListValuerDisplay(ValueDisplayQuery query, UserLoginInfo user, bool hasSub = false)
    {
        if (query.EnumTypeName.Has())
        {
            var enumType = query.GetType().Assembly.GetType(query.EnumTypeName);
            if (enumType == null)
            {
                return new List<ValueDisplay>();
            }

            return ReadEnumValues(enumType);
        }

        if (query.Type == ValueDisplayType.SysModule)
        {
            return SysModuleBuilder.List();
        }
        else if (query.Type == ValueDisplayType.LiveScriptTemplate)
        {
            using var db = Db.Open();
            var list = db.Select<LiveScriptTemplate>(
                t => t.UserId == user.UserId && t.TenantId == user.TenantId);
            return list.Select(l => new ValueDisplay
            {
                Value = l.Id.ToString(),
                Label = l.Name + "",
            }).ToList();
        }
        else if (query.Type == ValueDisplayType.MainScriptTemplate)
        {
            using var db = Db.Open();
            var list = db.Select<LiveScriptTemplate>(
                t => t.UserId == user.UserId
                     && t.TenantId == user.TenantId &&
                     t.Usage == UsageType.LiveScript);
            return list.Select(l => new ValueDisplay
            {
                Value = l.Id.ToString(),
                Label = l.Name ?? "",
            }).ToList();
        }
        else if (query.Type == ValueDisplayType.ChatScriptTemplate)
        {
            using var db = Db.Open();
            var list = db.Select<LiveScriptTemplate>(
                t => t.UserId == user.UserId &&
                     t.TenantId == user.TenantId &&
                     t.Usage == UsageType.Chat);
            return list.Select(l => new ValueDisplay
            {
                Value = l.Id.ToString(),
                Label = l.Name ?? "",
            }).ToList();
        }
        else if (query.Type == ValueDisplayType.InteractScriptTemplate)
        {
            using var db = Db.Open();
            var list = db.Select<LiveScriptTemplate>(
                t => t.UserId == user.UserId &&
                     t.TenantId == user.TenantId &&
                     t.Usage == UsageType.InteractScriptTemplate);
            return list.Select(l => new ValueDisplay
            {
                Value = l.Id.ToString(),
                Label = l.Name ?? "",
            }).ToList();
        }
        else if (query.Type == ValueDisplayType.ShelfTaskConfig)
        {
            using var db = Db.Open();
            var list = db.Select<ShelfTaskConfig>(
                t => t.UserId == user.UserId &&
                     t.TenantId == user.TenantId);
            return list.Select(l => new ValueDisplay
            {
                Value = l.Id.ToString(),
                Label = l.Name ?? "",
            }).ToList();
        }
        else if (query.Type == ValueDisplayType.AiVerticalAnchor)
        {
            using var db = Db.Open();
            var list = db.Select<AiVerticalAnchor>(
                t => t.UserId == user.UserId &&
                     t.TenantId == user.TenantId);
            return list.Select(l => new ValueDisplay
            {
                Value = l.Id.ToString(),
                Label = l.Name ?? "",
            }).ToList();
        }
        else if (query.Type == ValueDisplayType.LiveRoom)
        {
            using var db = Db.Open();
            var list = db.Select<LiveRoom>(t => t.UserId == user.UserId);
            return list.Select(l => new ValueDisplay
            {
                Value = l.Id.ToString(),
                Label = l.Name + "",
            }).ToList();
        }
        else if (query.Type == ValueDisplayType.LiveObserver)
        {
            using var db = Db.Open();
            var list = db.Select<LiveAccount>(a => a.UserId == user.UserId && a.RoleType == AccountRoleType.Observer);
            return list.Select(l => new ValueDisplay
            {
                Value = l.Id.ToString(),
                Label = l.Name + "",
            }).ToList();
        }
        else if (query.Type == ValueDisplayType.TtsModel)
        {
            using var db = Db.Open();
            var list = db.Select<TtsModel>(a => a.TenantId == user.TenantId);
            return list.Select(l => new ValueDisplay
            {
                Value = l.Id.ToString(),
                Label = l.Name + "",
            }).ToList();
        }
        else if (query.Type == ValueDisplayType.LiveHuangCheOperate)
        {
            using var db = Db.Open();
            var list = db.Select<LiveAccount>(a =>
                a.UserId == user.UserId && a.RoleType == AccountRoleType.HuangCheOprate);
            return list.Select(l => new ValueDisplay
            {
                Value = l.Id.ToString(),
                Label = l.Name + "",
            }).ToList();
        }
        else if (query.Type == ValueDisplayType.Type1)
        {
            return
            [
                new()
                {
                    Value = "1",
                    Label = "一",
                },
                new(value: "2", label: "二二"),
                new()
                {
                    Value = "3",
                    Label = "三三三",
                }
            ];
        }

        return new List<ValueDisplay>();
    }

    private static List<ValueDisplay> ReadEnumValues(Type type)
    {
        var list = Enum.GetValues(type);
        var reList = new List<ValueDisplay>();
        foreach (var item in list)
        {
            var val = (int)item;
            var desc = ((Enum)item).GetEnumDescription();
            reList.Add(new ValueDisplay
            {
                Value = val.ToString(), Label = desc,
            });
        }

        return reList;
    }
}