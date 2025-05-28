using SchemaBuilder.Svc.Helpers;

namespace SchemaBuilder.Svc.Core.Ext;

public static class SqlExt
{
    public static string ToSqlPar(this DateTime dt)
    {
        return string.Format("'{0:yyyy-MM-dd HH:mm:ss}'", (object)dt);
    }

    public static string ToSqlPar(this DateTime? dt) => !dt.HasValue ? "NULL" : dt.Value.ToSqlPar();

    /// <summary>转换成</summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    public static string ToSqlPar(this int[] ids)
    {
        return string.Format("'{0}'", (object)string.Join<int>("','", (IEnumerable<int>)ids));
    }

    /// <summary>转换成</summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    public static string ToSqlPar(this long[] ids)
    {
        return string.Format("'{0}'", (object)string.Join<long>("','", (IEnumerable<long>)ids));
    }

    public static string ToStrSpiltSqlPar(this long[] ids)
    {
        return ((IEnumerable<long>)ids).Count<long>() > 2000
            ? string.Format("'{0}'", (object)string.Join<long>("','", (IEnumerable<long>)ids))
            : string.Format("SELECT value FROM STRING_SPLIT('{0}', ',')",
                (object)string.Join<long>(",", (IEnumerable<long>)ids));
    }

    public static string ToSqlPar(this IEnumerable<int> ids)
    {
        if (!(ids is int[] values))
            values = ids.ToArray<int>();
        return string.Format("'{0}'", (object)string.Join<int>("','", (IEnumerable<int>)values));
    }

    public static string ToSqlPar(this IEnumerable<string> values)
    {
        if (!(values is string[] source))
            source = values.ToArray<string>();
        return string.Format("'{0}'",
            (object)string.Join("','",
                ((IEnumerable<string>)source).Select<string, string>(
                    (Func<string, string>)(s => !s.IsNullOrEmpty() ? s.Replace("'", "''") : string.Empty))));
    }

    public static string ToSqlPar(this IEnumerable<long> ids)
    {
        if (!(ids is long[] values))
            values = ids.ToArray<long>();
        return string.Format("'{0}'", (object)string.Join<long>("','", (IEnumerable<long>)values));
    }

    public static string ToStrSpiltSqlPar(this IEnumerable<long> ids)
    {
        if (!(ids is long[] numArray1))
            numArray1 = ids.ToArray<long>();
        long[] numArray2 = numArray1;
        return ((IEnumerable<long>)numArray2).Count<long>() > 2000
            ? string.Format("'{0}'", (object)string.Join<long>("','", (IEnumerable<long>)numArray2))
            : string.Format("SELECT value FROM STRING_SPLIT('{0}', ',')",
                (object)string.Join<long>(",", (IEnumerable<long>)numArray2));
    }

    public static string ToSqlPar(this List<int> ids)
    {
        return string.Format("'{0}'", (object)string.Join<int>("','", (IEnumerable<int>)ids));
    }

    public static string ToSqlPar(this List<long> ids)
    {
        return string.Format("'{0}'", (object)string.Join<long>("','", (IEnumerable<long>)ids));
    }

    public static string ToSqlPar(this long id) => string.Format("'{0}'", (object)id);

    public static string ToSqlPar(this string? val)
    {
        return val.IsNullOrEmpty() ? "''" : string.Format("'{0}'", val.Replace("'", "''"));
    }

    public static string ToSqlPar(this bool value) => value ? "1" : "0";

    public static int ToSqlPar(this Enum value) => (int)value.ToInt();

    public static string ToSqlParLike(this string val)
    {
        return val.IsNullOrEmpty()
            ? "''"
            : string.Format("'%{0}%'",
                (object)val.Replace("'", "''").Replace("[", "[[]").Replace("%", "[%]").Replace("_", "[_]"));
    }
}