namespace SchemaBuilder.Svc.Core.TableBuilder
{
    public interface ITableBuilder
    {
        /// <summary>
        ///     获取建表Sql
        /// </summary>
        /// <param name="modelTable"></param>
        /// <returns></returns>
        string GetCreateTableSql(ModelTable modelTable);

        /// <summary>
        ///     获取修改字段类型或长度的SQL
        /// </summary>
        /// <param name="modelTable"></param>
        /// <returns></returns>
        string GetAlterColumnSql(ModelTable modelTable);

        /// <summary>
        ///     获取添加列SQL
        /// </summary>
        /// <param name="modelTable"></param>
        /// <returns></returns>
        string GetAddColumnSql(ModelTable modelTable);

        /// <summary>
        ///     获取添加列时,修改已存在数据默认值的SQL
        /// </summary>
        /// <param name="modelTable"></param>
        /// <returns></returns>
        string GetSetDefaultValueSql(ModelTable modelTable);
    }
}