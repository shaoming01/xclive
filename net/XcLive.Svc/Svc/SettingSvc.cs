using System.Reflection;
using Newtonsoft.Json;
using SchemaBuilder.Svc.Core;
using SchemaBuilder.Svc.Core.Ext;
using SchemaBuilder.Svc.Helpers;
using SchemaBuilder.Svc.Models.Table;
using ServiceStack.OrmLite;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace SchemaBuilder.Svc.Svc;

public class SettingSvc
{
    public static R<T> GetSetting<T>(long tenantId, long userId) where T : ISettingObject
    {
        var r = GetSetting(tenantId, userId, typeof(T).Name);
        if (!r.Success)
        {
            return R.Faild<T>(r.Message);
        }

        return R.OK((T)r.Data);
    }

    public static R<ISettingObject> GetSetting(long tenantId, long userId, string typeName)
    {
        var type = Assembly.GetAssembly(typeof(ISettingObject))?.GetTypes().FirstOrDefault(t =>
        {
            return t.Name == typeName;
        });
        if (type == null)
        {
            return R.Faild<ISettingObject>($"未找到类型{typeName}");
        }

        if (!type.ImplementsInterface(typeof(IUserId)))
        {
            userId = 0;
        }

        if (!type.ImplementsInterface(typeof(ITenantId)))
        {
            tenantId = 0;
        }

        using var db = Db.Open();
        var setting = db.Single<SettingInfo>(s =>
            s.TenantId == tenantId && s.UserId == userId && s.ClassName == type.Name);
        if (setting == null)
        {
            var emptySetting = (ISettingObject)type.CreateInstanceEx();
            return R.OK(emptySetting);
        }

        var json = setting.JsonValue.IsNullOrEmpty() ? "{}" : setting.JsonValue;
        var obj = (ISettingObject)JsonConvert.DeserializeObject(json, type)!;
        return R.OK(obj);
    }

    public static R SaveSetting<T>(long tenantId, long userId, T setting)
    {
        return SaveSetting(tenantId, userId, typeof(T).Name, JsonConvert.SerializeObject(setting));
    }

    public static R SaveSetting(long tenantId, long userId, string typeName, string json)
    {
        var type = Assembly.GetAssembly(typeof(ISettingObject))?.GetTypes().FirstOrDefault(t => t.Name == typeName);
        if (type == null)
        {
            return R.Faild($"未找到类型{typeName}");
        }

        if (!type.ImplementsInterface(typeof(IUserId)))
        {
            userId = 0;
        }

        if (!type.ImplementsInterface(typeof(ITenantId)))
        {
            tenantId = 0;
        }

        var obj = JsonConvert.DeserializeObject(json, type);
        json = JsonSerializer.Serialize(obj);

        using var db = Db.Open();
        var row = db.Single<SettingInfo>(s =>
            s.TenantId == tenantId && s.UserId == userId && s.ClassName == type.Name);
        if (row == null)
        {
            row = new SettingInfo()
            {
                TenantId = tenantId,
                UserId = userId,
                Id = Id.NewId(),
                ClassName = type.Name,
                JsonValue = json,
            };
            db.Insert(row);
        }
        else
        {
            row.JsonValue = json;
            db.Update(row);
        }

        return R.OK();
    }
}