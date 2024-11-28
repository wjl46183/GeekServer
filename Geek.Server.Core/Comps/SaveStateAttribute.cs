using Geek.Server.Core.Actors;

namespace Geek.Server.Core.Comps
{
    /// <summary>
    /// 存档状态组件,标记状态属于那个Actor类型，内存中存在的临时数据可配合动态数据使用
    /// 数据特性：
    /// 1. 写入数据库
    /// 2. 生命周期与Actor对象同步
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SaveStateAttribute : Attribute
    {
        public SaveStateAttribute(ActorType type)
        {
            ActorType = type;
        }

        public ActorType ActorType { get; }

    }
}