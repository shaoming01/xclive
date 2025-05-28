﻿using System.ComponentModel;

namespace Barrage.Enums
{
    /// <summary>
    /// 粉丝团消息类型
    /// </summary>
    public enum FansclubTypeEnum
    {
        [Description("粉丝团升级")]
        UpGrade = 1,

        [Description("加入粉丝团")]
        Join,
    }
}
