
using Geek.Server.Core.Actors;
using Geek.Server.Core.Comps;
using Geek.Server.Core.Storage;
using MemoryPack;

namespace Geek.Server.App.Logic.Role.Base
{

    [MemoryPackable]
    public partial class RoleState : CacheState
    {
        public long RoleId => Id;
        public string RoleName;
        public int Level = 1;
        public int VipLevel = 1;
        public DateTime CreateTime;
        public DateTime LoginTime;
        public DateTime OfflineTime;
    }
    
    [Comp(ActorType.Role)]
    public partial class RoleComp : StateComp<RoleState>
    {

    }

}
