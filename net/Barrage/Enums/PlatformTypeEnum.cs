using System.ComponentModel;

namespace Barrage.Enums
{
    /// <summary>
    /// 平台
    /// </summary>
    public enum PlatformTypeEnum
    {
        [Description("抖音")]
        Douyin = 1,

        [Description("Tiktok")]
        Tiktok,

        [Description("快手")]
        Kuaishou,

        [Description("哔哩哔哩")]
        Bilibili,

        [Description("小红书")]
        Xiaohongshu,

        [Description("视频号")]
        Shipinhao,
    }
}
