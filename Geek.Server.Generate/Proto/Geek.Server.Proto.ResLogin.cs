//auto generated, do not modify it

using MemoryPack;
using Geek.Server.Core.Net;
namespace Geek.Server.Proto
{
	[MemoryPackable]
	public partial class ResLogin : Message
	{
		[MemoryPackIgnore]
		public const int Sid = 785960738;

		[MemoryPackIgnore]
		public const int MsgID = Sid;
		public override int MsgId => MsgID;

        /// <summary>
        /// 登陆结果，0成功，其他时候为错误码
        /// </summary>
        public int Code { get; set; }
        public UserInfo UserInfo { get; set; }
	}
}
