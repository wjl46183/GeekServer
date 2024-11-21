using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Geek.Server.CodeGenerator.Utils
{
    public static class ClassDeclarationSyntaxExt
    {
        public const string NESTED_CLASS_DELIMITER = "+";
        public const string NAMESPACE_CLASS_DELIMITER = ".";

        public static string GetFullName(this ClassDeclarationSyntax source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var items = new List<string>();
            var parent = source.Parent;

            // Collect all parent class names (for nested classes)
            while (parent is ClassDeclarationSyntax parentClass)
            {
                items.Add(parentClass.Identifier.Text);
                parent = parent.Parent;
            }

            // Determine namespace or file-scoped namespace
            string namespaceName = string.Empty;
            if (parent is NamespaceDeclarationSyntax namespaceDeclaration)
            {
                namespaceName = namespaceDeclaration.Name.ToString();
            }
            else if (parent is FileScopedNamespaceDeclarationSyntax fileScopedNamespace)
            {
                namespaceName = fileScopedNamespace.Name.ToString();
            }

            // Build full name
            var sb = new StringBuilder();
            if (!string.IsNullOrEmpty(namespaceName))
            {
                sb.Append(namespaceName).Append(NAMESPACE_CLASS_DELIMITER);
            }

            // Add nested class names
            for (int i = items.Count - 1; i >= 0; i--)
            {
                sb.Append(items[i]).Append(NESTED_CLASS_DELIMITER);
            }

            // Add the actual class name
            sb.Append(source.Identifier.Text);

            return sb.ToString();
        }
        
        // public static string GetFullName(this ClassDeclarationSyntax classDeclaration)
        // {
        //     // 获取类名
        //     string className = classDeclaration.Identifier.Text;
        //
        //     // 获取命名空间名
        //     string namespaceName = GetNamespace(classDeclaration);
        //
        //     // 拼接命名空间和类名
        //     return string.IsNullOrEmpty(namespaceName) ? className : $"{namespaceName}.{className}";
        // }

        // private static string GetNamespace(SyntaxNode syntaxNode)
        // {
        //     // 用于构建完整命名空间
        //     Stack<string> namespaces = new Stack<string>();
        //
        //     // 遍历祖先节点以找到 NamespaceDeclarationSyntax
        //     SyntaxNode current = syntaxNode;
        //     while (current != null)
        //     {
        //         if (current is NamespaceDeclarationSyntax namespaceDeclaration)
        //         {
        //             namespaces.Push(namespaceDeclaration.Name.ToString());
        //         }
        //         else if (current is FileScopedNamespaceDeclarationSyntax fileScopedNamespace)
        //         {
        //             namespaces.Push(fileScopedNamespace.Name.ToString());
        //         }
        //         current = current.Parent;
        //     }
        //
        //     return string.Join(".", namespaces);
        // }

// 示例用法
// 假设 classDeclaration 是一个有效的 ClassDeclarationSyntax 实例
// string fullName = FullNameExtractor.GetFullName(classDeclaration);
// Console.WriteLine($"Full Name: {fullName}");




    }
}
