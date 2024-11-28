
using Geek.Server.Main.Common;
using Geek.Server.Main.Common.Event;
using Geek.Server.Main.Common.Session;
using Geek.Server.Core.Actors;
using Geek.Server.Core.Events;
using Geek.Server.Core.Hotfix.Agent;
using Geek.Server.Core.Net;
using Geek.Server.Core.Timer;
using Server.Logic.Common.Handler;
using Server.Logic.Logic.Role.Bag;
using Server.Logic.Logic.Server;
using Geek.Server.Storage.Role.Base;
using Geek.Server.Storage.Role.Base.Comp;

namespace Server.Logic.Logic.Role.Base
{

    public static class RoleCompAgentExt
    {
        private static readonly Logger LOGGER = LogManager.GetCurrentClassLogger();
        public static async Task NotifyClient(this ICompAgent agent, Message msg, int uniId = 0, StateCode code = StateCode.Success)
        {
            var roleComp = await agent.GetCompAgent<RoleCompAgent>();
            if (roleComp != null)
                roleComp.NotifyClient(msg, uniId, code);
            else
                LOGGER.Warn($"{agent.OwnerType}未注册RoleComp组件");
        }
    }

    public class RoleCompAgent : StateCompAgent<RoleStateComp, RoleState>, ICrossDay
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();


        [Event(EventID.SessionRemove)]
        private class EL : EventListener<RoleCompAgent>
        {
            protected override Task HandleEvent(RoleCompAgent agent, Event evt)
            {
                return agent.OnLogout();
            }
        }

        public async Task<ResLogin> OnLogin(ReqLogin reqLogin, bool isNewRole)
        {
            SetAutoRecycle(false);
            if (isNewRole)
            {
                State.CreateTime = DateTime.Now;
                State.Level = 1;
                State.VipLevel = 1;
                State.RoleName = new System.Random().Next(1000, 10000).ToString();//随机给一个
                //激活背包组件
                await GetCompAgent<BagCompAgent>();
            }
            State.LoginTime = DateTime.Now;
            return BuildLoginMsg();
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
                res.UserInfo.CreateTime = State.CreateTime.Ticks;
                res.UserInfo.Level = State.Level;
                res.UserInfo.RoleId = State.RoleId;
                res.UserInfo.RoleName = State.RoleName;
                res.UserInfo.VipLevel = State.VipLevel;
            }
            return res;
        }

        Task ICrossDay.OnCrossDay(int openServerDay)
        {
            return Task.CompletedTask;
        }

        public void NotifyClient(Message msg, int uniId = 0, StateCode code = StateCode.Success)
        {
            var session = SessionManager.Get(ActorId);
            session?.Channel?.Write(msg, uniId, code);
        }
    }
}
