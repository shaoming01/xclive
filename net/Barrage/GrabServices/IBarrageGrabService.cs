using System.Net;
using Barrage.Models;

namespace Barrage.GrabServices
{
    /// <summary>
    /// barrage grab service interface
    /// </summary>
    internal interface IBarrageGrabService
    {
        void Start(string liveId, Cookie[] cookies);

        void Stop();

        void ReStart();

        event EventHandler<WebRoomInfo>? OnOpen;

        event EventHandler<OpenBarrageMessage>? OnMessage;

        event EventHandler<Exception>? OnError;

        event EventHandler? OnClose;
    }
}