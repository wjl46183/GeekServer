using Geek.Server.Core.Actors;
using Geek.Server.Core.Comps;
using Geek.Server.Core.Events;
using Geek.Server.Core.Hotfix;
using Geek.Server.Core.Net.Http;
using Geek.Server.Core.Net.Session;
using Geek.Server.Core.Net.Tcp;
using Geek.Server.Core.Net.Websocket;
using Geek.Server.Core.Timer;
using Geek.Server.Core.Utils;
using Geek.Server.HotData;
using Geek.Server.HotLogic.EventHandle;
using Microsoft.AspNetCore.Connections;

namespace Geek.Server.HotLogic.Common
{
    /// <summary>
    /// 热更新桥接器，启动时会反射调用，用于初始化热更新相关的内容
    /// </summary>
    internal class HotfixBridge : IHotfixBridge
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public ServerType BridgeType => ServerType.Game;

        public async Task<bool> OnLoadSuccess(bool reload)
        {
            if (reload)
            {
                ActorMgr.ClearAgent();
                return true;
            }

            // 绑定消息类型
            HotfixMgr.SetMsgGetter((i =>
            {
                var have = MemoryPackTypeMapping.GetIdTypeDict().ContainsKey(i);
                if (!have)
                {
                    if (Core.MemoryPackTypeMapping.GetIdTypeDict().ContainsKey(i))
                    {
                        return Core.MemoryPackTypeMapping.GetIdTypeDict()[i];
                    }
                    else
                    {
                        Log.Error($"消息类型未找到：{i}");
                        return null;
                    }
                }

                return MemoryPackTypeMapping.GetIdTypeDict()[i];
            }));
            HotfixMgr.SetMsgCreater((type) => MemoryPackTypeMapping.Create(type));
            //绑定事件处理器
            EventHandleMgr.SetHandleMap(EventMapings.typeIdHandleFuncs);

            await TcpServer.Start(Settings.TcpPort, builder => builder.UseConnectionHandler<TcpConnectionHandler>());
            await WebSocketServer.Start(Settings.WebSocketUrl, new WebSocketConnectionHandler());
            await HttpServer.Start(Settings.HttpPort);

            Log.Info("加载配置表...");
            (bool success, string msg) = ConfigManager.LoadTables();
            if (!success)
                throw new Exception($"载入配置表失败... {msg}");

            Log.Info($"初始化全局定时...");
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