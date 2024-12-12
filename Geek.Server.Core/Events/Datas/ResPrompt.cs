using Geek.Server.Core.Net;
using MemoryPack;

namespace Geek.Server.Core.Events.Datas;


/// <summary>
/// 客户端每次请求都会回复错误码
/// </summary>
[MemoryPackable]
public partial class ResErrorCode : BaseEvent
{
    public static ResErrorCode Create(int code, string msg)
    {
        var evt = ResErrorCode.Create();
        evt.ErrCode = code;
        evt.Desc = msg;
        return evt;
    }
}