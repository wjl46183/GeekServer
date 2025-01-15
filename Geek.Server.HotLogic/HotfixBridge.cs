using Geek.Server.Core.Actors;
using Geek.Server.Core.Comps;
using Geek.Server.Core.Events;
using Geek.Server.Core.Hotfix;
using Geek.Server.Core.Net.Http;
using Geek.Server.Core.Net.Session;
using Geek.Server.Core.Net.Tcp;
using Geek.Server.Core.Storage;
using Geek.Server.Core.Timer;
using Geek.Server.Core.Utils;
using Geek.Server.HotLogic.EventHandle;
using Geek.Server.Storage;

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
            Log.Info($"设置消息类型映射、消息事件映射...");
            UpdateMappings();
            if (reload)
            {
                ActorMgr.ClearAgent();
                return true;
            }

            Log.Info($"从Storage库存储数据结构到MongoDB...");
            BsonClassMapHelper.RegisterAllClass(typeof(AccountState).Assembly);

            Log.Info($"从Storage库注册组件...");
            await CompRegister.Init(typeof(LoginState).Assembly);

            Log.Info("加载配置表...");
            (bool success, string msg) = ConfigManager.LoadTables();
            if (!success)
                throw new Exception($"载入配置表失败... {msg}");

            Log.Info($"初始化全局定时...");
            GlobalTimer.Start();
            await CompRegister.ActiveGlobalComps();
            return true;
        }

        /// <summary>
        /// 更新消息映射
        /// </summary>
        public static void UpdateMappings()
        {
            // 绑定消息类型
            HotfixMgr.SetMsgGetter((i =>
            {
                if (MemoryPackTypeMapping.GetIdTypeDict().TryGetValue(i, out var type))
                {
                    return type;
                }

                if (Core.MemoryPackTypeMapping.GetIdTypeDict().TryGetValue(i, out type))
                {
                    return type;
                }

                Log.Error($"消息类型未找到：{i}");
                return null;
            }));
            HotfixMgr.SetMsgCreater((type) => MemoryPackTypeMapping.Create(type));
            //绑定事件处理器
            EventHandleMgr.SetHandleMap(EventMapings.typeIdHandleFuncs);
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