namespace Geek.Server.Core.Serialize;

/// <summary>
/// 默认实现的对象池
/// </summary>
public class BaseSafeObjectPool : ISafeObjectPool
{
    public virtual void OnUse()
    {
        
    }

    public virtual void OnReturn()
    {
    }
}