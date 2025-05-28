using SchemaBuilder.Svc.Core.Cache;
using SchemaBuilder.Svc.Core.KeyLock;
using SchemaBuilder.Svc.Helpers;
using SchemaBuilder.Svc.Svc;

namespace SchemaBuilder.Web.Helpers;

public class AppHelper
{
    public static void Init(ConfigurationManager builderConfiguration)
    {
        InitDb(builderConfiguration);
        InitCache(builderConfiguration);
    }

    private static void InitCache(ConfigurationManager builderConfiguration)
    {
        var connectionString = builderConfiguration.GetConnectionString("RedisCacheConnection");
        CacheHelper.Ini(connectionString);
        if (connectionString.Has())
        {
            KeyLockHelper.IniRedisKeyLock(connectionString);
        }
        else
        {
            KeyLockHelper.IniMemKeyLock();
        }
    }

    private static void InitDb(ConfigurationManager builderConfiguration)
    {
        ServiceStack.Logging.LogManager.LogFactory = new OrmliteLogFactory();

        var sqlConn = builderConfiguration.GetConnectionString("SqlServerConnection");
        if (sqlConn.Has())
        {
            Db.IniSqlServer(sqlConn);
        }

        var ormliteConn = builderConfiguration.GetConnectionString("SqliteConnection");
        if (ormliteConn.Has())
        {
            Db.IniSqlite(ormliteConn);
        }

        DataBaseHelper.IniTables();
    }
}