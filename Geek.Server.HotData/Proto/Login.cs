
using Geek.Server.Core.Net;
using MemoryPack;
using Geek.Server.Core.PolymorphicType;

namespace Geek.Server.HotData.Proto
{
    public enum TestEnum
    {
        A, B, C, D, E, F, G, H, I, J, K, L,
    }


    [MemoryPackable]
    public partial struct TestStruct
    {
        public int Age { get; set; }
        public string Name { get; set; }
    }

    [MemoryPackable]
    public partial class A : BaseEvent
    {
        public int Age { get; set; }
        public TestEnum E { get; set; } = TestEnum.B;
        public TestStruct TS { get; set; }
    }

    [MemoryPackable]
    public partial class B : A
    {
        public string Name { get; set; }
        [MemoryPackIgnore]
        public string Test { get; set; }
        public string Test2 { get; set; }
    }


    /// <summary>
    /// 玩家基础信息
    /// </summary>
    [MemoryPackable]
    public partial class UserInfo
    {
        /// <summary>
        /// 角色名
        /// </summary>
        public string RoleName { get; set; }
        /// <summary>
        /// 角色ID
        /// </summary>
        public long RoleId { get; set; }
        /// <summary>
        /// 角色等级
        /// </summary>
        public int Level { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public long CreateTime { get; set; }
        /// <summary>
        /// vip等级
        /// </summary>
        public int VipLevel { get; set; }
    }

    /// <summary>
    /// 请求登录
    /// </summary>
    [MemoryPackable]
    public partial class EventLogin : EventLinkCheck
    {
        /// <summary>
        /// 平台
        /// </summary>
        public string Platform { get; set; }
        
        /// <summary>
        /// sdk类型
        /// </summary>
        public int SdkType { get; set; }
        
        /// <summary>
        /// 登陆指定逻辑服
        /// </summary>
        public string logicServerId { get; set; }
    }


    /// <summary>
    /// 请求登录
    /// </summary>
    [MemoryPackable]
    public partial class ResLogin : BaseEvent
    {
        /// <summary>
        /// 登陆结果，0成功，其他时候为错误码
        /// </summary>
        public int Code { get; set; }
        public UserInfo UserInfo { get; set; }
    }


    /// <summary>
    /// 等级变化
    /// </summary>
    [MemoryPackable]
    public partial class ResLevelUp : BaseEvent
    {
        /// <summary>
        /// 玩家等级
        /// </summary>
        public int Level { get; set; }
    }

    /// <summary>
    /// 双向心跳/收到恢复同样的消息
    /// </summary>
    [MemoryPackable]
    public partial class HearBeat : BaseEvent
    {
        /// <summary>
        /// 当前时间
        /// </summary>
        public long TimeTick { get; set; }
    }

    /// <summary>
    /// 客户端每次请求都会回复错误码
    /// </summary>
    [MemoryPackable]
    public partial class ResErrorCode : BaseEvent
    {
    }

    [MemoryPackable]
    public partial class ResPrompt : BaseEvent
    {
        ///<summary>提示信息类型（1Tip提示，2跑马灯，3插队跑马灯，4弹窗，5弹窗回到登陆，6弹窗退出游戏）</summary>
		public int Type { get; set; }
        ///<summary>提示内容</summary>
        public string Content { get; set; }
    }

}
