using Geek.Server.Core.Actors;
using Geek.Server.Core.Comps;
using Geek.Server.Core.Storage;
using MemoryPack;

namespace Geek.Server.Main.Logic.Server
{
    [MemoryPackable]
    public partial class ServerState : CacheState
    {
        /// <summary>
        /// 世界等级
        /// </summary>
        public int WorldLevel { get; set; } = 1;
    }


    [Comp(ActorType.Server)]
    public class ServerComp : StateComp<ServerState>
    {
        /// <summary>
        /// 存放在此处的数据不会回存到数据库
        /// </summary>
        public HashSet<long> OnlineSet = new();
    }

}
