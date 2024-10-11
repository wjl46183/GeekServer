using cfg;
using Luban;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NLog;

public class ConfigManager
{
    private static readonly NLog.Logger LOGGER = LogManager.GetCurrentClassLogger();
    
    public static Tables tables;
    private static byte[] BUFFER = new byte[1024 * 1024 * 10];
    private static Dictionary<string, FileStream> fileStreams;
    
    /// <summary>
    /// 加载配置表
    /// </summary>
    public static ValueTuple<bool,string> LoadTables()
    {
        try
        {
            fileStreams = new Dictionary<string, FileStream>(Tables.TABLE_COUNT / 2);
            tables = new cfg.Tables(LoadOffsetByteBuf, ByteBufLoader);
            tables.LoadAll();
            initConfigEx();
        }
        catch (Exception e)
        {
            return (false, e.StackTrace);
        }
        finally
        {
            if (fileStreams?.Values != null)
            {
                foreach (var fs in fileStreams.Values)
                {
                    fs.Dispose();
                }
                fileStreams?.Clear();
            }
        }

        return (true, "");

    }
    
    /// <summary>
    /// 加载配置表偏移量
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    private static ByteBuf LoadOffsetByteBuf(string file)
    {
        var a = File.ReadAllBytes("exportData/offset/" + file + ".bytes");
        return new ByteBuf(a);
    }

    /// <summary>
    /// 加载配置表
    /// </summary>
    /// <param name="file"></param>
    /// <param name="offset"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    private static ByteBuf ByteBufLoader(string file, int offset, int length)
    {
        if (!fileStreams.TryGetValue(file, out var fs))
        {
            fs = new FileStream("exportData/bytes/" + file + ".bytes", FileMode.Open);
            fileStreams.Add(file, fs);
        }
        fs.Seek(offset, SeekOrigin.Begin);
        fs.Read(BUFFER, 0, length);
        var buf = new ByteBuf(BUFFER, 0, length);
        return buf;
    }
    
    /// <summary>
    /// 初始化扩展配置数据
    /// </summary>
    private static void initConfigEx()
    {
        Assembly currentAssembly = Assembly.GetExecutingAssembly();
        //初始化各个扩展配置数据接口
        ForeachAssemblyChildClassByInterface<IConfigEx>(currentAssembly,
            (obj) =>
            {
                obj.onInit();
            });
    }
    
    /// <summary>
    /// 遍历assembly下的所有实现了T接口的类
    /// </summary>
    /// <param name="assembly"></param>
    /// <param name="oneCall"></param>
    /// <typeparam name="T"></typeparam>
    private static void ForeachAssemblyChildClassByInterface<T>(Assembly assembly, Action<T> oneCall)
    {
        if (assembly == null)
        {
            LOGGER.Error("assembly 不能为空");
            return;
        }

        Type[] typeArr = assembly.GetTypes();
        for (int index = 0; index < typeArr.Length; index++)
        {
            Type type = typeArr[index];
            Type[] interfaces = type.GetInterfaces();
            for (int indey = 0; indey < interfaces.Length; indey++)
            {
                if (interfaces[indey].Name.Equals(typeof(T).Name))
                {
                    T sysObj = (T) assembly.CreateInstance(type.FullName);
                    oneCall(sysObj);
                }
            }
        }
    }
}
