using System.Collections.Concurrent;
using System.Reflection;
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

        public static void RegisterAllPolymorphicTypes(params Assembly[] assemblies)
        {
            // 存储类型和其对应的ID
            var typeIdPairs = new Dictionary<Type, List<(int, Type)>>();

            foreach (var assembly in assemblies)
            {
                // 获取当前程序集中的所有类型
                var memoryPackableTypes = assembly.GetTypes()
                    .Where(t => t.GetCustomAttribute<MemoryPackableAttribute>() != null && !t.IsAbstract &&
                                !t.IsInterface)
                    .ToArray();

                foreach (var type in memoryPackableTypes)
                {
                    // 找到该类型的基类，且基类也是MemoryPackable
                    var baseType = type.BaseType;
                    while (baseType != null && baseType != typeof(object))
                    {
                        if (baseType.GetCustomAttribute<MemoryPackableAttribute>() != null)
                        {
                            if (!typeIdPairs.ContainsKey(baseType))
                            {
                                typeIdPairs[baseType] = new List<(int, Type)>();
                            }

                            typeIdPairs[baseType].Add((typeIdPairs[baseType].Count, type));
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

                for (int i = 0; i < kvp.Value.Count; i++)
                {
                    var tag = (ushort)kvp.Value[i].Item1;
                    Type type = kvp.Value[i].Item2;
                    derivedTypes[i] = (tag, type);
                }

                var formatterType = typeof(DynamicUnionFormatter<>).MakeGenericType(baseType);
                var formatter = Activator.CreateInstance(formatterType, new object[] { derivedTypes });

                // Get the Register<T> method
                var registerMethod = typeof(MemoryPackFormatterProvider).GetMethods(BindingFlags.Static | BindingFlags.Public)
                    .FirstOrDefault(m => m.Name == "Register" && m.IsGenericMethodDefinition);

                if (registerMethod != null)
                {
                    // Make the generic method specific to the baseType
                    var genericRegisterMethod = registerMethod.MakeGenericMethod(baseType);

                    // Invoke the method
                    genericRegisterMethod.Invoke(null, new object[] { formatter });
                    Console.WriteLine($"Registered {kvp.Value.Count} types derived from {baseType.FullName}");
                }
                else
                {
                    Console.WriteLine($"Could not find generic Register method for {baseType.FullName}");
                }

                Console.WriteLine($"Registered {kvp.Value.Count} types derived from {baseType.FullName}");
            }
        }
    }
}