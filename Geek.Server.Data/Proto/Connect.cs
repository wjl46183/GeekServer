using Geek.Server.Core.Net;
using MemoryPack; 

namespace ClientProto
{ 

    [MemoryPackable]
    public partial class NetConnectMessage : Message
    { 
    }

    [MemoryPackable]
    public partial class NetDisConnectMessage : Message
    {
    } 
}
