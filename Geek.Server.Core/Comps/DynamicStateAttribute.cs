using Geek.Server.Core.Actors;
using Geek.Server.Core.Storage;

namespace Geek.Server.Core.Comps
{
    /// <summary>
    /// 状态缓存组件,标记状态属于那个Actor类型,动态类型需要绑定到一个存档数据上
    /// 动态数据具有一下特性：
    /// 1. 生命周期绑定Actor上
    /// 2. 不存档
    /// 3. 一个Actor对象可以有多个Cache数据结构，只能有一个主数据（存档数据）结构
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DynamicStateAttribute<TState> : Attribute where TState : BaseState
    {
    }
}