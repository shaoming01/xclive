using Barrage.EventArgs;

namespace Barrage.Handler
{
    public delegate void RoomMessageEventHandler(object? sender, RoomMessageEventArgs<object> e);
}
