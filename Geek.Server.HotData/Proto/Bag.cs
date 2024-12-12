using Geek.Server.Core.Net;
using MemoryPack;

namespace Geek.Server.HotData.Proto
{
    /// <summary>
    /// 请求背包数据
    /// </summary>
    [MemoryPackable]
    public partial class ReqBagInfo : BaseEvent
    {
    }

    /// <summary>
    /// 返回背包数据
    /// </summary>
    [MemoryPackable]
    public partial class ResBagInfo : BaseEvent
    {
        public Dictionary<int, long> ItemDic { get; set; } = new Dictionary<int, long>();
    }

    /// <summary>
    /// 请求背包数据
    /// </summary>
    [MemoryPackable]
    public partial class ReqComposePet : BaseEvent
    {
        /// <summary>
        /// 碎片id
        /// </summary>
        public int FragmentId { get; set; }
    }

    /// <summary>
    /// 返回背包数据
    /// </summary>
    [MemoryPackable]
    public partial class ResComposePet : BaseEvent
    {
        /// <summary>
        /// 合成宠物的Id
        /// </summary>
        public int PetId { get; set; }
    }


    /// <summary>
    /// 使用道具
    /// </summary>
    [MemoryPackable]
    public partial class ReqUseItem : BaseEvent
    {
        /// <summary>
        /// 道具id
        /// </summary>
        public int ItemId { get; set; }
    }

    /// <summary>
    /// 出售道具
    /// </summary>
    [MemoryPackable]
    public partial class ReqSellItem : BaseEvent
    {
        /// <summary>
        /// 道具id
        /// </summary>
        public int ItemId { get; set; }

        /// <summary>
        /// 道具id
        /// </summary>
        public int ItemId3 { get; set; }
    }

    [MemoryPackable]
    public partial class ResItemChange : BaseEvent
    {
        /// <summary>
        /// 变化的道具
        /// </summary>
        public Dictionary<int, long> ItemDic { get; set; }
    }
}