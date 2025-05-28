using SchemaBuilder.Svc.Core.Cache;
using SchemaBuilder.Svc.Helpers;
using SchemaBuilder.Svc.Models.Table;

namespace SchemaBuilder.Svc.Svc;

public class StockWatchSvc
{
    public DateTime WatchDate { get; set; }
    public string Code { get; set; }
    public List<StockBillVm> CacheList = new();
    public int LastAnalyzeIndex = 0;
    public List<StockBillVm> DataSourceList = new();
    public List<StockHold> HoldList = new();
    public List<StockMarketNode> NodeList = new();
    private bool Exit = false;
    private Task _task;
    private Task _syncTask;

    public StockWatchSvc(string code, DateTime watchDate)
    {
        Code = code;
        WatchDate = watchDate;
    }

    public StockWatchSetting StockWatchSetting { get; set; } = new()
    {
        LossPercent = 0.01M,
        WinPercent = 0.01M,
        NodeCheckSpan = new TimeSpan(0, 5, 5),
        NodeCheckSwing = 0.003M,
        Frequency = 1,
    };


    public void Start()
    {
        IniHistoryDataSource();
        _syncTask = Task.Factory.StartNew(() =>
        {
            while (!Exit)
            {
                SyncLast(50);
                Sleep(600);
            }
        });

        _task = Task.Factory.StartNew(() =>
        {
            while (!Exit)
            {
                var firstBill = GetFirstBill();
                if (firstBill == null)
                {
                    Sleep(300);
                    continue;
                }

                CacheList.Add(firstBill);
                CalcNode();
            }
        });
    }

    private void SyncLast(int count)
    {
        var re = StockDataSvc.GetDistinctList(Code, WatchDate, 1, count);
        if (!re.Success)
        {
            Log4.Log.Error("获取线上数据失败" + re.Message);
            Output("获取线上数据失败" + re.Message);
            return;
        }

        lock (DataSourceList)
        {
            var extHash = DataSourceList.Select(item => item.Id).ToHashSet();
            var newList = re.Data.Where(newItem => !extHash.Contains(newItem.Id)).ToList();
            DataSourceList.AddRange(newList);
            Output("同步新数据" + newList.Count);
        }
    }

    private void CalcNode()
    {
        if (!CacheList.Has())
        {
            return;
        }

        var lastBill = CacheList.Last();
        var amount = lastBill.Amount.ToInt();
        var type = lastBill.Kind == "U" ? "\u2191" : "\u2193";
        Output($"开始分析{lastBill.Name}:{lastBill.Created:HH:mm:ss} 价格：{lastBill.Price.Cut0()} 金额：{amount}万 {type}");

        var interval = new TimeSpan(0, 0, 20);
        var span = new TimeSpan(0, 5, 0);
        var stablePercent = 0.003M;
        var shakePercent = 0.003M;
        var upPerCent = 0.005M;
        var downPerCent = 0.005M;


        var assRe = StockCalcCore.CalcAssess(CacheList, span, interval, stablePercent, shakePercent, upPerCent,
            downPerCent);
        Output(assRe.Type.GetEnumDescription());
    }


    private void IniHistoryDataSource()
    {
        var re = StockDataSvc.GetDistinctList(Code, WatchDate);
        if (!re.Success)
        {
            Output("获取线上数据失败" + re.Message);
            return;
        }

        lock (DataSourceList)
        {
            DataSourceList.Clear();
            LastAnalyzeIndex = 0;
            DataSourceList.AddRange(re.Data.OrderBy(d => d.Id));
            Output("同步历史数据" + DataSourceList.Count);
        }
    }

    private void GetNewBills()
    {
    }

    private StockBillVm? GetFirstBill()
    {
        lock (DataSourceList)
        {
            if (DataSourceList.Count <= LastAnalyzeIndex)
            {
                return null;
            }

            return DataSourceList[LastAnalyzeIndex++];
        }
    }

    public void Stop()
    {
        Exit = true;
    }

    public void Sleep(int miniSec)
    {
        var step = 10;
        if (miniSec <= step)
        {
            Thread.Sleep(miniSec);
            return;
        }

        int i = 0;
        while (!Exit && i++ * step < miniSec)
        {
            Thread.Sleep(step);
        }
    }

    private void Output(string text)
    {
        Log4.Log.Info(text);
    }
}

public class StockWatchSetting
{
    /// <summary>
    /// 频率,默认1秒
    /// </summary>
    public decimal Frequency { get; set; }

    /// <summary>
    /// 止损点
    /// </summary>
    public decimal LossPercent { get; set; }

    /// <summary>
    /// 止赢点
    /// </summary>
    public decimal WinPercent { get; set; }

    /// <summary>
    /// 稳定时长
    /// </summary>
    public TimeSpan NodeCheckSpan { get; set; }

    /// <summary>
    /// 振幅
    /// </summary>
    public decimal NodeCheckSwing { get; set; }
}