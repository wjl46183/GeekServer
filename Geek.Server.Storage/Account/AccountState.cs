using Geek.Server.Core.Actors;
using Geek.Server.Core.Comps;
using Geek.Server.Core.Storage;
using MemoryPack;

namespace Geek.Server.Storage;

[MemoryPackable]
[SaveState(ActorType.Account)]
public partial class AccountState : BaseState
{
    /// <summary>
    /// 账户唯一ID
    /// </summary>
    public string openId = String.Empty;
    
    /// <summary>
    /// 服务器ID => 角色ID
    /// </summary>
    public Dictionary<int, long> RoleMap = new();
}