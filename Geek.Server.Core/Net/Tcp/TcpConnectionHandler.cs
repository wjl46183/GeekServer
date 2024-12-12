using System.Diagnostics;
using Geek.Server.Core.Actors;
using Geek.Server.Core.Events;
using Geek.Server.Core.Events.Datas;
using Geek.Server.Core.Hotfix;
using Geek.Server.Core.Net.Session;
using Geek.Server.Core.Utils;
using MemoryPack;
using Microsoft.AspNetCore.Connections;

namespace Geek.Server.Core.Net.Tcp
{
    public class TcpConnectionHandler : ConnectionHandler
    {
        static readonly NLog.Logger LOGGER = NLog.LogManager.GetCurrentClassLogger();

        public override async Task OnConnectedAsync(ConnectionContext connection)
        {
            LOGGER.Debug($"{connection.RemoteEndPoint?.ToString()} 链成功");
            NetChannel channel = null;
            channel = new TcpChannel(connection, async (msg) => await OnDispatcher(channel, msg));
            await channel.StartAsync();
            LOGGER.Debug($"{channel.RemoteAddress} 断开链接");
            OnDisconnection(channel);
        }

        protected virtual void OnDisconnection(NetChannel channel)
        {
            var sessionId = channel.actorId;
            if (sessionId > 0)
            {
                SessionManager.Remove(sessionId);
            }
        }

        protected async Task OnDispatcher(NetChannel channel, BaseEvent msg)
        {
            if (msg == null)
                return;

            //初始化协议必须是 EventLinkCheck 的字类型，否则不接受并且断掉链接
            if (channel.actorId == 0)
            {
                var check = msg as EventLinkCheck;
                if (check == null)
                {
                    channel.Write(ResErrorCode.Create(-1, "初始化协议必须是 EventLinkCheck 的字类型"));
                    channel.Close();
                    return;
                }
                else
                {
                    FixChannelBindState(channel);
                    check.cacheSessionSerialId = channel.SerialId;
                }
            }
            
            // 处理消息
            var actorId = channel.actorId;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            await EventHandleMgr.Handle(actorId, msg);
            sw.Stop();
            LOGGER.Debug($"[处理消息] {msg.TypeId} {msg.SerialId} {msg.GetType().FullName} {sw.ElapsedMilliseconds}");
            channel.Write(msg);
        }

        /// <summary>
        /// 检查修正 Channel绑定Actor关系
        /// </summary>
        /// <param name="channel"></param>
        protected virtual void FixChannelBindState(NetChannel channel)
        {
            //未绑定的消息，由 Server 处理
            if (channel.actorId == 0)
            {
                var session = Session.Session.Create();
                channel.actorId = IdGenerator.GetActorID(ActorType.PhysicServer);
                channel.SerialId = session.SerialId;
                session.Id = channel.actorId;
                session.Time = DateTime.Now;
                session.Channel = channel;
                SessionManager.AddCache(session);
                
            }
        }
    }
}