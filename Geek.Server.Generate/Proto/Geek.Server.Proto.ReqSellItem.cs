//auto generated, do not modify it

using MemoryPack;
using System.Collections.Generic;
using Geek.Server.Core.Net;
namespace Geek.Server.Proto
{
	[MemoryPackable]
	public partial class ReqSellItem : Message
	{
		[MemoryPackIgnore]
		public const int Sid = -1395845865;

		[MemoryPackIgnore]
		public const int MsgID = Sid;
		public override int MsgId => MsgID;

        /// <summary>
        /// 道具id
        /// </summary>
        public int ItemId { get; set; }
	}
}
