using System.ComponentModel;
using SchemaBuilder.Svc.Core.QueryHelper.Data;
using SchemaBuilder.Svc.Models.Attr;

namespace SchemaBuilder.Svc.Models.Table;

[SearchContainer(nameof(StockBillVm))]
public class StockBillQuery
{
    [FieldEditor("代码", Span = 6, LabelColSpan = 6, WrapperColSpan = 18, Default = "sz002230")]
    public string? Code { get; set; }

    [FieldEditor("日期", Span = 6, LabelColSpan = 6, WrapperColSpan = 18, Default = "今天")]
    public DateQuery Date { get; set; }

    [FieldEditor("时间单位", Span = 6, LabelColSpan = 6, WrapperColSpan = 18)]
    public DateUnitType? DateUnit { get; set; }

    [FieldEditor("整数搜索", Span = 6, LabelColSpan = 6, WrapperColSpan = 18)]
    public IntQuery? IntValue { get; set; }

    [FieldEditor("小数搜索", Span = 6, LabelColSpan = 6, WrapperColSpan = 18)]
    public DecimalQuery? DecimalValue { get; set; }
}

public class UserSearchGroupQuery
{
    public long? Id { get; set; }
    public string? Path { get; set; }
}

public enum DateUnitType
{
    [Description("按秒")] Undefined = 0,
    [Description("1分钟")] Min1 = 1,
    [Description("2分钟")] Min2 = 2,
    [Description("3分钟")] Min3 = 3,
    [Description("5分钟")] Min5 = 5,
}

/// <summary>
/// 行情节点，高点，低点
/// </summary>
public class StockMarketNode
{
    public StockBillNodeType Type { get; set; }
    public DateTime Date { get; set; }
    public decimal Price { get; set; }
    public string Memo { get; set; }
}

/// <summary>
/// 节点判断
/// </summary>
public enum StockBillNodeType
{
    /**
     * 峰
     */
    Peek = 1,

    /**
     * 谷
     */
    LowEbb = 2,
}

public class StockHold
{
    public List<StockHoldItem> Items { get; set; } = new();
    public List<StockActionApply> Bills { get; set; } = new();
}

/// <summary>
/// 交易申请
/// </summary>
public class StockActionApply
{
    public DateTime Created { get; set; }

    /// <summary>
    /// 正数是加，负数是减
    /// </summary>
    public int Count { get; set; }

    public decimal Price { get; set; }
    public string Memo { get; set; }
}

/// <summary>
/// 持仓实体
/// </summary>
public class StockHoldItem
{
    public string Code { get; set; }
    public DateTime InDate { get; set; }
    public decimal Price { get; set; }
    public int Count { get; set; }
}

/**
 * 交易明细
 */
[FullTable(nameof(StockBillVm), TableType.MainTable, QueryDataUrl = "/api/stock/query")]
public class StockBillVm
{
    [TableColumn("Id")] public long Id { get; set; }

    [TableColumn("代码")] public string Code { get; set; }
    [TableColumn("名称")] public string Name { get; set; }
    [TableColumn("时间")] public DateTime Created { get; set; }
    [TableColumn("价格")] public decimal Price { get; set; }
    [TableColumn("之前价格")] public decimal PrevPrice { get; set; }

    /// <summary>
    /// 比数
    /// </summary>
    [TableColumn("比数")]
    public int Volume { get; set; }


    [TableColumn("金额(万)")] public decimal Amount { get; set; }

    /// <summary>
    /// U涨，D跌
    /// </summary>
    [TableColumn("类型")]
    public string Kind { get; set; }

    public string Hash { get; set; }
}

public class StockBill : ITable
{
    public long Id { get; set; }
    public DateTime Date { get; set; }
    public decimal Price { get; set; }
    public int HandCount { get; set; }

    /// <summary>
    /// B/S
    /// </summary>
    public string Type { get; set; }
}