//auto generated, do not modify it

using MemoryPack;
using Geek.Server.Core.Net;
namespace Geek.Server.Proto
{
	[MemoryPackable]
	public partial class ResErrorCode : Message
	{
		[MemoryPackIgnore]
		public const int Sid = 1179199001;

		[MemoryPackIgnore]
		public const int MsgID = Sid;
		public override int MsgId => MsgID;

        /// <summary>
        /// 0:表示无错误
        /// </summary>
        public long ErrCode { get; set; }
        /// <summary>
        /// 错误描述（不为0时有效）
        /// </summary>
        public string Desc { get; set; }
	}
}
