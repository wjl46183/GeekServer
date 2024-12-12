using Geek.Server.Core.Actors;
using Geek.Server.Core.Comps;
using Geek.Server.Core.Storage;
using MemoryPack;

namespace Geek.Server.Storage;

[MemoryPackable]
[SaveState(ActorType.Role)]
public partial class RoleState : BaseState
{
    public long RoleId => Id;
    public string RoleName = string.Empty;
    public int Level = 1;
    public int VipLevel = 1;
    public DateTime CreateTime;
    public DateTime LoginTime;
    public DateTime OfflineTime;
}