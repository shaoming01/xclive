using System.Globalization;
using System.Text;

namespace SchemaBuilder.Svc.Core.TableBuilder
{
    public class SqlServerTableBuilder : ITableBuilder
    {
        public string GetCreateTableSql(ModelTable modelTable)
        {
            const string fmt = @"if not exists( select 1 FROM sysobjects WHERE type='U' AND name='{0}')
begin
CREATE TABLE [{0}]
(
{1}
)
end
";
            const string fmtItem = "[{0}] {1} {2} {3} {4},";
            var colSb = new StringBuilder();
            modelTable.Cols.ForEach(col =>
            {
                DbLength length = GetLength(col);
                string lengthStr = length.ToString();
                object defaultValue = GetDefaultValue(col);
                string defaultStr = defaultValue != null ? string.Format("{0}", defaultValue) : "";
                colSb.AppendLine(string.Format(fmtItem, col.Name, GetDbTypeString(col), lengthStr, defaultStr,
                    GetNullString(col)));
            });
            string colStr = colSb.ToString();
            return string.Format(fmt, modelTable.Name, colStr);
        }


        public string GetAlterColumnSql(ModelTable modelTable)
        {
            const string fmtNoLength = @"
IF EXISTS (SELECT 1 FROM sys.objects tab
INNER JOIN sys.columns col ON col.object_id = tab.object_id
INNER JOIN sys.types type ON col.system_type_id=type.user_type_id
WHERE tab.name='{0}' AND col.name='{1}' AND REPLACE(type.name,'smalldatetime','datetime') <>'{2}')
BEGIN
ALTER TABLE [{0}]
ALTER COLUMN [{1}] {2}
END 
";
            const string fmt = @"
IF EXISTS (SELECT 1 FROM sys.objects tab
INNER JOIN sys.columns col ON col.object_id = tab.object_id
INNER JOIN sys.types type ON col.system_type_id=type.user_type_id
WHERE tab.name='{0}' AND col.name='{1}' AND (REPLACE(type.name,'smalldatetime','datetime') <>'{2}' or col.max_length<'{3}' ))
BEGIN
ALTER TABLE [{0}]
ALTER COLUMN [{1}] {2}{4}
END 
";

            var reSb = new StringBuilder();
            modelTable.Cols.ForEach(col =>
            {
                if (col.IsRowVersion)
                {
                    return;
                }
                DbLength length = GetLength(col);
                if (length.Type == DbLengthType.None)
                {
                    reSb.AppendLine(string.Format(fmtNoLength, modelTable.Name, col.Name, GetDbTypeString(col)));
                }
                else
                {
                    reSb.AppendLine(string.Format(fmt, modelTable.Name, col.Name, GetDbTypeString(col),
                        length.GetLengthValue(), length));
                }
            }
                );
            return reSb.ToString();
        }

        public string GetSetDefaultValueSql(ModelTable modelTable)
        {
            const string fmt2 = @"UPDATE [{0}] SET [{1}]={2} WHERE [{1}] IS NULL";
            var reSb = new StringBuilder();
            modelTable.Cols.ForEach(col =>
            {
                string setDefaultValue = GetSetDefaultValue(col);
                if (!string.IsNullOrEmpty(setDefaultValue))
                {
                    string setDefaultSql = string.Format(fmt2, modelTable.Name, col.Name, setDefaultValue);
                    reSb.AppendLine(setDefaultSql);
                }
            });
            return reSb.ToString();
        }

        public string GetAddColumnSql(ModelTable modelTable)
        {
            const string fmt = @"
IF NOT EXISTS (SELECT 1 FROM sys.objects tab
INNER JOIN sys.columns col ON col.object_id = tab.object_id
INNER JOIN sys.types type ON col.system_type_id=type.user_type_id
WHERE tab.name='{0}' AND col.name='{1}')
BEGIN 
ALTER TABLE [{0}]
ADD [{1}] {2}{3} {4}
END
";
            var reSb = new StringBuilder();
            modelTable.Cols.ForEach(col =>
            {
                DbLength length = GetLength(col);
                object defaultValue = GetDefaultValue(col);
                string defaultStr = defaultValue != null ? string.Format("{0}", defaultValue) : "";
                reSb.AppendLine(string.Format(fmt, modelTable.Name, col.Name, GetDbTypeString(col),
                    length.ToString(), defaultStr));
            });
            return reSb.ToString();
        }

