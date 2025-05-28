namespace Barrage.Models.Douyin
{
    /// <summary>
    /// 抖音加入直播间消息
    /// </summary>
    public class DouyinMsgMember : DouyinMsgBase
    {
        /// <summary>
        /// 当前直播间人数
        /// </summary>
        public long MemberCount { get; set; }
    }
}
