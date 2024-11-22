using Geek.Server.Core.Net;
using Geek.Server.Core.Net.Websocket;
using Geek.Server.Main.Common.Session;

namespace Geek.Server.Main.Common.Net
{
    public class AppWebSocketConnectionHandler : WebSocketConnectionHandler
    {
        public override void OnDisconnection(NetChannel channel)
        {
            base.OnDisconnection(channel);
            var sessionId = channel.GetData<long>(SessionManager.SESSIONID);
            if (sessionId > 0)
                SessionManager.Remove(sessionId);
        }
    }
}
