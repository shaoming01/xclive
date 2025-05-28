﻿namespace Barrage.Models.Douyin
{
    /// <summary>
    /// 抖音粉丝团消息
    /// </summary>
    public class DouyinMsgFansClub : DouyinMsgBase
    {
        /// <summary>
        /// 粉丝团消息类型,升级1，加入2
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 粉丝团等级
        /// </summary>
        public int Level { get; set; }
    }
}
