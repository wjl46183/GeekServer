using MemoryPack;

namespace Geek.Server.Core.PolymorphicType
{
    /// <summary>
    /// 类型ID接口
    /// </summary>
    [MemoryPackable(GenerateType.NoGenerate)]
    public partial interface ITypeId
    {
        /// <summary>
        /// 当前类型的ID
        /// </summary>
        public int TypeId { get; }
    }
}
