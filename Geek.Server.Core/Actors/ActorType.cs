namespace Geek.Server.Core.Actors
{
    /// <summary>
    /// 每个服存在多个实例的（如玩家和公会）需要小于Separator
    /// 最大id应当小于999
    /// Id一旦定义了不应该修改
    /// </summary>
    public enum ActorType
    {
        /// <summary>
        /// 逻辑服
        /// </summary>
        Server,

        /// <summary>
        /// 账号
        /// </summary>
        Account,

        /// <summary>
        /// 公会
        /// </summary>
        Guild,

        /// <summary>
        /// 角色
        /// </summary>
        Role,

        /// <summary>
        ///  =========== 分割线(勿调整,勿用于业务逻辑) =========== 
        /// </summary>
        Separator = 128, 

        /// <summary>
        /// 物理服
        /// </summary>
        PhysicServer = 129, //物理服务器
    }

    /// <summary>
    /// 供ActorLimit检测调用关系
    /// </summary>
    public enum ActorTypeLevel
    {
        Role = 1,
        Guild,
        Server,
    }
}