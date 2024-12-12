
using Geek.Server.Core.Net;
using MemoryPack;

namespace Geek.Server.Core.Net.Session
{
    [MemoryPackable]
    public partial class Session
    {
        /// <summary>
        /// 唯一ID，默认使用int类型，最大21亿，够用了
        /// </summary>
        public static int CUR_SERIAL_ID = 0;
        
        /// <summary>
        /// 全局标识符
        /// </summary>
        public long Id { set; get; }
        
        /// <summary>
        /// 消息唯一id
        /// </summary>
        public int SerialId { get; set; }

        /// <summary>
        /// 连接时间
        /// </summary>
        public DateTime Time { set; get; }

        /// <summary>
        /// 连接上下文
        /// </summary>
        [MemoryPackIgnore]
        public NetChannel Channel { get; set; }

        /// <summary>
        /// 连接标示，避免自己顶自己的号,客户端每次启动游戏生成一次/或者每个设备一个
        /// </summary>
        public string Sign { get; set; }

        public override void OnUse()
        {
            base.OnUse();
            SerialId = Interlocked.Increment(ref CUR_SERIAL_ID);
            Id = default;
            Time = default;
            Channel = null;
            Sign = default;
        }

        public void WriteAsync(BaseEvent msg)
        {
            Channel?.Write(msg);
        }
    }
}