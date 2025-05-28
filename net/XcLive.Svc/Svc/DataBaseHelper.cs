using SchemaBuilder.Svc.Core.TableBuilder;
using SchemaBuilder.Svc.Models.Table;
using ServiceStack.OrmLite;

namespace SchemaBuilder.Svc.Svc;

public static class DataBaseHelper
{
    public static void IniTables()
    {
        using var db = Db.Open();
        DbRepairHelper.RepairTables(typeof(ITable), Db.GetDbFactory());
        db.Delete<Menu>(m => m.Id < 100);
        db.InsertAll(DemoDataSvc.CreateDemoMenu());

#if DEBUG
        db.Delete<UserInfo>(m => m.Id < 200);
#endif
        if (!db.Exists<UserInfo>(u => true))
        {
            db.InsertAll(DemoDataSvc.CreateDemoUser());
        }

#if DEBUG
        db.Delete<Module>(m => m.Id < 200);
#endif
        if (!db.Exists<Module>(m => m.Id < 200))
        {
            db.InsertAll(DemoDataSvc.CreateDemoModules());
        }
        db.InsertAll(DemoDataSvc.GetNewSystemModules());



#if DEBUG
        db.Delete<ModelAuthInfo>(m => m.Id < 200);
#endif
        if (!db.Exists<ModelAuthInfo>(m => true))
            db.InsertAll(DemoDataSvc.CreateDemoModelAuthInfo());

#if DEBUG
        db.Delete<LiveScriptTemplate>(m => m.Id < 200);
#endif
        if (!db.Exists<LiveScriptTemplate>(m => true))
            db.InsertAll(DemoDataSvc.CreateDemoScriptTemplate());

#if DEBUG
        db.Delete<LiveRoom>(m => m.Id < 200);
#endif
        if (!db.Exists<LiveRoom>(m => true))
            db.InsertAll(DemoDataSvc.CreateDemoLiveRoom());

#if DEBUG
        db.Delete<LiveAccount>(m => m.Id < 200);
        db.Delete<TtsModel>(m => m.Id < 200);

#endif
        if (!db.Exists<TtsModel>(m => true))
            db.InsertAll(DemoDataSvc.CreateDemoTtsModel());

#if DEBUG
        db.Delete<AiVerticalAnchor>(m => m.Id < 200);
#endif

        if (!db.Exists<AiVerticalAnchor>(m => true))
            db.InsertAll(DemoDataSvc.CreateDemoAiVerticalAnchor());

#if DEBUG
        db.Delete<Tenant>(m => m.Id < 200);
#endif
        if (!db.Exists<Tenant>(m => true))
            db.InsertAll(DemoDataSvc.CreateDemoTenant());

#if DEBUG
        db.Delete<CardKey>(m => m.Id < 200);
#endif
        if (!db.Exists<CardKey>(m => true))
            db.InsertAll(DemoDataSvc.CreateDemoCardKey());
    }
}