namespace Geek.Server.Core.Serialize;

/// <summary>
/// 对象池接口
/// </summary>
public interface ISafeObjectPool
{
    void OnUse();
    
    void OnReturn();
    
    void Release();
}