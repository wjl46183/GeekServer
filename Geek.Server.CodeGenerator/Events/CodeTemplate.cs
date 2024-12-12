using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Geek.Server.CodeGenerator.Utils;

namespace Geek.Server.CodeGenerator.Events
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
        public static string getEventHandleClassStr(string namespaceName,Dictionary<string, Dictionary<int,List<EventInfo>>> eventMap)
        {
            var funcArr = new StringBuilder();
            var funcKVArr = new StringBuilder();
            foreach (var keyValuePair in eventMap)
            {
                funcArr.Append(getFuncContextStr(keyValuePair.Key, keyValuePair.Value));
                funcKVArr.AppendLine($"\t\t\t{{{Tools.GetStringHash(keyValuePair.Key)},On{keyValuePair.Key}}},");
            }

            var sourceBuilder = new StringBuilder(
$@"using Geek.Server.Core.Net;
using Geek.Server.Core.Actors;
using Geek.Server.Storage;
using Geek.Server.HotData.Proto;
namespace {namespaceName}
{{


    public static class EventMapings
    {{
        /// <summary>
        /// 事件绑定函数字典
        /// </summary>
        public static Dictionary<int,Geek.Server.Core.Events.EventHandleMgr.HandleEvent> typeIdHandleFuncs = new (){{
{funcKVArr}        }};

{funcArr}
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
        public static string getFuncContextStr(string eventClassName, Dictionary<int, List<EventInfo>> eventInfoDict)
        {
            List<int> keys = eventInfoDict.Keys.ToList();
            keys.Sort();

            Dictionary<string,bool> agentDict = new Dictionary<string, bool>();
            var kvStr = new StringBuilder();
            foreach (var key in keys)
            {
                kvStr.AppendLine("\t\t\t//优先级：" + key);
                var list = eventInfoDict[key];
                foreach (var eventInfo in list)
                {
                    if (!agentDict.ContainsKey(eventInfo.agentFullClassName))
                    {
                        kvStr.AppendLine($"\t\t\tvar tmp{eventInfo.agentFullClassName.GetWithoutNamespace()} = await ActorMgr.GetCompAgent<{eventInfo.agentFullClassName}>(actorId);");
                        agentDict[eventInfo.agentFullClassName] = true;
                    }
                    kvStr.AppendLine($"\t\t\tawait tmp{eventInfo.agentFullClassName.GetWithoutNamespace()}.{eventInfo.funcName}(tmp{eventInfo.parameterType});");
                }
                //单个Actor没有并发，不需要处理，后期有一个事件给多个人处理的时候需要
            }


            var sourceBuilder = new StringBuilder($@"
        public static async ValueTask On{eventClassName}(long actorId, BaseEvent evt)
        {{
            var tmp{eventClassName} = evt as {eventClassName};
{kvStr}
        }}
");
            return sourceBuilder.ToString();
        }
        
        public static string GetStringAfterLastDot(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }
            int lastDotIndex = input.LastIndexOf('.');
            if (lastDotIndex != -1)
            {
                return input.Substring(lastDotIndex + 1);
            }
            return input;
        }
    }
}