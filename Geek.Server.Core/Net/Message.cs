using MemoryPack;
using Geek.Server.Core.PolymorphicType;

namespace Geek.Server.Core.Net
{
    
    /// <summary>
    /// 消息基类型
    /// </summary>
    [MemoryPackable]
    public partial class Message : ITypeId
    {
        /// <summary>
        /// 消息唯一id
        /// </summary>
        public int SerialId { get; set; }
        
        /// <summary>
        /// 消息类型唯一ID
        /// </summary>
        [MemoryPackIgnore]
        public virtual int TypeId { get; }
    
    }

}