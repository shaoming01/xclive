using System.ComponentModel;
using SchemaBuilder.Svc.Helpers;
using SchemaBuilder.Svc.Models.Table;

namespace SchemaBuilder.Svc.Svc;

public static class StockCalcCore
{
    public static AssessResult CalcAssess(List<StockBillVm> bills,
        TimeSpan span, TimeSpan interval,
        decimal stablePercent,
        decimal shakePercent,
        decimal upPerCent,
        decimal downPerCent)
    {
        if (!bills.Has() || span <= TimeSpan.Zero || interval <= TimeSpan.Zero)
        {
            return new AssessResult();
        }

        var lastBill = bills.Last();
        var calcList = bills.Where(b => b.Created <= lastBill.Created.Add(span)).ToList();
        var valList = SplitByDate(calcList, interval);
        var min = valList.Min(l => l.Avg);
        var minIndex = valList.FindIndex(l => l.Avg == min);
        var minBills = valList.First(l => l.Avg == min).Bills;

        var max = valList.Max(l => l.Avg);
        var maxIndex = valList.FindIndex(l => l.Avg == max);
        var maxBills = valList.First(l => l.Avg == max).Bills;

        var totalPercent = (max - min) / max; //最大落差
        if (maxIndex > minIndex && totalPercent > upPerCent) //涨过涨幅
        {
            if (maxBills.Contains(lastBill)) //峰顶
            {
                return new AssessResult
                {
                    Duration = lastBill.Created - minBills.First().Created,
                    Type = AssessResultType.Peek,
                };
            }

            if ((max - lastBill.Price) / max < stablePercent) //峰转
            {
                return new AssessResult
                {
                    Duration = lastBill.Created - maxBills.First().Created,
                    Type = AssessResultType.PeekTurn,
                };
            }

            //过了峰顶，又下来了
            return new AssessResult
            {
                Duration = lastBill.Created - maxBills.First().Created,
                Type = AssessResultType.Down,
            };
        }

        if (maxIndex < minIndex && totalPercent > downPerCent) //跌过跌幅
        {
            if (minBills.Contains(lastBill)) //谷底
            {
                return new AssessResult
                {
                    Duration = lastBill.Created - maxBills.First().Created,
                    Type = AssessResultType.Low,
                };
            }

            if ((lastBill.Price - max) / max < stablePercent) //谷转
            {
                return new AssessResult
                {
                    Duration = lastBill.Created - minBills.First().Created,
                    Type = AssessResultType.LowTurn,
                };
            }

            //过了峰顶，又下来了
            return new AssessResult
            {
                Duration = lastBill.Created - minBills.First().Created,
                Type = AssessResultType.Up,
            };
        }

        return new AssessResult
        {
            Duration = TimeSpan.Zero,
            Type = AssessResultType.Shake,
        };
    }

    private static List<PriceSpan> SplitByDate(List<StockBillVm> list, TimeSpan interval)
    {
        var lastDate = list.Last().Created;
        var priceGroup = list.GroupBy(l =>
        {
            var groupIndex = (int)(lastDate - l.Created).TotalSeconds / interval.TotalSeconds;
            return groupIndex;
        }).Select(g =>
        {
            return new PriceSpan
            {
                Avg = g.Average(l => l.Price),
                Bills = g.ToList(),
            };
        }).ToList();
        return priceGroup;
    }
}

public class PriceSpan
{
    public decimal Avg { get; set; }
    public List<StockBillVm> Bills { get; set; }
}

public class AssessResult
{
    public AssessResultType Type { get; set; }
    public TimeSpan Duration { get; set; }
}

public enum AssessResultType
{
    [Description("空")] Undefined = 0,
    [Description("峰点")] Peek = 1, //峰点
    [Description("峰转")] PeekTurn = 2, //峰转
    [Description("谷点")] Low = 3, //谷点
    [Description("谷转")] LowTurn = 4, //谷转
    [Description("走高")] Up = 5, //走高
    [Description("走低")] Down = 6, //走低
    [Description("振动")] Shake = 7, //振动
}