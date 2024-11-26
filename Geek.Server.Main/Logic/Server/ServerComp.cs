using Geek.Server.Core.Actors;
using Geek.Server.Core.Comps;
using ServerState = Server.Storage.Services.ServerState;

namespace Geek.Server.Main.Logic.Server
{
    [Comp(ActorType.Server)]
    public class ServerComp : StateComp<ServerState>
    {
        /// <summary>
        /// 存放在此处的数据不会回存到数据库
        /// </summary>
        public HashSet<long> OnlineSet = new();
    }
}