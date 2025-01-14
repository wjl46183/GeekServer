using System.Collections.Concurrent;
using System.Reflection;
using Geek.Server.Core.Actors;
using Geek.Server.Core.Comps;
using Geek.Server.Core.Events;
using Geek.Server.Core.Hotfix.Agent;
using Geek.Server.Core.Net;
using Geek.Server.Core.Net.BaseHandler;
using Geek.Server.Core.Net.Http; 
using Geek.Server.Core.Net.Tcp.Handler;

namespace Geek.Server.Core.Hotfix
{
    public class HotfixMgr
    {
        internal static volatile bool DoingHotfix = false;

        private static volatile HotfixModule module = null; 

        public static Assembly HotfixAssembly => module?.HotfixAssembly;

        private static readonly ConcurrentDictionary<int, HotfixModule> oldModuleMap = new();

        public static DateTime ReloadTime { get; private set; }
        
        static Func<int, Type> msgGetter;
        
        static Func<Type, BaseEvent> msgCreater;

        public static async Task<bool> LoadHotfixModule(string dllVersion = "")
        {
            var dllPath = Path.Combine(Environment.CurrentDirectory, string.IsNullOrEmpty(dllVersion) ? "hotfix/" : $"{dllVersion}/");
            var newModule = new HotfixModule(dllPath);
            bool reload = module != null;
            // 起服时失败会有异常抛出
            var success = newModule.Init(reload);
            if (!success)
                return false;
            return await Load(newModule, reload);
        }

        public static Task<bool> LoadSelfModule()
        {
            return Load(new HotfixModule(), false);
        }

        private static async Task<bool> Load(HotfixModule newModule, bool reload)
        {
            ReloadTime = DateTime.Now;
            if (reload)
            {
                var oldModule = module;
                DoingHotfix = true;
                int oldModuleHash = oldModule.GetHashCode();
                oldModuleMap.TryAdd(oldModuleHash, oldModule);
                _ = Task.Run(async () =>
                {
                    await Task.Delay(1000 * 60 * 3);
                    oldModuleMap.TryRemove(oldModuleHash, out _);
                    oldModule.Unload();
                    DoingHotfix = !oldModuleMap.IsEmpty;
                });
            }

            module = newModule;
            if (module.HotfixBridge != null)
                return await module.HotfixBridge.OnLoadSuccess(reload);
            return true;
        }

        public static Task Stop()
        {
            return module?.HotfixBridge?.Stop() ?? Task.CompletedTask;
        }

        internal static Type GetAgentType(Type compType)
        {
            return module.GetAgentType(compType);
        }

        internal static Type GetCompType(Type agentType)
        {
            return module.GetCompType(agentType);
        }

        public static T GetAgent<T>(BaseComp comp, Type refAssemblyType) where T : ICompAgent
        {
            if (!oldModuleMap.IsEmpty)
            {
                var asb = typeof(T).Assembly;
                var asb2 = refAssemblyType?.Assembly;
                foreach (var kv in oldModuleMap)
                {
                    var old = kv.Value;
                    if (asb == old.HotfixAssembly || asb2 == old.HotfixAssembly)
                        return old.GetAgent<T>(comp);
                }
            }
            return module.GetAgent<T>(comp);
        }

        public static BaseHttpHandler GetHttpHandler(string cmd)
        {
            return module.GetHttpHandler(cmd);
        }

        public static void SetMsgGetter(Func<int, Type> msgGetter)
        {
            HotfixMgr.msgGetter = msgGetter;
        }
        
        public static void SetMsgCreater(Func<Type, BaseEvent> msgGetter)
        {
            msgCreater = msgGetter;
        }

        public static Type GetMsgType(int msgId)
        {
            if(MemoryPackTypeMapping.GetIdTypeDict().TryGetValue(msgId, out var coreType))
            {
                return coreType;
            }
            return msgGetter(msgId);
        }
        
        public static BaseEvent CreateMsgType(Type type)
        {
            return msgCreater(type);
        }

        /// <summary>
        /// 获取实例
        /// 主要用于获取Event,Timer, Schedule,的Handler实例
        /// </summary>
        public static T GetInstance<T>(string typeName, Type refAssemblyType = null)
        {
            if (string.IsNullOrEmpty(typeName))
                return default;
            if (oldModuleMap.Count > 0)
            {
                var asb = refAssemblyType?.Assembly;
                foreach (var kv in oldModuleMap)
                {
                    var old = kv.Value;
                    if (asb == old.HotfixAssembly)
                        return old.GetInstance<T>(typeName);
                }
            }
            return module.GetInstance<T>(typeName);
        }
    }
}
