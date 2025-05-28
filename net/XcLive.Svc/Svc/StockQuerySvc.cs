using SchemaBuilder.Svc.Core;
using SchemaBuilder.Svc.Core.Ext;
using SchemaBuilder.Svc.Core.QueryHelper.Data;
using SchemaBuilder.Svc.Helpers;
using SchemaBuilder.Svc.Models.Table;

namespace SchemaBuilder.Svc.Svc;

public abstract class StockQuerySvc
{
    public static R<List<StockBillVm>> QueryList(PageQueryObject<StockBillQuery> query)
    {
        var re = new R<List<StockBillVm>>();
        if (query.QueryObject == null)
        {
            return R.Faild<List<StockBillVm>>("QueryObject不能为空");
        }

        if (!query.QueryObject.Code.Has())
        {
            return R.Faild<List<StockBillVm>>("代码不能为空");
        }

        if (query.QueryObject.Date.ComplexValue.IsNullOrEmpty())
        {
            return R.Faild<List<StockBillVm>>("日期不能为空");
        }

        var start = query.QueryObject.Date.Start.ToDate() ?? DateTime.Now.AddYears(-1);
        var end = query.QueryObject.Date.End.ToDate() ?? DateTime.Now;
        if (start.Date > end.Date)
        {
            return R.Faild<List<StockBillVm>>("结束日期需要小于开始日期");
        }

        var days = (end.Date - start.Date).TotalDays + 1;
        var list = new List<StockBillVm>();
        for (int i = 0; i < days; i++)
        {
            var getRe = StockDataSvc.GetDistinctList(query.QueryObject.Code ?? "", start.AddDays(i));
            if (!getRe.Success)
            {
                return R.Faild<List<StockBillVm>>(getRe.Message);
            }

            list.AddRange(getRe.Data);
        }

        return R.OK(list);
    }
}