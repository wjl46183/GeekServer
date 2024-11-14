//auto generated, do not modify it

using MemoryPack;
using Geek.Server.Core.Net;
namespace ClientProto
{
	[MemoryPackable]
	public partial class NetDisConnectMessage : Message
	{
		[MemoryPackIgnore]
		public const int Sid = 1245418514;

		[MemoryPackIgnore]
		public const int MsgID = Sid;
		public override int MsgId => MsgID;

	}
}
