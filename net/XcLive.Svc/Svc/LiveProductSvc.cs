using Newtonsoft.Json.Linq;
using SchemaBuilder.Svc.Core;
using SchemaBuilder.Svc.Core.Ext;
using SchemaBuilder.Svc.Helpers;
using SchemaBuilder.Svc.Models.Table;
using ServiceStack.OrmLite;

namespace SchemaBuilder.Svc.Svc;

public class LiveProductSvc
{
    public static R<string> GetProductDescription(long liveAccountProductId)
    {
        using var db = Db.Open();
        var product = db.SingleById<LiveAccountProduct>(liveAccountProductId);
        if (product == null) return R.Faild<string>("产品不存在");
        var proJson = product.ProductJson;
        if (!proJson.IsJson())
        {
            return R.Faild<string>("产品配置无效");
        }

        var jObject = JObject.Parse(proJson);
        //主图
        var mainImgToken = jObject.SelectToken("h5Data.data.promotion_h5.head_figure_data.media_list") as JArray;
        var mainImgList = new List<string>();
        mainImgToken?.Where(token => token.SelectToken("type")?.Value<string>() == "image").ForEach(token =>
        {
            var contentList = token?.SelectToken("content_list") as JArray;
            contentList?.ForEach(contentToken =>
            {
                var url = contentToken.SelectToken("url")?.Value<string>();
                if (url.Has())
                {
                    mainImgList.Add(url);
                }
            });
        });
        var mainText = GetImageText(product.ProductName!, mainImgList);
        if (!mainText.Success)
        {
            return R.Faild<string>(mainText.Message);
        }

        //价格
        var priceText = "";
        var priceToken = jObject.SelectToken("h5Data.data.promotion_h5.basic_info_data.price_info.price");
        if (priceToken != null)
        {
            var price = priceToken.SelectToken("min_price")?.Value<string>();
            var suffix = priceToken.SelectToken("suffix")?.Value<string>();
            priceText = $"{price}{suffix},";
        }

        var discountToken = jObject.SelectToken("h5Data.data.promotion_h5.basic_info_data.price_info.discount_price");
        if (discountToken != null)
        {
            var price = discountToken.SelectToken("min_price")?.Value<string>();
            var suffix = discountToken.SelectToken("suffix")?.Value<string>();
            var prefix = discountToken.SelectToken("prefix")?.Value<string>();
            priceText += $"{prefix}{price}{suffix};";
        }

        //保障内容
        var safetyContent = new List<string>();
        var safetyContentToken =
            jObject.SelectToken("h5Data.data.promotion_h5.product_support_info_data.safety_content.content_list") as
                JArray;
        safetyContentToken?.ForEach(c =>
        {
            var coms = c.SelectToken("coms") as JArray;
            coms?.ForEach(com =>
            {
                var text = com.SelectToken("text")?.Value<string>() ?? "";
                safetyContent.Add(text);
            });
        });

        //优惠活动
        var rewardContent = new List<string>();
        var rewardContentToken =
            jObject.SelectToken("h5Data.data.promotion_h5.product_support_info_data.reward_content.content_list") as
                JArray;
        rewardContentToken?.ForEach(c =>
        {
            var coms = c.SelectToken("coms") as JArray;
            coms?.Where(com => com.SelectToken("type")?.Value<string>() == "text").ForEach(com =>
            {
                var text = com.SelectToken("text")?.Value<string>() ?? "";
                rewardContent.Add(text);
            });
        });
        //物流
        var logisticsContent = new List<string>();
        var logisticsContentToken =
            jObject.SelectToken("h5Data.data.promotion_h5.product_support_info_data.logistics_content.content_list") as
                JArray;
        logisticsContentToken?.ForEach(c =>
        {
            var coms = c.SelectToken("coms") as JArray;
            coms?.Where(com => com.SelectToken("type")?.Value<string>() == "text").ForEach(com =>
            {
                var text = com.SelectToken("text")?.Value<string>() ?? "";
                logisticsContent.Add(text);
            });
        });

        //标签
        var tags = new List<string>();
        var tagsToken = jObject.SelectToken("h5Data.data.promotion_h5.comment_data.good_comment.tag_list") as JArray;
        tagsToken?.ForEach(tag =>
        {
            var text = tag.SelectToken("text")?.Value<string>() ?? "";
            var num = tag.SelectToken("num")?.Value<int>() ?? 0;
            tags.Add(text + num + "人");
        });

        //商品属性
        var productAttr = new List<string>();
        var productAttrToken = jObject.SelectToken("pageDetail.data.detail_info.product_format") as JArray;
        productAttrToken?.ForEach(attr =>
        {
            var format = attr.SelectToken("format") as JArray;
            format?.ForEach(prop =>
            {
                var name = prop.SelectToken("name")?.Value<string>() ?? "";
                var values = new List<string>();
                var message = prop.SelectToken("message") as JArray;
                message?.ForEach(m =>
                {
                    var desc = m.SelectToken("desc")?.Value<string>() ?? "";
                    values.Add(desc);
                });
                productAttr.Add(name + "：" + string.Join(",", values));
            });
        });
        //详情图
        var detailImgList = new List<string>();
        var detailImgToken = jObject.SelectToken("pageDetail.data.detail_info.detail_imgs") as JArray;
        detailImgToken?.ForEach(img =>
        {
            var urls = img.SelectToken("url_list") as JArray;
            var url = urls?.FirstOrDefault()?.Value<string>();
            if (url.Has())
            {
                detailImgList.Add(url);
            }
        });
        var detailText = GetImageText(product.ProductName!, detailImgList);
        if (!detailText.Success)
        {
            return R.Faild<string>(detailText.Message);
        }

        var summaryText =
            $"商品名称：{product.ProductName};价格：{priceText};消费保障：{string.Join(",", safetyContent)};优惠活动：{string.Join(",", rewardContent)};物流：{string.Join(",", logisticsContent)};标签：{string.Join(",", tags)};商品属性：{string.Join(",", productAttr)};主图描述：{mainText.Data};详情图描述：{detailText.Data};";

        return R.OK(summaryText);
    }

    private static R<string> GetImageText(string productProductName, List<string> detailImgList)
    {
        var groupSplitCount = 20;
        using var db = Db.Open();
        var model = db.Select<ModelAuthInfo>().FirstOrDefault();
        if (model == null)
        {
            return R.Faild<string>("model不能为空");
        }

        var userText =
            $"这是一个名称为：'{productProductName}',我提供是抖音直播商品的图片，帮我详细分析一下这些图片的内容，要求：口语化，通俗易懂，不要有特殊字符，不要emoji的表情";
        var groups = detailImgList.Split(groupSplitCount);
        if (groups.Count > 1 && groups.Last().Count < groupSplitCount) //合并最后2组
        {
            groups[groups.Count - 2].AddRange(groups.Last());
            groups.RemoveAt(groups.Count - 1);
        }

        var textList = new List<string>();
        foreach (var group in groups)
        {
            var promptRe = ModelApiHelper.ImagePrompt(model, userText, group);
            if (!promptRe.Success)
            {
                return R.Faild<string>(promptRe.Message);
            }

            textList.Add(promptRe.Data);
        }


        return R.OK(string.Join(",", textList));
    }
}