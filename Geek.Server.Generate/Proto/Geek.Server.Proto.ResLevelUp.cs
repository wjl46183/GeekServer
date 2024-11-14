//auto generated, do not modify it

using MemoryPack;
using Geek.Server.Core.Net;
namespace Geek.Server.Proto
{
	[MemoryPackable]
	public partial class ResLevelUp : Message
	{
		[MemoryPackIgnore]
		public const int Sid = 1587576546;

		[MemoryPackIgnore]
		public const int MsgID = Sid;
		public override int MsgId => MsgID;

        /// <summary>
        /// 玩家等级
        /// </summary>
        public int Level { get; set; }
	}
}
