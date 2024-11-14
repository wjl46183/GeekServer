//auto generated, do not modify it

using MemoryPack;
using System.Collections.Generic;
using Geek.Server.Core.Net;

namespace Geek.Server.Proto
{
	[MemoryPackable]
	public partial class ReqBagInfo : Message
	{
		[MemoryPackIgnore]
		public const int Sid = 1435193915;

		[MemoryPackIgnore]
		public const int MsgID = Sid;
		public override int MsgId => MsgID;

	}
}
