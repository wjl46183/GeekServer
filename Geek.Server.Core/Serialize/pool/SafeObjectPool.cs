using System.Collections.Concurrent;

namespace Geek.Server.Core.Serialize;

/// <summary>
/// 线程安全的对象池
/// </summary>
/// <typeparam name="T"></typeparam>
public class SafeObjectPool<T> where T : ISafeObjectPool
{
    private readonly ConcurrentBag<T> _objects;
    private readonly Func<T> _objectGenerator;

    // 构造函数，允许自定义对象生成函数
    public SafeObjectPool(Func<T> objectGenerator)
    {
        _objects = new ConcurrentBag<T>();
        _objectGenerator = objectGenerator;
    }

    // 获取对象
    public T GetObject()
    {
        if (_objects.TryTake(out T item))
        {
            return item;
        }

        return _objectGenerator();
    }

    // 回收对象
    public void ReturnObject(T item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item), "Returned object cannot be null");
        }

        _objects.Add(item);
    }
}