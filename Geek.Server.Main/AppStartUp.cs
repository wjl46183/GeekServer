using Geek.Server.Core.Actors.Impl;
using Geek.Server.Core.Comps;
using Geek.Server.Core.Hotfix;
using Geek.Server.Core.Storage;
using Geek.Server.Core.Utils;
using Geek.Server.HotData.Proto;
using Geek.Server.Storage;
using NLog;
using NLog.Config;

namespace Geek.Server.Main.Common
{
    internal class AppStartUp
    {
        static readonly Logger Log = LogManager.GetCurrentClassLogger();
        public static async Task Enter()
        {
            try
            {
                var flag = Start();
                if (!flag) return; //启动服务器失败

                //Actor模型设置递归检测规则
                ActorLimit.Init(ActorLimit.RuleType.None);
                Log.Info($"Actor循环消息规则：{ActorLimit.RuleType.None}");
                
                //数据库初始化
                Log.Info($"数据库初始化...");
                GameDB.Init(new MongoDBConnection());
                GameDB.Open(Settings.MongoUrl, Settings.MongoDBName);
                
                Log.Info($"从Storage库注册组件...");
                await CompRegister.Init(typeof(LoginState).Assembly);
                Log.Info($"加载支持热更新的逻辑模块...");
                await HotfixMgr.LoadHotfixModule();

                Log.Info("进入游戏主循环...");
                Console.WriteLine("进入游戏主循环!!!");
                Settings.LauchTime = DateTime.Now;
                Settings.AppRunning = true;

                await Settings.AppExitToken;
            }
            catch (Exception e)
            {
                Console.WriteLine($"服务器执行异常，e:{e}");
                Log.Fatal(e);
            }

            Console.WriteLine($"退出服务器开始");
            await HotfixMgr.Stop();
            Console.WriteLine($"退出服务器成功");
        }

        /// <summary>
        /// 基本初始化
        /// </summary>
        /// <returns></returns>
        private static bool Start()
        {
            try
            {
                string configPath = "Configs/app_config.json";
                Console.WriteLine($"初始化配置:{configPath}");
                Settings.Load<AppSetting>(configPath, ServerType.Game);
                LogManager.Setup().SetupExtensions(s => s.RegisterConditionMethod("logState", (e) => Settings.IsDebug ? "debug" : "release"));
                LogManager.Configuration = new XmlLoggingConfiguration("Configs/app_log.config");
                LogManager.AutoShutdown = false;

                Console.WriteLine($"注册MongoDB类型解析...");
                BsonClassMapHelper.SetConvention();
                BsonClassMapHelper.RegisterAllClass(typeof(EventLogin).Assembly);
                BsonClassMapHelper.RegisterAllClass(typeof(Program).Assembly);

                return true;
            }
            catch (Exception e)
            {
                Log.Error($"启动服务器失败,异常:{e}");
                return false;
            }
        }
    }
}
