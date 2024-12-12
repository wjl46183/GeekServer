using Geek.Server.Core.Net;
using MemoryPack;

namespace Geek.Server.HotData.Proto;
[MemoryPackable]
public partial class EventNetConnect : BaseEvent
{ 
}

[MemoryPackable]
public partial class EventNetDisConnect : BaseEvent
{
} 