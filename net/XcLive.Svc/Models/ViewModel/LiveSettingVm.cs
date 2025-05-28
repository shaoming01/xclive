using SchemaBuilder.Svc.Models.Table;

namespace SchemaBuilder.Svc.Models.ViewModel;

/// <summary>
/// 互动消息回复设置
/// </summary>
public class LiveSettingVm : ISettingObject, ITenantId, IUserId
{
    /// <summary>
    /// 关注
    /// </summary>
    public string SocialReply { get; set; }

    /// <summary>
    /// 加团
    /// </summary>
    public string FansClubReply { get; set; }

    /// <summary>
    /// 点赞
    /// </summary>
    public string LikeReply { get; set; }

    /// <summary>
    /// 进入，来了
    /// </summary>
    public string MemberReply { get; set; }

    /// <summary>
    /// 礼物
    /// </summary>
    public string GiftReply { get; set; }

    public int InteractMsgCount { get; set; } = 3;
    public int ReplyMsgCount { get; set; } = 3;

    /// <summary>
    /// 随机插入语音
    /// </summary>
    public string InsertVoice { get; set; }

    public List<ReplySettingItem> ReplySetting { get; set; }

    public long TenantId { get; set; }

    /// <summary>
    /// 实现用户接口表示这个设置项是用户级的
    /// </summary>
    public long UserId { get; set; }


    public class ReplySettingItem
    {
        public string Name { get; set; }
        public string Content { get; set; }
        public List<ReplyMatchRule> MatchRules { get; set; }
    }

    public class ReplyMatchRule
    {
        public string Keyword { get; set; }
        public bool IsFuzzy { get; set; }
    }
}