//auto generated, do not modify it

using MemoryPack;
using System.Collections.Generic;
using Geek.Server.Core.Net;
namespace Geek.Server.Proto
{
	[MemoryPackable]
	public partial class ResComposePet : Message
	{
		[MemoryPackIgnore]
		public const int Sid = 750865816;

		[MemoryPackIgnore]
		public const int MsgID = Sid;
		public override int MsgId => MsgID;

        /// <summary>
        /// 合成宠物的Id
        /// </summary>
        public int PetId { get; set; }
	}
}
