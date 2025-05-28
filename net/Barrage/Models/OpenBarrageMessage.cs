using Barrage.Enums;

namespace Barrage.Models
{
    /// <summary>
    /// 弹幕消息实体模型
    /// </summary>
    public class OpenBarrageMessage
    {
        public MessageTypeEnum Type { get; set; }

        public object? Data { get; set; }
    }
}
