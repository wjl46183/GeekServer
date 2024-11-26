using System.Collections.Concurrent;
using Geek.Server.Core.Storage;
using MemoryPack;

namespace Server.Storage.Login;

[MemoryPackable]
public partial class PlayerInfo
{
    //player相对特殊，id不是long，所以不继承DBState，自定义mongoDB的id
    public string playerId;
    public int SdkType;
    public string UserName;

    //这里设定每个账号在1服只有能创建1个角色 
    public Dictionary<int, long> RoleMap = new(); 
}

[MemoryPackable]
public partial class LoginState : CacheState
{
    public ConcurrentDictionary<string, PlayerInfo> PlayerMap = new();
}