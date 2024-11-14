//auto generated, do not modify it

using MemoryPack;
using System.Collections.Generic;
using Geek.Server.Core.Net;
namespace Geek.Server.Proto
{
	[MemoryPackable]
	public partial class ReqUseItem : Message
	{
		[MemoryPackIgnore]
		public const int Sid = 1686846581;

		[MemoryPackIgnore]
		public const int MsgID = Sid;
		public override int MsgId => MsgID;

        /// <summary>
        /// 道具id
        /// </summary>
        public int ItemId { get; set; }
	}
}
