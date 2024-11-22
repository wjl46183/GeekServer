using Geek.Server.Core.Actors;
using Geek.Server.TestPressure.Logic;
using Geek.Server.Core.PolymorphicType;
using System.Net.WebSockets;
using System.Text;
using MemoryPack;

namespace Geek.Server.TestPressure
{
    [MemoryPackable]
    [MemoryPackUnion(0, typeof(FooClass))]
    [MemoryPackUnion(1, typeof(BarClass))]
    public partial interface IUnionSample
    {
    }

    [MemoryPackable]
    public partial class FooClass : IUnionSample
    {
        public int XYZ { get; set; }
    }

    [MemoryPackable]
    public partial class BarClass : IUnionSample
    {
        public string? OPQ { get; set; }
    }
    
    class Program
    {
        


        public static async Task Main(string[] args)
        {
// ---

            IUnionSample data = new BarClass() {OPQ = "aaa"};

// Serialize as interface type.
            var bin = MemoryPackSerializer.Serialize(data);

// Deserialize as interface type.
            var reData = MemoryPackSerializer.Deserialize<IUnionSample>( bin);

            switch (reData)
            {
                // case FooClass x:
                //     Console.WriteLine(x.XYZ);
                //     break;
                // case BarClass x:
                //     Console.WriteLine(x.OPQ);
                //     break;
                default:
                    break;
            }
        }
    }
}