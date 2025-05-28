﻿namespace Barrage.Models.Douyin
{
    /// <summary>
    /// 抖音直播间统计消息
    /// </summary>
    public class DouyinMsgRoomUserSeq : DouyinMsgBase
    {
        /// <summary>
        /// 当前直播间用户数量
        /// </summary>
        public long OnlineUserCount { get; set; }

        /// <summary>
        /// 累计直播间用户数量
        /// </summary>
        public long TotalUserCount { get; set; }

        /// <summary>
        /// 累计直播间用户数量 显示文本
        /// </summary>
        public string? TotalUserCountStr { get; set; }

        /// <summary>
        /// 当前直播间用户数量 显示文本
        /// </summary>
        public string? OnlineUserCountStr { get; set; }
    }
}
