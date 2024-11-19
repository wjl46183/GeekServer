using System.Collections.Concurrent;
using System.Reflection;
using System.Text;
using MemoryPack;
using MemoryPack.Formatters;

namespace PolymorphicMessagePack
{
    public sealed class PolymorphicResolver
    {
        public static PolymorphicResolver Instance { get; private set; } = new PolymorphicResolver();

        public void Init()
        {
            PolymorphicTypeMapper.RegisterCore();

            // 注册多态类型
            RegisterAllPolymorphicTypes(GetGeekServerAssemblies());
        }

        public static Assembly[] GetGeekServerAssemblies()
        {
            // 获取当前应用程序域中加载的所有程序集
            var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            // 筛选出以 "Geek.Server" 开头的程序集
            var geekServerAssemblies = allAssemblies
                .Where(assembly =>
                    assembly.GetName().Name.StartsWith("Geek.Server", StringComparison.OrdinalIgnoreCase))
                .ToArray();

            return geekServerAssemblies;
        }
        
        private static ushort ComputeCRC16(byte[] data)
        {
            const ushort polynomial = 0xA001;
            ushort crc = 0xFFFF;

            foreach (byte b in data)
            {
                crc ^= b;

                for (int i = 0; i < 8; i++)
                {
                    if ((crc & 1) != 0)
                    {
                        crc = (ushort)((crc >> 1) ^ polynomial);
                    }
                    else
                    {
                        crc >>= 1;
                    }
                }
            }

            return crc;
        }

        public static void RegisterAllPolymorphicTypes(params Assembly[] assemblies)
        {
            // 存储类型和其对应的ID
            var typeIdPairs = new Dictionary<Type, Dictionary<int, Type>>();

            foreach (var assembly in assemblies)
            {
                // 获取当前程序集中的所有类型
                var memoryPackableTypes = assembly.GetTypes()
                    .Where(t => t.GetCustomAttribute<MemoryPackableAttribute>() != null && !t.IsAbstract &&
                                !t.IsInterface)
                    .ToArray();

                
                foreach (var type in memoryPackableTypes)
                {
                    Dictionary<ushort,bool> usageIdSet = new Dictionary<ushort,bool>();
                    // 找到该类型的基类，且基类也是MemoryPackable
                    var baseType = type.BaseType;
                    while (baseType != null && baseType != typeof(object))
                    {
                        if (baseType.GetCustomAttribute<MemoryPackableAttribute>() != null)
                        {
                            if (!typeIdPairs.ContainsKey(baseType))
                            {
                                typeIdPairs[baseType] = new Dictionary<int, Type>();
                            }
                            var hash = ComputeCRC16(Encoding.UTF8.GetBytes(type.FullName));
                            if (typeIdPairs[baseType].ContainsKey(hash))
                            {
                                throw new Exception($"生成的Hash冲突： id {hash} for {type.FullName} 尝试修改类型名称，重新运行游戏");
                            }
                            typeIdPairs[baseType][hash] = type;
                            break;
                        }

                        baseType = baseType.BaseType;
                    }
                }
            }

            // 注册每个基类和其派生类
            foreach (var kvp in typeIdPairs)
            {
                var baseType = kvp.Key;
                (ushort Tag, Type Type)[] derivedTypes = new (ushort Tag, Type Type)[kvp.Value.Count];

                int index = 0;
                foreach (var keyValuePair in kvp.Value)
                {
                    var tag = (ushort)keyValuePair.Key;
                    Type type = keyValuePair.Value;
                    derivedTypes[index++] = (tag, type);
                }

                var formatterType = typeof(DynamicUnionFormatter<>).MakeGenericType(baseType);
                var formatter = Activator.CreateInstance(formatterType, new object[] { derivedTypes });

                var registerMethod = typeof(MemoryPackFormatterProvider).GetMethods(BindingFlags.Static | BindingFlags.Public)
                    .FirstOrDefault(m => m.Name == "Register" && m.IsGenericMethodDefinition);

                var genericRegisterMethod = registerMethod.MakeGenericMethod(baseType);

                genericRegisterMethod.Invoke(null, new object[] { formatter });
                Console.WriteLine($"动态注册MemoryPack基类： {baseType.FullName} -> {kvp.Value.Count}");
            }
        }
    }
}