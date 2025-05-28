using Barrage.Enums;

namespace Barrage.Models.Douyin
{
    /// <summary>
    /// 抖音分享消息
    /// </summary>
    public class DouyinMsgShare : DouyinMsgBase
    {
        /// <summary>
        /// 分享目标
        /// </summary>
        public ShareTypeEnum ShareType { get; set; }

        public string ShareTarget { get; set; }
    }
}
