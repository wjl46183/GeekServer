using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Geek.Server.CodeGenerator.MemoryPack
{
    public class MemoryPackFilter : ISyntaxReceiver
    {
        public List<ClassDeclarationSyntax> CandidateClasses { get; } = new List<ClassDeclarationSyntax>();
        
        public ClassDeclarationSyntax MappedClass { get; set; }

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is ClassDeclarationSyntax typeDeclaration)
            {
                if (typeDeclaration.AttributeLists.Count > 0 &&
                    typeDeclaration.AttributeLists.Any(attrList =>
                        attrList.Attributes.Any(attr => attr.Name.ToString() == "MemoryPackable" || attr.Name.ToString() == "MemoryPackableAttribute")))
                {
                    CandidateClasses.Add(typeDeclaration);
                }
            }
        }

    }
}
