using MemoryPack;
using Geek.Server.Core.PolymorphicType;
using Geek.Server.Core.Serialize;
using Geek.Server.Core.Utils;

namespace Geek.Server.Core.Net
{
    
    /// <summary>
    /// 消息基类型
    /// </summary>
    [MemoryPackable]
    public partial class Message : ITypeId,ISafeObjectPool
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

        /// <summary>
        /// 从池中获取出来的时候调用
        /// </summary>
        public virtual void OnUse()
        {
        }

        /// <summary>
        /// 放入池子中的时候调用
        /// </summary>
        public virtual void OnReturn()
        {
        }
    }

}