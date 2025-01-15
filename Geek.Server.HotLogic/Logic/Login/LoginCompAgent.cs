using Geek.Server.LogicLaunch.Common;
using Geek.Server.Core.Net.Session;
using Geek.Server.Core.Actors;
using Geek.Server.Core.Hotfix.Agent;
using Geek.Server.Core.Net.BaseHandler;
using Geek.Server.Core.Utils;
using Geek.Server.HotData.Proto;
using Geek.Server.Storage;
using Geek.Server.HotLogic.Logic.Role.Base;
using Geek.Server.HotLogic.Logic.Server;
using Geek.Server.Storage.Comp;

namespace Geek.Server.HotLogic.Logic.Login
{
    public class LoginCompAgent : BaseCompAgent<LoginStateComp>
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// TCP链接成功后，登陆
        /// </summary>
        /// <param name="eventLogin"></param>
        [BindEvent]
        public virtual async Task OnLogin(EventLogin eventLogin)
        {
            if (string.IsNullOrEmpty(eventLogin.OpenId))
            {
                eventLogin.ErrCode = (int)StateCode.AccountCannotBeNull;
                return;
            }

            //查询角色账号，这里设定每个服务器只能有一个角色
            var roleId = GetRoleIdOfPlayer(eventLogin.OpenId);
            var isNewRole = roleId <= 0;
            if (isNewRole)
            {
                //没有老角色，创建新号
                roleId = IdGenerator.GetActorID(ActorType.Role);
                CreateRoleToPlayer(eventLogin.OpenId, eventLogin.SdkType, roleId);
                // Log.Info("创建新号:" + roleId);
            }

            //登陆流程
            var roleComp = await ActorMgr.GetCompAgent<RoleCompAgent>(roleId);
            //从登录线程-->调用Role线程 所以需要入队
            await roleComp.OnLogin(eventLogin, isNewRole);

            //加入在线玩家
            var serverComp = await ActorMgr.GetCompAgent<ServerCompAgent>();
            await serverComp.AddOnlineRole(ActorId);
            //激活 session
            SessionManager.Active(eventLogin.cacheSessionSerialId,roleId,eventLogin.Sign);
        }

        private long GetRoleIdOfPlayer(string openId)
        {
            var playerId = openId;
            if (Comp.State.PlayerMap.TryGetValue(playerId, out var state))
            {
                if (state.RoleMap.TryGetValue(Settings.ServerId, out var roleId))
                    return roleId;
                return 0;
            }
            return 0;
        }

        private void CreateRoleToPlayer(string userName, int sdkType, long roleId)
        {
            var playerId = $"{sdkType}_{userName}";
            Comp.State.PlayerMap.TryGetValue(playerId, out var info);
            if (info == null)
            {
                info = PlayerInfo.Create();
                info.playerId = playerId;
                info.sdkType = sdkType;
                info.userName = userName;
                Comp.State.PlayerMap[playerId] = info;
            } 
            info.RoleMap[Settings.ServerId] = roleId;
        }

    }
}