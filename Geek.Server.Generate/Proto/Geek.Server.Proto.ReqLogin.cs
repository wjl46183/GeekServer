//auto generated, do not modify it

using MemoryPack;
using Geek.Server.Core.Net;
namespace Geek.Server.Proto
{
	[MemoryPackable]
	public partial class ReqLogin : Message
	{
		[MemoryPackIgnore]
		public const int Sid = 1267074761;

		[MemoryPackIgnore]
		public const int MsgID = Sid;
		public override int MsgId => MsgID;

        public string UserName { get; set; }
        public string Platform { get; set; }
        public int SdkType { get; set; }
        public string SdkToken { get; set; }
        public string Device { get; set; }
	}
}
