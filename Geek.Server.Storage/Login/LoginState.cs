using System.Collections.Concurrent;
using Geek.Server.Core.Actors;
using Geek.Server.Core.Comps;
using Geek.Server.Core.Storage;
using MemoryPack;

namespace Geek.Server.Storage;

[MemoryPackable]
public partial class PlayerInfo
{
    //player相对特殊，id不是long，所以不继承DBState，自定义mongoDB的id
    public string playerId = String.Empty;
    public int sdkType = 0;
    public string userName = String.Empty;

    //这里设定每个账号在1服只有能创建1个角色 
    public Dictionary<int, long> RoleMap = new(); 
}

[MemoryPackable]
[SaveState(ActorType.PhysicServer)]
public partial class LoginState : BaseState
{
    public ConcurrentDictionary<string, PlayerInfo> PlayerMap = new();
}