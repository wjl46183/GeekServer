//auto generated, do not modify it

using MemoryPack;
using Geek.Server.Core.Net;
namespace Geek.Server.Proto
{
	[MemoryPackable]
	public partial class HearBeat : Message
	{
		[MemoryPackIgnore]
		public const int Sid = 1575482382;

		[MemoryPackIgnore]
		public const int MsgID = Sid;
		public override int MsgId => MsgID;

        /// <summary>
        /// 当前时间
        /// </summary>
        public long TimeTick { get; set; }
	}
}
