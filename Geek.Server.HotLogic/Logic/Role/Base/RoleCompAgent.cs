using Geek.Server.Main.Common;
using Geek.Server.Core.Net.Session;
using Geek.Server.Core.Actors;
using Geek.Server.Core.Events;
using Geek.Server.Core.Hotfix.Agent;
using Geek.Server.Core.Net;
using Geek.Server.Core.Timer;
using Geek.Server.HotData.Proto;
using Geek.Server.HotLogic.Logic.Role.Bag;
using Geek.Server.HotLogic.Logic.Server;
using Geek.Server.Storage.Comp;

namespace Geek.Server.HotLogic.Logic.Role.Base
{
    public static class RoleCompAgentExt
    {
        private static readonly Logger LOGGER = LogManager.GetCurrentClassLogger();

        public static async Task NotifyClient(this ICompAgent agent, BaseEvent msg, long serialId = 0,
            StateCode code = StateCode.Success)
        {
            var roleComp = await agent.GetCompAgent<RoleCompAgent>();
            if (roleComp != null)
                roleComp.NotifyClient(msg, serialId, code);
            else
                LOGGER.Warn($"{agent.OwnerType}未注册RoleComp组件");
        }
    }

    public class RoleCompAgent : BaseCompAgent<RoleStateComp>, ICrossDay
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        [Discard]
        public virtual async Task OnLogin(EventLogin eventLogin, bool isNewRole)
        {
            SetAutoRecycle(false);
            if (isNewRole)
            {
                Comp.State.CreateTime = DateTime.Now;
                Comp.State.Level = 1;
                Comp.State.VipLevel = 1;
                Comp.State.RoleName = new System.Random().Next(1000, 10000).ToString(); //随机给一个
                //激活背包组件
                await GetCompAgent<BagCompAgent>();
            }

            Comp.State.LoginTime = DateTime.Now;
            var resLogin = BuildLoginMsg();
            NotifyClient(resLogin);
        }

        public async Task OnLogout()
        {
            //移除在线玩家
            var serverComp = await ActorMgr.GetCompAgent<ServerCompAgent>();
            await serverComp.RemoveOnlineRole(ActorId);
            //下线后会被自动回收
            SetAutoRecycle(true);
            QuartzTimer.Unschedule(ScheduleIdSet);
        }

        private ResLogin BuildLoginMsg()
        {
            var res = ResLogin.Create();
            res.Code = 0;
            res.UserInfo = UserInfo.Create();
            {
                res.UserInfo.CreateTime = Comp.State.CreateTime.Ticks;
                res.UserInfo.Level = Comp.State.Level;
                res.UserInfo.RoleId = Comp.State.RoleId;
                res.UserInfo.RoleName = Comp.State.RoleName;
                res.UserInfo.VipLevel = Comp.State.VipLevel;
            }
            return res;
        }

        Task ICrossDay.OnCrossDay(int openServerDay)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 通知绑定的客户端，无消息体
        /// </summary>
        /// <param name="serialId"></param>
        /// <param name="code"></param>
        public void NotifyClient(long serialId = 0, StateCode code = StateCode.Success)
        {
            var session = SessionManager.Get(ActorId);
            session?.Channel?.Write(serialId, (int)code, "");
        }

        /// <summary>
        /// 通知绑定的客户端
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="serialId"></param>
        public void NotifyClient(BaseEvent msg, long serialId = 0)
        {
            var session = SessionManager.Get(ActorId);
            session?.Channel?.Write(msg, serialId);
        }
    }
}