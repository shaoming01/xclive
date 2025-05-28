using Barrage.Enums;

namespace Barrage.EventArgs
{
    public class RoomMessageEventArgs<T> : System.EventArgs where T : class
    {
        /// <summary>
        /// 消息类型
        /// </summary>
        public MessageTypeEnum? Type { get; set; }

        /// <summary>
        /// 消息
        /// </summary>
        public T? Message { get; set; }


        public RoomMessageEventArgs()
        {

        }

        public RoomMessageEventArgs(MessageTypeEnum? type, T message)
        {
            this.Type = type;
            this.Message = message;
        }
    }
}
