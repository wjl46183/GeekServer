
using Geek.Server.Main.Common.Net;
using Geek.Server.Main.Common.Session;
using Geek.Server.Core.Actors;
using Geek.Server.Core.Comps;
using Geek.Server.Core.Events;
using Geek.Server.Core.Hotfix;
using Geek.Server.Core.Net;
using Geek.Server.Core.Net.Http;
using Geek.Server.Core.Net.Tcp;
using Geek.Server.Core.Net.Websocket;
using Geek.Server.Core.Timer;
using Geek.Server.Core.Utils;
using Microsoft.AspNetCore.Connections;
using Server.Logic.Logic.Login;
using Server.Logic.Logic.Role.Base;

namespace Server.Logic.Common
{
    internal class HotfixBridge : IHotfixBridge
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public ServerType BridgeType => ServerType.Game;

        public static async Task OnHandleLoginEvent(long actorId, Message evt)
        {
            var login = evt as ReqLogin;
            
            //单个（优先级）
            await (await ActorMgr.GetCompAgent<LoginCompAgent>(actorId)).OnLogin(login);

            //多个（相同优先级）
            await Task.WhenAll(new []{
                (await ActorMgr.GetCompAgent<LoginCompAgent>(actorId)).OnLogin(login),
                (await ActorMgr.GetCompAgent<LoginCompAgent>(actorId)).OnLogin(login),
                (await ActorMgr.GetCompAgent<LoginCompAgent>(actorId)).OnLogin(login),
            });
            
        }

        public async Task<bool> OnLoadSuccess(bool reload)
        {
            if (reload)
            {
                ActorMgr.ClearAgent();
                return true;
            }
            HotfixMgr.SetMsgGetter(Geek.Server.HotData.MemoryPackTypeMapping.GetType);
            
            Dictionary<int,EventHandleMgr.HandleEvent> dict = new();
            dict[1] = OnHandleLoginEvent;
            EventHandleMgr.SetHandleMap(dict);
            
            await EventHandleMgr.Handle(1111, ReqLogin.Create());

            await TcpServer.Start(Settings.TcpPort, builder => builder.UseConnectionHandler<AppTcpConnectionHandler>());
            await WebSocketServer.Start(Settings.WebSocketUrl, new AppWebSocketConnectionHandler());
            await HttpServer.Start(Settings.HttpPort);

            Log.Info("加载配置表...");
            (bool success, string msg) = ConfigManager.LoadTables();
            if (!success)
                throw new Exception($"载入配置表失败... {msg}");

            GlobalTimer.Start();
            await CompRegister.ActiveGlobalComps();
            return true;
        }

        public async Task Stop()
        {
            try
            {
                // 断开所有连接
                await SessionManager.RemoveAll();
                // 取消所有未执行定时器
                await QuartzTimer.Stop();
                // 保证actor之前的任务都执行完毕
                await ActorMgr.AllFinish();
                // 关闭网络服务
                await HttpServer.Stop();
                await TcpServer.Stop();
                await WebSocketServer.Stop();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            finally
            {
                // 存储所有数据
                await GlobalTimer.Stop();
                await ActorMgr.RemoveAll();
            }
        }
    }
}
