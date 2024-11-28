using Geek.Server.Main.Common;
using Geek.Server.Main.Common.Session;
using Geek.Server.Core.Actors;
using Geek.Server.Core.Hotfix.Agent;
using Geek.Server.Core.Net;
using Geek.Server.Core.Utils;
using Geek.Server.Storage.Login;
using Geek.Server.Storage.Login.Comp;
using Server.Logic.Common.Handler;
using Server.Logic.Logic.Role.Base;
using Server.Logic.Logic.Server;

namespace Server.Logic.Logic.Login
{
    public class LoginCompAgent : StateCompAgent<LoginStateComp, LoginState>
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public async Task OnLogin(NetChannel channel, ReqLogin reqLogin)
        {
            if (string.IsNullOrEmpty(reqLogin.UserName))
            {
                channel.Write(null, reqLogin.SerialId, StateCode.AccountCannotBeNull); 
                return;
            }

            if (reqLogin.Platform != "android" && reqLogin.Platform != "ios" && reqLogin.Platform != "unity")
            {
                //验证平台合法性
                channel.Write(null, reqLogin.SerialId, StateCode.UnknownPlatform);
                return;
            }

            //查询角色账号，这里设定每个服务器只能有一个角色
            var roleId = GetRoleIdOfPlayer(reqLogin.UserName, reqLogin.SdkType);
            var isNewRole = roleId <= 0;
            if (isNewRole)
            {
                //没有老角色，创建新号
                roleId = IdGenerator.GetActorID(ActorType.Role);
                CreateRoleToPlayer(reqLogin.UserName, reqLogin.SdkType, roleId);
                // Log.Info("创建新号:" + roleId);
            }

            //添加到session
            var session = new Session
            {
                Id = roleId,
                Time = DateTime.Now,
                Channel = channel,
                Sign = reqLogin.Device
            };
            SessionManager.Add(session);

            //登陆流程
            var roleComp = await ActorMgr.GetCompAgent<RoleCompAgent>(roleId);
            //从登录线程-->调用Role线程 所以需要入队
            var resLogin = await roleComp.OnLogin(reqLogin, isNewRole);
            channel.Write(resLogin, reqLogin.SerialId, StateCode.Success);

            //加入在线玩家
            var serverComp = await ActorMgr.GetCompAgent<ServerCompAgent>();
            await serverComp.AddOnlineRole(ActorId);
        }

        private long GetRoleIdOfPlayer(string userName, int sdkType)
        {
            var playerId = $"{sdkType}_{userName}";
            if (State.PlayerMap.TryGetValue(playerId, out var state))
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
            State.PlayerMap.TryGetValue(playerId, out var info);
            if (info == null)
            {
                info = PlayerInfo.Create();
                info.playerId = playerId;
                info.sdkType = sdkType;
                info.userName = userName;
                State.PlayerMap[playerId] = info;
            } 
            info.RoleMap[Settings.ServerId] = roleId;
        }

    }
}