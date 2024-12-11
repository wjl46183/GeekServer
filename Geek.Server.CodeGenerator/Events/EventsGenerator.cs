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

namespace Geek.Server.CodeGenerator.Events
{
    [Generator]
    public class EventSourceGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            // Debugger.Launch();
            ResLoader.LoadDll();
            context.RegisterForSyntaxNotifications(() => new EventFilter());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            Logger.LogNormal(context,
                $"程序集： {context.Compilation.AssemblyName} 自动生成 Events 代码 -> 开始");
            if (context.SyntaxReceiver is EventFilter receiver)
            {
                foreach (var s in receiver.errLog)
                {
                    Logger.LogNormal(context,
                        $"过滤日志：{s}");
                }

                if (receiver.EventMap.Count > 0)
                {
                    string codeStr = CodeTemplate.getEventHandleClassStr(context.Compilation.AssemblyName + ".EventHandle",receiver.EventMap);
                    context.AddSource($"EventMapings.g.cs",
                        SourceText.From(codeStr, Encoding.UTF8));
                }
               
            }
            
            Logger.LogNormal(context,
                $"程序集： {context.Compilation.AssemblyName} 自动生成 Events 代码 -> 完成");
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