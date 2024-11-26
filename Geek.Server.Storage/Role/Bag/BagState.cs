using Geek.Server.Core.Storage;
using MemoryPack;

namespace Server.Storage.Role.Bag;

[MemoryPackable]
public partial class BagState : CacheState
{
    public Dictionary<int, long> ItemMap = new Dictionary<int, long>();
}