using System.Data;
using SchemaBuilder.Svc.Helpers;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.Sqlite;
using ServiceStack.OrmLite.SqlServer;

namespace SchemaBuilder.Svc.Svc;

public class Db
{
    public static OrmLiteConnectionFactory _dbFactory;

    public static void IniSqlServer(string? connectionString)
    {
        _dbFactory = new OrmLiteConnectionFactory(connectionString, SqlServerOrmLiteDialectProvider.Instance);
    }

    public static void IniSqlite(string dbPath)
    {
        string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dbPath);

        // 确保目录存在
        string? directory = Path.GetDirectoryName(fullPath);
        if (directory.Has() && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        _dbFactory = new OrmLiteConnectionFactory(dbPath, SqliteOrmLiteDialectProvider.Instance);
    }

    public static OrmLiteConnectionFactory GetDbFactory()
    {
        return _dbFactory;
    }

    public static IDbConnection Open()
    {
        return _dbFactory.OpenDbConnection();
    }

    public static T OpenDbWithNoLock<T>(Func<IDbConnection, T> func)
    {
        using (var db = Open())
        using (var tran = db.OpenTransaction(IsolationLevel.ReadUncommitted))
        {
            return func(db);
        }
    }

    public static void OpenDbWithNoLock(Action<IDbConnection> func)
    {
        using (var db = Open())
        using (var tran = db.OpenTransaction(IsolationLevel.ReadUncommitted))
        {
            func(db);
        }
    }

    public static T Open<T>(Func<IDbConnection, T> func)
    {
        using (var db = Open())
        {
            return func(db);
        }
    }

    public static void Open(Action<IDbConnection> func)
    {
        using (var db = Open())
        {
            func(db);
        }
    }
}