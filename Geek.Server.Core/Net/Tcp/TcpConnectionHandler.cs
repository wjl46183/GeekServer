using Geek.Server.Core.Events;
using Geek.Server.Core.Hotfix;
using MemoryPack;
using Microsoft.AspNetCore.Connections;

namespace Geek.Server.Core.Net.Tcp
{
    public class TcpConnectionHandler : ConnectionHandler
    {
        static readonly NLog.Logger LOGGER = NLog.LogManager.GetCurrentClassLogger();

        public TcpConnectionHandler() { }

        public override async Task OnConnectedAsync(ConnectionContext connection)
        {
            // LOGGER.Debug($"{connection.RemoteEndPoint?.ToString()} 链成功");
            NetChannel channel = null;
            channel = new TcpChannel(connection, async (msg) => await Dispatcher(channel, msg));
            await channel.StartAsync();
            // LOGGER.Debug($"{channel.RemoteAddress} 断开链接");
            OnDisconnection(channel);
        } 

        protected virtual void OnDisconnection(NetChannel channel)
        {
        }

        protected async Task Dispatcher(NetChannel channel, Message msg)
        {
            if (msg == null)
                return;

           // LOGGER.Debug($"-------------收到消息{msg.MsgId} {msg.GetType()}");
           
           var sessionId = channel.GetData<long>("SESSION_ID");
           await EventHandleMgr.Handle(sessionId, msg);
           channel.Write(msg);
            var handler = HotfixMgr.GetTcpHandler(msg.TypeId);
            if (handler == null)
            {
                LOGGER.Error($"找不到[{msg.TypeId}][{msg.GetType()}]对应的handler");
                return;
            }
            handler.Msg = msg;
            handler.Channel = channel;
            await handler.Init();
            await handler.InnerAction();
        }
    }
}