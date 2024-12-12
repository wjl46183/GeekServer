using System;
using System.Collections.Generic;
using System.Text;
using Geek.Server.CodeGenerator.Utils;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Geek.Server.CodeGenerator.Comps
{
    public class CodeTemplate
    {
        /// <summary>
        /// 生成Comp
        /// </summary>
        /// <param name="namespaceName"></param>
        /// <param name="className"></param>
        /// <param name="sidValue"></param>
        /// <returns></returns>
        public static string getCompStr(string namespaceName,
            string actorType, string stateName, List<ClassDeclarationSyntax> dynamicTypes)
        {
            StringBuilder dynamicTypeAttributes = new StringBuilder();
            StringBuilder deactiveList = new StringBuilder();
            foreach (var dynamicType in dynamicTypes)
            {
                string classFullName = dynamicType.GetFullName();
                string typeValueName = ToLowerFirstChar(dynamicType.Identifier.Text);
                dynamicTypeAttributes.AppendLine(
                    $"\tpublic {classFullName} {typeValueName} = {classFullName}.Create();");
                deactiveList.AppendLine($"\t\t{typeValueName}.Release();\n\t\t{typeValueName} = null;");
            }

            var sourceBuilder = new StringBuilder($@"
using {namespaceName};
using Geek.Server.Core.Actors;
using Geek.Server.Core.Comps;

namespace {namespaceName}.Comp;

[Comp({actorType})]
public sealed partial class {stateName}Comp : StateComp<{stateName}>
{{
{dynamicTypeAttributes}
    public override Task Deactive()
    {{
{deactiveList}
		return base.Deactive();
    }}
}}
");
            return sourceBuilder.ToString();
        }


        public static string ToLowerFirstChar(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            char[] chars = input.ToCharArray();
            chars[0] = char.ToLower(chars[0]);

            return new string(chars);
        }
    }
}