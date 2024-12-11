using System.Collections.Concurrent;
using System.Reflection;
using Geek.Server.Core.Actors;
using Geek.Server.Core.Net;

namespace Geek.Server.Core.Events;

/// <summary>
/// 消息处理器，只处理消息，不处理逻辑
/// </summary>
public static class EventHandleMgr
{
    static readonly NLog.Logger LOGGER = NLog.LogManager.GetCurrentClassLogger();

    /// <summary>
    /// 消息发送给指定的Actor，由Actor处理消息
    /// </summary>
    public delegate ValueTask HandleEvent(long actorId, Message evt);

    /// <summary>
    /// 绑定消息处理类型
    /// </summary>
    private static Dictionary<int, HandleEvent> HandleEventMap = new();

    /// <summary>
    /// 处理消息
    /// </summary>
    /// <param name="actor"></param>
    /// <param name="evt"></param>
    public static async ValueTask Handle(long actorId, Message evt)
    {
        if (actorId == 0)
        {
            LOGGER.Error($"消息处理失败，actorId为0：{evt}");
            return;
        }

        if (!HandleEventMap.TryGetValue(evt.TypeId,out var handleEvent))
        {
            LOGGER.Error($"消息处理失败，消息类型未绑定：{evt}");
            return;
        }
        
        await handleEvent.Invoke(actorId, evt);
    }

    /// <summary>
    /// 设置处理绑定关系
    /// </summary>
    /// <param name="handleEvents"></param>
    public static void SetHandleMap(Dictionary<int, HandleEvent> handleEvents)
    {
        HandleEventMap = handleEvents;
    }
}