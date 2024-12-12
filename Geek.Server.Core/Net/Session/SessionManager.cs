using System.Collections.Concurrent;
using Geek.Server.Core.Actors;
using Geek.Server.Core.Events;
using Geek.Server.Core.Events.Datas;
using NLog;

namespace Geek.Server.Core.Net.Session
{
    /// <summary>
    /// 管理玩家session，一个玩家一个，下线之后移除，顶号之后释放之前的channel，替换channel
    /// </summary>
    public sealed class SessionManager
    {
        internal static readonly ConcurrentDictionary<long, Session> sessionMap = new();
        
        internal static readonly ConcurrentDictionary<long, Session> cacheSessionMap = new();
        
        static readonly NLog.Logger LOGGER = NLog.LogManager.GetCurrentClassLogger();

        public static int Count()
        {
            return sessionMap.Count;
        }

        public static void Remove(long id)
        { 
            if (sessionMap.TryRemove(id, out var _) && ActorMgr.HasActor(id))
            {
                // EventDispatcher.Dispatch(id, (int)EventID.SessionRemove);
            }
        }

        public static Task RemoveAll()
        {
            foreach (var session in sessionMap.Values)
            {
                if (ActorMgr.HasActor(session.Id))
                {
                    // EventDispatcher.Dispatch(session.Id, (int)EventID.SessionRemove);
                }
            }
            sessionMap.Clear();
            return Task.CompletedTask;
        }

        public static Session Get(long id)
        {
            sessionMap.TryGetValue(id, out Session session);
            return session;
        }

        public static void Add(Session session)
        {
            if (sessionMap.TryGetValue(session.Id, out var oldSession) && oldSession.Channel != session.Channel)
            {
                if (oldSession.Sign != session.Sign)
                {
                    var msg = ResErrorCode.Create(5,"你的账号已在其他设备上登陆");
                    oldSession.WriteAsync(msg);
                }
                // 新连接 or 顶号
                oldSession.Channel.Close();
            }
            session.Channel.actorId = session.Id;
            sessionMap[session.Id] = session;
        }
        
        /// <summary>
        /// 添加一个缓存Session，在绑定到Actor之前，临时绑定的都在这里，绑定Actor后移除
        /// </summary>
        /// <param name="session"></param>
        public static void AddCache(Session session)
        {
            if (cacheSessionMap.ContainsKey(session.SerialId))
            {
                LOGGER.Error("cacheSessionId:{} 已存在",session.SerialId);
                return;
            }
            session.Channel.actorId = session.Id;
            cacheSessionMap[session.SerialId] = session;
        }

        /// <summary>
        /// 激活一个缓存Session，激活后从缓存移除，绑定到新的Session
        /// </summary>
        /// <param name="fromId"></param>
        /// <param name="ToId"></param>
        public static bool Active(long sessionSerialId, long newSessionId,string Sign)
        {
            if(cacheSessionMap.Remove(sessionSerialId, out var session))
            {
                session.Channel.actorId = newSessionId;
                session.Sign = Sign;
                session.Time = DateTime.Now;
                Add(session);
                return true;
            }
            LOGGER.Error("cacheSessionId:{} 不存在",sessionSerialId);
            return false;
            
            
            
        }
    }
}