        #region Mapping

        private string GetDbTypeString(ModelColumn column)
        {
            Type perType = column.PerInfo.PropertyType;
            string re = "varchar";
            /*if (column.IsNvarchar)
            {
                re = "nvarchar";
            }*/
            if (column.IsRowVersion)
            {
                re = "timestamp";
            }
            else if (perType == typeof (long) || perType == typeof (long?)
                     || perType == typeof (ulong) || perType == typeof (ulong?))
            {
                re = "bigint";
            }
            else if (perType == typeof (short) || perType == typeof (short?))
            {
                re = "smallint";
            }
            else if (perType == typeof (int) || perType == typeof (int?)
                     || perType == typeof (uint) || perType == typeof (uint?))
            {
                re = "int";
            }
            else if (perType == typeof (Guid) || perType == typeof (Guid?))
            {
                re = "uniqueidentifier";
            }
            else if (perType == typeof (DateTime) || perType == typeof (DateTime?))
            {
                re = "datetime";
            }
            else if (perType == typeof (DateTimeOffset) || perType == typeof (DateTimeOffset?))
            {
                re = "datetimeoffset";
            }
            else if (perType == typeof (double) || perType == typeof (double?))
            {
                re = "float";
            }
            else if (perType == typeof (float) || perType == typeof (float?))
            {
                re = "float";
            }
            else if (perType == typeof (decimal) || perType == typeof (decimal?))
            {
                re = "decimal";
            }
            else if (perType == typeof (byte) || perType == typeof (byte?))
            {
                re = "tinyint";
            }
            else if (perType == typeof (bool) || perType == typeof (bool?))
            {
                re = "bit";
            }
            else if (perType == typeof (byte[]))
            {
                re = "varbinary";
            }
            else if (perType.IsEnum)
            {
                re = "int";
            }
            else if (column.Length > 8000)
            {
                re = "text";
            }
            return re;
        }

        private DbLength GetLength(ModelColumn column)
        {
            var re = new DbLength {Type = DbLengthType.None};
            Type perType = column.PerInfo.PropertyType;
            if (column.IsRowVersion)
            {
                re.Type = DbLengthType.None;
            }
            else if (perType == typeof (string) && column.Length > 8000)
            {
                re.Type = DbLengthType.None;
            }
            else if (perType == typeof (byte[]) && column.Length > 8000)
            {
                re.Type = DbLengthType.Max;
                re.Length = -1;
            }
            else if (perType == typeof (decimal) || perType == typeof (decimal?))
            {
                re.Type = DbLengthType.Fix;
                re.Length = column.Length > 0 ? column.Length : 18;
                re.LengthBak = 6;
            }
            else if (column.Length > 0)
            {
                re.Type = DbLengthType.Fix;
                re.Length = column.Length;
            }
            else if (perType == typeof (string))
            {
                re.Type = DbLengthType.Fix;
                re.Length = 128;
            }
            else if (perType == typeof (byte[]))
            {
                re.Type = DbLengthType.Fix;
                re.Length = 8000;
            }
/*            if (column.Length > 0)
            {
                re = column.Length.Value;
            }
            else if (perType == typeof (long) || perType == typeof (long?))
            {
                re = null;
            }
            else if (perType == typeof (short) || perType == typeof (short?))
            {
                re = null;
            }
            else if (perType == typeof (int) || perType == typeof (int?))
            {
                re = null;
            }
            else if (perType == typeof (Guid) || perType == typeof (Guid?))
            {
                re = null;
            }
            else if (perType == typeof (DateTime) || perType == typeof (DateTime?))
            {
                re = null;
            }
            else if (perType == typeof (DateTimeOffset) || perType == typeof (DateTimeOffset?))
            {
                re = null;
            }
            else if (perType == typeof (double) || perType == typeof (double?))
            {
                re = null;
            }
            else if (perType == typeof (decimal) || perType == typeof (decimal?))
            {
                re = null;
            }
            else if (perType == typeof (byte) || perType == typeof (byte?))
            {
                re = null;
            }
            else if (perType == typeof (bool) || perType == typeof (bool?))
            {
                re = null;
            }
            else if (perType == typeof (byte[]))
            {
                re = 8000;
            }*/
            return re;
        }

