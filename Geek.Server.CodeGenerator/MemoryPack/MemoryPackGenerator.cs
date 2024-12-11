using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Text;
using Geek.Server.CodeGenerator.Agent;
using Geek.Server.CodeGenerator.Utils;

namespace Geek.Server.CodeGenerator.MemoryPack
{
    [Generator]
    public class MemoryPackSourceGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            // Debugger.Launch();
            ResLoader.LoadDll();
            context.RegisterForSyntaxNotifications(() => new MemoryPackFilter());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            Logger.LogNormal(context,
                $"程序集： {context.Compilation.AssemblyName} 自动生成 MemoryPack 代码 -> 开始");
            if (context.SyntaxReceiver is MemoryPackFilter receiver)
            {
                if (receiver.CandidateClasses.Count > 0)
                {
                    Dictionary<int, string> typeDict = new Dictionary<int, string>();
                    foreach (var typeDeclaration in receiver.CandidateClasses)
                    {
                        var namespaceName = GetNamespace(typeDeclaration);
                        var className = typeDeclaration.Identifier.Text;
                        var sidValue =  Tools.GetStringHash(className);

                        bool isOverride = false;
                        bool isPoolinterface = false;
                        bool hasTypeIdInBase = false;
                        var model = context.Compilation.GetSemanticModel(typeDeclaration.SyntaxTree);
                        var classSymbol = model.GetDeclaredSymbol(typeDeclaration);
                        if (classSymbol != null)
                        {
                            var interfaces = classSymbol.AllInterfaces;
                            isOverride = interfaces.Any(interfaceSymbol =>
                                interfaceSymbol.Name.ToString() == "ITypeId");
                            isPoolinterface = interfaces.Any(interfaceSymbol =>
                                interfaceSymbol.Name.ToString() == "ISafeObjectPool");
                            hasTypeIdInBase = classSymbol.BaseType?.Name != "Message" && classSymbol.BaseType.GetAttributes().Any(attr => attr.AttributeClass?.Name == "MemoryPackableAttribute");
                        }

                        var sourceBuilder = CodeTemplate.getMemoryPackSidStr(namespaceName, className, sidValue,
                            isOverride, isPoolinterface,hasTypeIdInBase);
                        if (typeDict.ContainsKey(sidValue))
                        {
                            Logger.LogError(context,
                                $"重复类型ID: {sidValue} 与类型: {typeDict[sidValue]} 重复，尝试修改类型名称");
                            throw new Exception($"重复类型ID: {sidValue} 与类型: {typeDict[sidValue]} 重复，尝试修改类型名称");
                        }

                        typeDict[sidValue] = $"{namespaceName}.{className}";
                        context.AddSource($"{className}_MemoryPack_Sid.g.cs",
                            SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
                    }

                    string mapNamespaceName = context.Compilation.AssemblyName;
                    context.AddSource($"MemoryPackTypeMapping.g.cs",
                        SourceText.From(CodeTemplate.getMsgFactoryStr(mapNamespaceName, typeDict), Encoding.UTF8));
                }
            }

            Logger.LogNormal(context,
                $"程序集： {context.Compilation.AssemblyName} 自动生成 MemoryPack 代码 -> 完成");
        }

        private string GetNamespace(TypeDeclarationSyntax typeDeclaration)
        {
            // 首先检查文件作用域的命名空间
            var parent = typeDeclaration.Parent;
            while (parent != null)
            {
                if (parent is FileScopedNamespaceDeclarationSyntax fileScopedNamespace)
                {
                    return fileScopedNamespace.Name.ToString();
                }
                else if (parent is NamespaceDeclarationSyntax namespaceDeclaration)
                {
                    return namespaceDeclaration.Name.ToString();
                }

                parent = parent.Parent;
            }

            return "GlobalNamespace";
        }
    }
}