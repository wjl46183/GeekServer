using System.Diagnostics;
using Geek.Server.Core.Hotfix;
using Geek.Server.Core.Net.Tcp;
using System.Net.WebSockets;
using Geek.Server.Core.Actors;
using Geek.Server.Core.Events;
using Geek.Server.Core.Net.Session;
using Geek.Server.Core.Utils;

namespace Geek.Server.Core.Net.Websocket
{
    public class WebSocketConnectionHandler
    {
        static readonly NLog.Logger LOGGER = NLog.LogManager.GetCurrentClassLogger();

        public virtual async Task OnConnectedAsync(WebSocket socket, string clientAddress)
        {
            LOGGER.Info($"new websocket {clientAddress} connect...");
            WebSocketChannel channel = null;
            channel = new WebSocketChannel(socket, clientAddress, (msg) => _ = Dispatcher(channel, msg));
            await channel.StartAsync();
            OnDisconnection(channel);
        }

        protected virtual void OnDisconnection(NetChannel channel)
        {
            LOGGER.Debug($"{channel.RemoteAddress} 断开链接");
            var sessionId = channel.actorId;
            if (sessionId > 0)
            {
                SessionManager.Remove(sessionId);
            }
        }

        protected virtual async Task Dispatcher(NetChannel channel, BaseEvent msg)
        {
            if (msg == null)
                return;

            // 检查修正 Channel绑定Actor关系
            FixChannelBindState(channel);
            var sessionId = channel.actorId;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            await EventHandleMgr.Handle(sessionId, msg);
            sw.Stop();
            LOGGER.Debug($"[Message] {msg.TypeId} {msg.SerialId} {msg.GetType().FullName} Consumes Time: {sw.ElapsedMilliseconds}");
            channel.Write(msg);
        }

        /// <summary>
        /// 检查修正 Channel绑定Actor关系
        /// </summary>
        /// <param name="channel"></param>
        protected virtual void FixChannelBindState(NetChannel channel)
        {
            //未绑定的消息，由 Server 处理
            //未绑定的消息，由 Server 处理
            if (channel.actorId == 0)
            {
                channel.actorId = IdGenerator.GetActorID(ActorType.PhysicServer);
            }
        }
    }
}