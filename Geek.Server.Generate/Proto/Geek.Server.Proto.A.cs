//auto generated, do not modify it

using MemoryPack;

namespace Geek.Server.Proto
{
	[MemoryPackable]
	public partial class A 
	{
		[MemoryPackIgnore]
		public const int Sid = 1250601847;


        public int Age { get; set; }
        public TestEnum E { get; set; } = TestEnum.B;
        public TestStruct TS { get; set; }
	}
}
