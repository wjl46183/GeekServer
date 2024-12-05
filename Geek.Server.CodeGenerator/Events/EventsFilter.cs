using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Geek.Server.CodeGenerator.Events
{
    public class EventInfo
    {
        /// <summary>
        /// 事件优先级
        /// </summary>
        public int priority;

        /// <summary>
        /// 参数类型
        /// </summary>
        public string parameterType;

        /// <summary>
        /// 代理类型名称
        /// </summary>
        public string agentClassName;

        /// <summary>
        /// 事件方法名
        /// </summary>
        public string funcName;
    }

    public class EventFilter : ISyntaxReceiver
    {
        public Dictionary<int, List<EventInfo>> EventMap { get; } = new Dictionary<int, List<EventInfo>>();

        public List<string> errLog = new List<string>();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {

            if (syntaxNode is ClassDeclarationSyntax typeDeclaration)
            {
                errLog.Add(syntaxNode.ToString());

                // 判断类型继承自 BaseCompAgent<T>
                var baseType = typeDeclaration.BaseList?.Types.FirstOrDefault();
                bool isDerivedFromBaseCompAgent = baseType != null &&
                                                  baseType.Type.ToString().StartsWith("BaseCompAgent");
                if (isDerivedFromBaseCompAgent)
                {
                    var className = typeDeclaration.Identifier.Text;
                    // 遍历所有方法
                    foreach (var member in typeDeclaration.Members)
                    {
                        if (member is MethodDeclarationSyntax methodDeclaration)
                        {
                            foreach (var attributeList in methodDeclaration.AttributeLists)
                            {
                                foreach (var attribute in attributeList.Attributes)
                                {
                                    var attributeName = attribute.Name.ToString();
                                    if (attributeName == "BindEvent" || attributeName == "BindEventAttribute")
                                    {
                                        // 提取 BindEvent(1) 中的 1（priority 属性）
                                        int priority = 0;
                                        if (attribute.ArgumentList?.Arguments.Count > 0)
                                        {
                                            var argExpression = attribute.ArgumentList.Arguments[0].Expression;
                                            if (argExpression is LiteralExpressionSyntax literal &&
                                                literal.IsKind(SyntaxKind.NumericLiteralExpression))
                                            {
                                                priority = (int)literal.Token.Value;
                                            }
                                        }

                                        // 提取函数名
                                        var funcName = methodDeclaration.Identifier.Text;

                                        // 提取函数的第一个参数类型
                                        var parameterType = methodDeclaration.ParameterList.Parameters.FirstOrDefault()
                                            ?.Type.ToString();

                                        // 创建 EventInfo 并添加到 EventMap
                                        var eventInfo = new EventInfo
                                        {
                                            agentClassName = className,
                                            funcName = funcName,
                                            priority = priority,
                                            parameterType = parameterType
                                        };

                                        if (!EventMap.ContainsKey(priority))
                                        {
                                            EventMap[priority] = new List<EventInfo>();
                                        }

                                        EventMap[priority].Add(eventInfo);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}