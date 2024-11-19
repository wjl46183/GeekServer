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
	    public static string getMemoryPackSidStr(string namespaceName, string className, int sidValue)
	    {
		    var sourceBuilder = new StringBuilder($@"
using MemoryPack;

namespace {namespaceName}
{{
    public partial class {className}
    {{
        [MemoryPackIgnore] public const int MsgID = {sidValue};
        public override int MsgId => MsgID;
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
	    public static string getMsgFactoryStr(Dictionary<int, string> kvDict)
	    {
		    
		    var kvStr = new StringBuilder();
		    foreach (var pair in kvDict)
		    {
			    kvStr.AppendLine($"                {{{pair.Key}, typeof({pair.Value})}},");
		    }
		    
		    var sourceBuilder = new StringBuilder($@"

using System;
namespace Geek.Server.Proto
{{
	public class MsgFactory
	{{
		//类型映射容器
		private static readonly System.Collections.Generic.Dictionary<int, Type> lookup;

        static MsgFactory()
        {{
            lookup = new System.Collections.Generic.Dictionary<int, Type>(19)
            {{
{kvStr}
            }};
        }}

		/// <summary>
        /// 获取指定消息ID对应的类型
        /// </summary>
        public static Type GetType(int msgId)
		{{
			if (lookup.TryGetValue(msgId, out Type res))
				return res;
			else
				throw new Exception($""can not find msg type :{{msgId}}"");
		}}

	}}
}}
");
		    return sourceBuilder.ToString();
	    }
	    
        
    }
}