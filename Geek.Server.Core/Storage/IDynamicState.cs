using System.Buffers;
using Geek.Server.Core.Net;
using MemoryPack;
using MongoDB.Bson.Serialization.Attributes;
using NLog;

namespace Geek.Server.Core.Storage
{
    /// <summary>
    /// 动态数据基类
    /// </summary>
    public interface IDynamicState
    {
    }

    /// <summary>
    /// 动态数据基类
    /// </summary>
    public class NullDynamicState : IDynamicState
    {
        
    }
}