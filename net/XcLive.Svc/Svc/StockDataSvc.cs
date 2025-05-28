using System.Net;
using System.Text;
using SchemaBuilder.Svc.Core;
using SchemaBuilder.Svc.Core.Ext;
using SchemaBuilder.Svc.Helpers;
using SchemaBuilder.Svc.Models.Table;
using ServiceStack.Text;

namespace SchemaBuilder.Svc.Svc;

public abstract class StockDataSvc
{
    public static List<StockBillVm> Distinct(List<StockBillVm> list)
    {
        var relist = list.GroupBy(l => l.Created).Select(g =>
        {
            var fisrt = g.First();
            fisrt.Price = g.Average(gl => gl.Price).CutDecimal();
            fisrt.Amount = g.Sum(gl => gl.Amount);
            fisrt.Volume = g.Sum(gl => gl.Volume);
            fisrt.Id = fisrt.Created.Ticks;
            return fisrt;
        }).ToList();
        return relist;
    }

    public static R<List<StockBillVm>> GetDistinctList(string code, DateTime queryDt, int page = 1,
        int pageSize = 12000)
    {
        var result = GetList(code, queryDt, page, pageSize);
        if (result.Success && result.Data.Has())
        {
            result.Data = Distinct(result.Data);
        }

        return result;
    }

    public static R<List<StockBillVm>> GetList(string code, DateTime queryDt, int page = 1, int pageSize = 12000)
    {
        var parStr =
            $"symbol={code}&num={pageSize}&page={page}&sort=ticktime&asc=0&volume=0&amount=0&type=0&day={queryDt:yyyy-MM-dd}";

        var url = "https://vip.stock.finance.sina.com.cn/quotes_service/api/json_v2.php/CN_Bill.GetBillList?" + parStr;
        var refe = "https://vip.stock.finance.sina.com.cn/quotes_service/view/cn_bill.php";
        var handler = new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        };
        var client = new HttpClient(handler);
        client.DefaultRequestHeaders.Referrer = new Uri(refe);
        var response = client.GetAsync(url).Result;

        string content;
        using (var responseStream = response.Content.ReadAsStreamAsync().Result)
        using (var reader = new StreamReader(responseStream, Encoding.UTF8)) // 明确指定编码
        {
            content = reader.ReadToEnd();
        }

        if (!content.IsJson())
        {
            return R.Faild<List<StockBillVm>>("获取内容异常:" + content);
        }

        var list = JsonSerializer.DeserializeFromString<List<billDto>>(content);
        var relist = list.Select(l => ConvertToRe(queryDt, l)).ToList();
        return R.OK(relist);
    }

    private static StockBillVm ConvertToRe(DateTime dt, billDto billDto)
    {
        var time = billDto.ticktime.ToDate() ?? DateTime.MinValue;
        var dt2 = dt.Date;
        dt2 = dt2.AddHours(time.Hour);
        dt2 = dt2.AddMinutes(time.Minute);
        dt2 = dt2.AddSeconds(time.Second);

        var price = billDto.price.ToDecimal() ?? 0;
        var count = billDto.volume.ToInt() ?? 0;
        return new StockBillVm()
        {
            Id = Id.NewId(),
            Created = dt2,
            Name = billDto.name,
            Amount = (count * price) / 10000,
            Code = billDto.symbol,
            Kind = billDto.kind,
            Price = price,
            PrevPrice = billDto.prev_price.ToDecimal() ?? 0,
            Volume = count,
        };
    }

    public static R<int> GetCount(string code, DateTime queryDt, int page = 1, int pageSize = 12000)
    {
        var parStr =
            $"symbol={code}&num={pageSize}&page={page}&sort=ticktime&asc=0&volume=0&amount=0&type=0&day={queryDt:yyyy-MM-dd}";
        var url = "https://vip.stock.finance.sina.com.cn/quotes_service/api/json_v2.php/CN_Bill.GetBillListCount?" +
                  parStr;
        var refe = "https://vip.stock.finance.sina.com.cn/quotes_service/view/cn_bill.php";
        var client = new HttpClient();
        client.DefaultRequestHeaders.Referrer = new Uri(refe);
        var x = client.GetAsync(url).Result;

        var content = x.Content.ReadAsStringAsync().Result;
        content = content.Replace("\"", "");
        return R.OK(content.ToInt() ?? 0);
    }

    public class billDto
    {
        public string kind { get; set; }

        public string name { get; set; }

        public string prev_price { get; set; }

        public string price { get; set; }

        public string symbol { get; set; }

        public string ticktime { get; set; }
        public string volume { get; set; }
    }
}