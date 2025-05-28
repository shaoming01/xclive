using SchemaBuilder.Svc.Core.Aq;

namespace SchemaBuilder.Svc.Core.QueryHelper.Data;

/// <summary>
/// 查询范围
/// </summary>
public struct QueryIntRange : IQueryField
{
    public int? Begin { get; set; }
    public int? End { get; set; }

    public string GetSqlExpression(string colName, string provider = "")
    {
        if (Begin.HasValue && End.HasValue)
        {
            return string.Format("{0} Between {1} And {2}", colName, Begin.Value, End.Value);
        }
        else if (Begin.HasValue)
        {
            return string.Format("{0} > {1} ", colName, Begin.Value);
        }
        else if (End.HasValue)
        {
            return string.Format("{0} < {1} ", colName, End.Value);
        }

        return "1=1"; //必须要有个条件    }
    }
}