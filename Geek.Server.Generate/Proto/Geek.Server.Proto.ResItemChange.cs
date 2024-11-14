//auto generated, do not modify it

using MemoryPack;
using System.Collections.Generic;
using Geek.Server.Core.Net;
namespace Geek.Server.Proto
{
	[MemoryPackable]
	public partial class ResItemChange : Message
	{
		[MemoryPackIgnore]
		public const int Sid = 901279609;

		[MemoryPackIgnore]
		public const int MsgID = Sid;
		public override int MsgId => MsgID;

        /// <summary>
        /// 变化的道具
        /// </summary>
        public Dictionary<int, long> ItemDic { get; set; }
	}
}