        private string GetSetDefaultValue(ModelColumn column)
        {
            Type perType = column.PerInfo.PropertyType;
            string re = null;
            if (column.AutoIncrement || 
                column.IsRowVersion ||
                perType == typeof (string) ||
                (perType.IsGenericType && perType.GetGenericTypeDefinition().IsAssignableFrom(typeof (Nullable<>))))
            {
            }
            else if (perType.IsEnum || perType == typeof (bool))
            {
                return "0";
            }
            else if (perType == typeof (DateTime))
            {
                re = "'1900-1-1'";
            }
            else if (perType.IsValueType)
            {
                return string.Format("'{0}'", Activator.CreateInstance(perType));
            }
            return re;
        }

        private string GetDefaultValue(ModelColumn column)
        {
//            Type perType = column.PerInfo.PropertyType;
            string re = "";
            if (column.AutoIncrement) //自增列
            {
                re = "IDENTITY(1,1)";
            }
/*
            else if (perType == typeof (long)
                     || perType == typeof (short)
                     || perType == typeof (int)
                     || perType == typeof (double)
                     || perType == typeof (decimal)
                     || perType.IsEnum
                )
            {
                re = "DEFAULT(0)";
            }
            else if (perType == typeof (Guid))
            {
                re = string.Format("DEFAULT('{0}')", Guid.Empty);
            }
            else if (perType == typeof (DateTime))
            {
                re = null;
            }
            else if (perType == typeof (DateTimeOffset))
            {
                re = null;
            }
            else if (perType == typeof (byte))
            {
                re = null;
            }
            else if (perType == typeof (bool))
            {
                re = "DEFAULT(0)";
            }
            else if (perType == typeof (string))
            {
                re = "DEFAULT('')";
            }
            else if (perType.IsGenericType && perType.GetGenericTypeDefinition().IsAssignableFrom(typeof (Nullable<>)))
            {
                re = "DEFAULT(null)";
            }
*/
            return re;
        }

        private string GetNullString(ModelColumn column)
        {
/*
            if (column.AutoIncrement)
            {
                return "";
            }
            return "NULL"; //否则增加新的字段需要有默认值,而默认值是通过约束实现的,修改字段时又很麻烦,所以直接不通过这种方式来实现了
*/
            Type perType = column.PerInfo.PropertyType;
            string re = "NOT NULL";
            Type nullType = typeof (Nullable<>);
            var isNullable = perType.IsGenericType && perType.GetGenericTypeDefinition().IsAssignableFrom(nullType);
            if (isNullable || perType == typeof (string) || column.IsNull == true)
            {
                re = "NULL";
            }
            return re;
        }

        #region 附属类

        private class DbLength
        {
            public DbLengthType Type { get; set; }
            public int? Length { get; set; }
            public int? LengthBak { get; set; }

            public override string ToString()
            {
                switch (Type)
                {
                    case DbLengthType.Fix:
                        return string.Format("({0}{1})", Length, LengthBak.HasValue ? "," + LengthBak : "");
                    case DbLengthType.Max:
                        return "(max)";
                    case DbLengthType.None:
                        return "";
                }
                return base.ToString();
            }

            public string GetLengthValue()
            {
                if (Length.HasValue)
                {
                    return Length.Value.ToString(CultureInfo.InvariantCulture);
                }
                return "";
            }
        }

        private enum DbLengthType
        {
            /// <summary>
            ///     无长度
            /// </summary>
            None,

            /// <summary>
            ///     定长
            /// </summary>
            Fix,

            /// <summary>
            ///     最大
            /// </summary>
            Max
        }

        #endregion

        #endregion
    }
}