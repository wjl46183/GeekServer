using System;
using System.Collections.Generic;
using System.Text;

namespace Geek.Server.CodeGenerator.MemoryPack
{
    public class CodeTemplate
    {

	    /// <summary>
	    /// 生成MemoryPackSid
	    /// </summary>
	    /// <param name="namespaceName"></param>
	    /// <param name="className"></param>
	    /// <param name="sidValue"></param>
	    /// <returns></returns>
	    public static string getMemoryPackSidStr(string namespaceName, string className, int sidValue,bool isOverride,bool isPoolInterface,bool newTYPEID)
	    {
		    string poolType = isPoolInterface? "" : "BaseSafeObjectPool, ";
		    string overrideTag = isOverride ? " override " : " ";
		    var sourceBuilder = new StringBuilder($@"
using MemoryPack;
using  Geek.Server.Core.PolymorphicType;
using Geek.Server.Core.Serialize;

namespace {namespaceName}
{{
    public partial class {className} : {poolType}ITypeId
    {{
        [MemoryPackIgnore] public{(newTYPEID ? " new ": " ")}const int TYPE_ID = {sidValue};
        public{overrideTag}int TypeId => {sidValue};

		private static SafeObjectPool<{className}> _Pool = new (_New);
        
		static {className} _New(){{
			return new {className}();
		}}

        protected {className}()
        {{
            
        }}

        public static{(newTYPEID ? " new " :" ")}{className} Create()
        {{
            var obj = _Pool.GetObject();
            obj.OnUse();
            return obj;
        }}

        
        public override void Release()
        {{
            OnReturn();
            _Pool.ReturnObject(this);
        }}
    }}
}}
");
		    return sourceBuilder.ToString();
	    }
	    
	    /// <summary>
	    /// 生成MsgFactory
	    /// </summary>
	    /// <param name="kvStr"></param>
	    /// <returns></returns>
	    public static string getMsgFactoryStr(string namespaceStr,Dictionary<int, string> kvDict)
	    {
		    var kvStr = new StringBuilder();
		    foreach (var pair in kvDict)
		    {
			    kvStr.AppendLine($"                {{{pair.Key}, typeof({pair.Value})}},");
		    }
		    
		    var kcStr = new StringBuilder();
		    foreach (var pair in kvDict)
		    {
			    kcStr.AppendLine($"                {{typeof({pair.Value}),{pair.Value}.Create }},");
		    }
		    
		    var sourceBuilder = new StringBuilder($@"

using System;
using Geek.Server.Core.Net;
namespace {namespaceStr};

public static partial class MemoryPackTypeMapping
{{
    //类型映射容器
    private static readonly Dictionary<int, Type> typeMapDict;

	//类型映射构建器
    private static readonly Dictionary<Type, Func<object>> typeCreateDict;

    static MemoryPackTypeMapping()
    {{
        typeMapDict = new System.Collections.Generic.Dictionary<int, Type>({kvDict.Count})
        {{
{kvStr}
        }};


        typeCreateDict = new System.Collections.Generic.Dictionary<Type, Func<object>>({kvDict.Count})
        {{
{kcStr}
        }};

    }}

	/// <summary>
    /// 构建指定类型对象
    /// </summary>
    public static T Create<T>()
    {{
        if (typeCreateDict.TryGetValue(typeof(T), out Func<object> func))
        {{
            return (T)(func.Invoke());
        }}
        else
        {{
            throw new Exception($""找不到指定Type对应的构造器 :{{typeof(T)}} 检查前后端协议是否同步"");
        }}
    }}

	/// <summary>
    /// 构建指定类型对象
    /// </summary>
    public static Message Create(Type type)
    {{
        if (typeCreateDict.TryGetValue(type, out Func<object> func))
        {{
            return (Message)(func.Invoke());
        }}
        else
        {{
            throw new Exception($""找不到指定Type对应的构造器 :{{type}} 检查前后端协议是否同步"");
        }}
    }}
    
    /// <summary>
    /// 获取指定消息ID对应的类型
    /// </summary>
    public static Type GetType(int typeId)
    {{
        if (typeMapDict.TryGetValue(typeId, out Type res))
        {{
            return res;
        }}
        else
        {{
            throw new Exception($""找不到指定ID对应的类型 :{{typeId}} 检查前后端协议是否同步"");
        }}
    }}
}}

");
		    return sourceBuilder.ToString();
	    }
	    
        
    }
}