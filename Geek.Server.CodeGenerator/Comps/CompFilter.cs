using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Geek.Server.CodeGenerator.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Geek.Server.CodeGenerator.Comps
{
    public class StateTypeInfo
    {
        public string className;
        public string actorType;
        public ClassDeclarationSyntax StateClass { get; set; }
        public List<ClassDeclarationSyntax> DynamicClasses { get; set; } = new List<ClassDeclarationSyntax>();
    }

    public class CompFilter : ISyntaxReceiver
    {
        public Dictionary<string, StateTypeInfo> StateClasses { get; } = new Dictionary<string, StateTypeInfo>();

        public List<string> errorLog = new List<string>();
        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            try
            {
                if (syntaxNode is ClassDeclarationSyntax typeDeclaration)
                {
                    string saveStateActorType = "";
                    bool isSaveState = typeDeclaration.AttributeLists.Any(attrList =>
                        attrList.Attributes.Any(attr =>
                        {
                            isSaveState = attr.Name.ToString() == "SaveState" ||
                                          attr.Name.ToString() == "SaveStateAttribute";
                            if (isSaveState)
                            {
                                saveStateActorType = attr.ToString().Replace("SaveState(", "").Replace(")", "");
                            }
                            return isSaveState;
                        }));
                            
                    if (isSaveState)
                    {
                        // 判断 syntaxNode 的类型是否是继承自 BaseState
                        var baseType = typeDeclaration.BaseList?.Types
                            .Select(t => t.ToString())
                            .FirstOrDefault(bt => bt == "BaseState");

                        if (baseType == null)
                        {
                            // 如果没有继承自 BaseState，您可以在这里报告诊断
                            errorLog.Add($"类型：{typeDeclaration.GetFullName()} 需要没有继承类型：BaseState");
                        }
                        if (!StateClasses.ContainsKey(typeDeclaration.Identifier.Text))
                        {
                            StateClasses[typeDeclaration.Identifier.Text] = new StateTypeInfo()
                            {
                                actorType = saveStateActorType,
                                className = typeDeclaration.Identifier.Text,
                                StateClass = typeDeclaration,
                                DynamicClasses = new List<ClassDeclarationSyntax>()
                            };
                        }
                        else
                        {
                            StateClasses[typeDeclaration.Identifier.Text].StateClass = typeDeclaration;
                            StateClasses[typeDeclaration.Identifier.Text].actorType = saveStateActorType;
                        }
                    }



                    bool isDynamicState = typeDeclaration.AttributeLists.Any(attrList =>
                    {
                        return attrList.Attributes.Any(attr =>
                        {
                            // 检查属性的名称是否是 GenericNameSyntax
                            if (attr.Name is GenericNameSyntax genericName)
                            {
                                // 提取泛型参数列表中的第一个参数
                                var genericArgument = genericName.TypeArgumentList.Arguments.FirstOrDefault();
                                if (genericArgument != null)
                                {
                                    return genericName.Identifier.Text == "DynamicState" || genericName.Identifier.Text == "DynamicStateAttribute";
                                }
                            }
                            return false;
                        });
                    });
                        
                            
                    if (isDynamicState)
                    {
                        // 判断 syntaxNode 的类型是否是实现了接口：IDynamicState
                        var implementedInterfaces = typeDeclaration.BaseList?.Types
                            .Select(t => t.ToString())
                            .ToList();
                        if (implementedInterfaces == null || !implementedInterfaces.Contains("IDynamicState"))
                        {
                            // 如果没有实现接口 IDynamicState，您可以在这里报告诊断
                            errorLog.Add($"类型：{typeDeclaration.GetFullName()} 需要没有实现接口：IDynamicState");
                        }
                        else
                        {
                            TypeSyntax bindType = getDynamicBindStateType(typeDeclaration);
                            string className = bindType.ToString();
                            if (!StateClasses.ContainsKey(className))
                            {
                                StateClasses[className] = new StateTypeInfo()
                                {
                                    className = className,
                                    DynamicClasses = new List<ClassDeclarationSyntax>()
                                };
                            }
                            StateClasses[className].DynamicClasses.Add(typeDeclaration);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                errorLog.Add(e.Message);
            }
        }

        /// <summary>
        /// 获取状态类型
        /// </summary>
        /// <param name="classDeclaration"></param>
        /// <returns></returns>
        private TypeSyntax getDynamicBindStateType(ClassDeclarationSyntax classDeclaration)
        {
            foreach (var attributeList in classDeclaration.AttributeLists)
            {
                foreach (var attribute in attributeList.Attributes)
                {
                    // 检查属性的名称是否是 GenericNameSyntax
                    if (attribute.Name is GenericNameSyntax genericName)
                    {
                        if (genericName.Identifier.Text == "DynamicState" ||
                            genericName.Identifier.Text == "DynamicStateAttribute")
                        {
                            // 提取泛型参数列表中的第一个参数
                            var genericArgument = genericName.TypeArgumentList.Arguments.FirstOrDefault();
        
                            if (genericArgument != null)
                            {
                                return genericArgument;
                            }
                        }  
                    }
                }
            }

            return null;
        }
    }
}