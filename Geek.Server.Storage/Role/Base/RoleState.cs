using Geek.Server.Core.Storage;
using MemoryPack;

namespace Server.Storage.Role.Base;

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