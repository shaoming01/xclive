using System.Reflection;
using System.Text;
using SchemaBuilder.Svc.Core.Cache;
using SchemaBuilder.Svc.Helpers;
using ServiceStack.OrmLite;

namespace SchemaBuilder.Svc.Core.TableBuilder;

public static class DbRepairHelper
{
    public static void RepairTables(Type tableBaseType, OrmLiteConnectionFactory dbDefFactory,
        OrmLiteConnectionFactory dbUpdateFactory = null)
    {
        dbUpdateFactory ??= dbDefFactory;
        try
        {
            ITableBuilder? builer = GetTableBuilder(dbDefFactory.DialectProvider);
            if (builer == null)
            {
                CreteTables(tableBaseType, dbDefFactory, dbUpdateFactory);
            }
            else
            {
                UpdateTables(tableBaseType, builer, dbDefFactory, dbUpdateFactory);
            }
        }
        catch (Exception ex)
        {
            Log4.Log.Error(ex);
            throw;
        }
    }

    private static void CreteTables(Type tableBaseType, OrmLiteConnectionFactory dbFactory,
        OrmLiteConnectionFactory dbUpdateFactory)
    {
        var tableTypes = FindTableTypes(tableBaseType);
        using var db = dbFactory.OpenDbConnection();
        tableTypes.ForEach(model => { db.CreateTableIfNotExists(model); });
    }

    private static Type[] FindTableTypes(Type tableBaseType)
    {
        Assembly? ass = Assembly.GetAssembly(tableBaseType);
        Type[]? types = ass?.GetExportedTypes();
        Type[]? modelTypes =
            types?.Where(
                    t => tableBaseType.IsAssignableFrom(t) && !t.IsAbstract)
                .ToArray();
        return modelTypes?? [];
    }

    public static ITableBuilder? GetTableBuilder(IOrmLiteDialectProvider dbProvider)
    {
        if (dbProvider.GetType().Name == "SqlServerOrmLiteDialectProvider")
        {
            return new SqlServerTableBuilder();
        }
        return null;
    }

    /// <summary>
    ///     更新数据库表
    /// </summary>
    /// <param name="tableBaseType"></param>
    /// <param name="builer"></param>
    /// <param name="dbDefFactory"></param>
    /// <param name="dbUpdateFactory"></param>
    private static void UpdateTables(Type tableBaseType, ITableBuilder? builer, OrmLiteConnectionFactory dbDefFactory,
        OrmLiteConnectionFactory dbUpdateFactory)
    {
        var tableTypes = FindTableTypes(tableBaseType);
        using (var dbDef = dbDefFactory.OpenDbConnection())
        using (var dbUpdate = dbUpdateFactory.OpenDbConnection())
        {
            dbDef.SetCommandTimeout(900);
            dbUpdate.SetCommandTimeout(900);

            var updateSql = new StringBuilder();
            var setDefaultValueSql = new StringBuilder();
            tableTypes.Split(10)
                .ForEach(page =>
                {
                    page.ForEach(m =>
                    {
                        //比对版本号
                        var modelTable = new ModelTable(m);
                        updateSql.AppendLine(builer.GetCreateTableSql(modelTable));
                        updateSql.AppendLine(builer.GetAlterColumnSql(modelTable));
                        updateSql.AppendLine(builer.GetAddColumnSql(modelTable));
                        setDefaultValueSql.AppendLine(builer.GetSetDefaultValueSql(modelTable));
                    });
                    //执行更新脚本
                    if (updateSql.Length > 0)
                    {
                        dbDef.ExecuteSql(updateSql.ToString());
                    }
                    if (setDefaultValueSql.Length > 0)
                    {
                        dbUpdate.ExecuteSql(setDefaultValueSql.ToString());
                    }
                    setDefaultValueSql.Clear();
                    updateSql.Clear();
                });

               
        }
    }

}