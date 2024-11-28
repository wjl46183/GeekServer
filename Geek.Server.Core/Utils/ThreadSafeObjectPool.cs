namespace Geek.Server.Core.Utils;

using System;
using System.Collections.Concurrent;

/// <summary>
/// 对象池，线程安全，无锁，性能高，适用于频繁创建和销毁的对象
/// </summary>
/// <typeparam name="T"></typeparam>
public class ThreadSafeObjectPool
{
    private readonly ConcurrentDictionary<int, object> _objects;

    // 构造函数，允许自定义对象生成函数
    public ThreadSafeObjectPool()
    {
        _objects = new ConcurrentDictionary<int, object>();
    }

    // 获取对象
    public T GetObject<T>(int key) where T : new()
    {
        return (T)_objects.GetOrAdd(key, (k) => new T());
    }

    // 回收对象
    public void ReturnObject(int key, object item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item), "Returned object cannot be null");
        }

        // 更新或添加对象到字典中
        _objects.AddOrUpdate(key, item, (k, existingVal) => item);
    }

    // 删除对象
    public bool RemoveObject(int key)
    {
        return _objects.TryRemove(key, out _);
    }
}
