namespace Geek.Server.Core.Net.BaseHandler
{
    [AttributeUsage(AttributeTargets.Method)]
    public class BindEventAttribute : Attribute
    {
        /// <summary>
        /// 事件的优先级，同一个主体下安优先级await 执行
        /// </summary>
        public int Priority { get; }

        public BindEventAttribute(int priority = 0)
        {
            Priority = priority;
        }
    }
}
