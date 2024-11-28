using MemoryPack;

namespace Geek.Server.Core.Serialize;

public class MemoryPackTools
{
    public static bool IsImplementingIMemoryPackable(Type type)
    {
        // 获取该类型实现的所有接口
        var interfaces = type.GetInterfaces();

        // 检查是否有任何接口是 IMemoryPackable<T>
        bool hasGenericInterface = interfaces.Any(i =>
            i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMemoryPackable<>));

        return hasGenericInterface;
    }
}