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
	    public static string getMemoryPackSidStr(string namespaceName, string className, int sidValue,bool isOverride)
	    {
		    string overrideTag = isOverride ? " override " : " ";
		    var sourceBuilder = new StringBuilder($@"
using MemoryPack;
using  Geek.Server.Core.PolymorphicType;

namespace {namespaceName}
{{
    public partial class {className} : ITypeId
    {{
        [MemoryPackIgnore] public const int TYPE_ID = {sidValue};
        public{overrideTag}int TypeId => {sidValue};
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
		    
		    var sourceBuilder = new StringBuilder($@"

using System;
namespace {namespaceStr}
{{
	public static partial class MemoryPackTypeMapping
    {{
        //类型映射容器
        private static readonly Dictionary<int, Type> typeMapDict;

        static MemoryPackTypeMapping()
        {{
            typeMapDict = new System.Collections.Generic.Dictionary<int, Type>({kvDict.Count})
            {{
{kvStr}
            }};
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
}}

");
		    return sourceBuilder.ToString();
	    }
	    
        
    }
}