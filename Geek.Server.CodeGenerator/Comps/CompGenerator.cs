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

namespace Geek.Server.CodeGenerator.Comps
{
    [Generator]
    public class CompGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            // Debugger.Launch();
            ResLoader.LoadDll();
            context.RegisterForSyntaxNotifications(() => new CompFilter());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            Logger.LogNormal(context,
                $"程序集： {context.Compilation.AssemblyName} 自动生成 Comp 代码 -> 开始");
            
            if (context.SyntaxReceiver is CompFilter receiver)
            {
                if (receiver.errorLog.Count > 0)
                {
                    foreach (var s in receiver.errorLog)
                    {
                        Logger.LogError(context,s);
                    }
                }
                
                if (receiver.StateClasses.Count > 0)
                {
                    foreach (var stateInfo in receiver.StateClasses)
                    {
                        var typeDeclaration = stateInfo.Value.StateClass;
                        var namespaceName = GetNamespace(typeDeclaration);
                        var className = stateInfo.Value.className + "Comp";
                        
                        var sourceBuilder = CodeTemplate.getCompStr(namespaceName, stateInfo.Value.actorType,
                            stateInfo.Value.className, stateInfo.Value.DynamicClasses);
                        
                        context.AddSource($"{className}.g.cs",
                            SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
                    }
                }
            }

            Logger.LogNormal(context,
                $"程序集： {context.Compilation.AssemblyName} 自动生成 Comp 代码 -> 完成");
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