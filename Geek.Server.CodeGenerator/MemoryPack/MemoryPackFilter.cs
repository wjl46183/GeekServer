using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Geek.Server.CodeGenerator.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Geek.Server.CodeGenerator.MemoryPack
{
    public class MemoryPackFilter : ISyntaxReceiver
    {
        
        public List<ClassDeclarationSyntax> CandidateClasses { get; } = new List<ClassDeclarationSyntax>();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            ClassDeclarationSyntax typeDeclaration = syntaxNode as ClassDeclarationSyntax;
            if(typeDeclaration == null)
            {
                return;
            }
            
            if (typeDeclaration.AttributeLists.Count <= 0)
            {
                return;
            }

            
            if (!typeDeclaration.AttributeLists.Any(attrList =>
                    attrList.Attributes.Any(attr => attr.Name.ToString() == "MemoryPackable" || attr.Name.ToString() == "MemoryPackableAttribute")))
            {
                return;
            }

            if (typeDeclaration.BaseList == null)
            {
                return;
            }

            bool isAdd = typeDeclaration.BaseList.Types.Any(baseType =>
            {
                LogMessage(typeDeclaration.GetFullName() + "   " + baseType.Type.ToString());
                return baseType.Type.ToString() == "Message";
            });
            
            if (isAdd)
            {
                CandidateClasses.Add(typeDeclaration);
            }
        }
        
        public static void LogMessage(string message)
        {
            System.Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "  " + message);
             var logFilePath = "/Users/wangjinliang/work/pixelminion/pixelminion-client/GeekServer/LogFile.log"; 
             using (var writer = new StreamWriter(logFilePath, append: true))
             {
                 writer.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "  " + message);
             }
        }
    }
}