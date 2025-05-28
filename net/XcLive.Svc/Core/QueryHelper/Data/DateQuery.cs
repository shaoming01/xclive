using SchemaBuilder.Svc.Core.Aq;
using SchemaBuilder.Svc.Core.Ext;
using SchemaBuilder.Svc.Helpers;
using ServiceStack;

namespace SchemaBuilder.Svc.Core.QueryHelper.Data;

public class DateQuery : IQueryField
{
    /// <summary>
    /// 格式：今天、昨天、2021-1-1~、~2021-1-1
    /// </summary>
    public string? ComplexValue { get; set; }

    public DateTime? Start => CalcStartEnd().Item1;

    public DateTime? End => CalcStartEnd().Item2;

    public Tuple<DateTime?, DateTime?> CalcStartEnd()
    {
        if (string.IsNullOrWhiteSpace(ComplexValue))
        {
            return new Tuple<DateTime?, DateTime?>(null, null);
        }

        if (ComplexValue.Contains("~"))
        {
            var arr = ComplexValue.Split("~");
            if (arr.Length != 2)
            {
                return new Tuple<DateTime?, DateTime?>(null, null);
            }

            return new Tuple<DateTime?, DateTime?>(arr[0].ToDate(), arr[1].ToDate());
        }

        switch (ComplexValue)
        {
            case "昨天":
                return new Tuple<DateTime?, DateTime?>(DateTime.Today.AddDays(-1), DateTime.Today);
            case "今天":
                return new Tuple<DateTime?, DateTime?>(DateTime.Today, null);
            case "近3天":
                return new Tuple<DateTime?, DateTime?>(DateTime.Today.AddDays(-2), null);
            case "近7天":
                return new Tuple<DateTime?, DateTime?>(DateTime.Today.AddDays(-6), null);
            case "近30天":
                return new Tuple<DateTime?, DateTime?>(DateTime.Today.AddDays(-29), null);
            case "本周":
            {
                int diff = (int)DateTime.Today.DayOfWeek - 1; // 计算与星期一的差值 (星期一为0)
                if (diff < 0) diff = 6; // 如果是星期天，调整为上一周的星期一

                return new Tuple<DateTime?, DateTime?>(DateTime.Today.AddDays(-diff), null);
            }
            case "本月":
                return new Tuple<DateTime?, DateTime?>(new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1),
                    null);
            case "上月":
                return new Tuple<DateTime?, DateTime?>(
                    new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1),
                    new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1));
            case "近3月":
                return new Tuple<DateTime?, DateTime?>(
                    new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-3), null);
            case "近12月":
                return new Tuple<DateTime?, DateTime?>(
                    new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-12), null);
            case "今年":
                return new Tuple<DateTime?, DateTime?>(new DateTime(DateTime.Today.Year, 1, 1), null);
            case "去年":
                return new Tuple<DateTime?, DateTime?>(new DateTime(DateTime.Today.Year, 1, 1).AddYears(-1),
                    new DateTime(DateTime.Today.Year, 1, 1));
        }

        return new Tuple<DateTime?, DateTime?>(null, null);
    }

    public string GetSqlExpression(string colName, string provider = "")
    {
        if (Start.HasValue && End.HasValue)
        {
            return string.Format("{0} Between {1} And {2}", colName, Start.ToSqlPar(), End.ToSqlPar());
        }
        else if (Start.HasValue)
        {
            return string.Format("{0} > {1} ", colName, Start.ToSqlPar());
        }
        else if (End.HasValue)
        {
            return string.Format("{0} < {1} ", colName, End.ToSqlPar());
        }

        return "1=1"; //必须要有个条件
    }
}

public class IntQuery : QueryBetween<int>
{
}

public class DecimalQuery : QueryBetween<int>
{
}

public class QueryBetween<T> : IQueryField
    where T : struct
{
    public T? Start { get; set; }
    public T? End { get; set; }


    public string GetSqlExpression(string colName, string provider = "")
    {
        if (Start.HasValue && End.HasValue)
        {
            return string.Format("{0} Between {1} And {2}", colName, Start.Value.ToString().ToSqlPar(),
                End.Value.ToString().ToSqlPar());
        }
        else if (Start.HasValue)
        {
            return string.Format("{0} > {1} ", colName, Start.Value.ToString().ToSqlPar());
        }
        else if (End.HasValue)
        {
            return string.Format("{0} < {1} ", colName, End.Value.ToString().ToSqlPar());
        }

        return "1=1"; //必须要有个条件
    }
}