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
            if (context.SyntaxReceiver is MemoryPackFilter receiver)
            {
                if (receiver.CandidateClasses.Count <= 0)
                {
                    return;
                }
                Dictionary<int, string> typeDict = new Dictionary<int, string>();
                foreach (var typeDeclaration in receiver.CandidateClasses)
                {
                    var namespaceName = GetNamespace(typeDeclaration);
                    var className = typeDeclaration.Identifier.Text;
                    var sidValue = className.GetHashCode();
                    Debug.WriteLine($"生成类型ID: {sidValue}");
                    var sourceBuilder = CodeTemplate.getMemoryPackSidStr(namespaceName, className, sidValue);
                    if (typeDict.ContainsKey(sidValue))
                    {
                        throw new Exception($"重复类型ID: {sidValue} 与类型: {typeDict[sidValue]} 重复，尝试修改类型名称");
                    }
                    typeDict[sidValue] = $"{namespaceName}.{className}";
                    context.AddSource($"{className}_MemoryPack_Sid.g.cs",
                        SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
                }
                context.AddSource($"MsgFactory.g.cs",
                    SourceText.From(CodeTemplate.getMsgFactoryStr(typeDict), Encoding.UTF8));
                Debug.Flush();
            }
        }

        private string GetNamespace(TypeDeclarationSyntax typeDeclaration)
        {
            // Traverse up the syntax tree to find the namespace declaration
            var namespaceDeclaration = typeDeclaration.Ancestors()
                .OfType<NamespaceDeclarationSyntax>()
                .FirstOrDefault();

            return namespaceDeclaration?.Name.ToString() ?? "GlobalNamespace";
        }
    }
}