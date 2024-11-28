using Geek.Server.Core.Storage;

namespace Geek.Server.Core.Net.BaseHandler
{
    [AttributeUsage(AttributeTargets.Class)]
    public class StateCacheAttribute<T> : Attribute where T : IDynamicState, new()
    {
    }
}
