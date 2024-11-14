//auto generated, do not modify it

using MemoryPack;
using System.Collections.Generic;
using Geek.Server.Core.Net;
namespace Geek.Server.Proto
{
	[MemoryPackable]
	public partial class ResBagInfo : Message
	{
		[MemoryPackIgnore]
		public const int Sid = -1872884227;

		[MemoryPackIgnore]
		public const int MsgID = Sid;
		public override int MsgId => MsgID;

        public Dictionary<int, long> ItemDic { get; set; } = new Dictionary<int, long>();
	}
}
