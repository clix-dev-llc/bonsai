﻿using Microsoft.CSharp;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Bonsai.Editor
{
    class TypeDefinitionCodeProvider : CSharpCodeProvider
    {
        public override void GenerateCodeFromCompileUnit(CodeCompileUnit compileUnit, TextWriter writer, CodeGeneratorOptions options)
        {
            using (var indentedWriter = new IndentedTextWriter(writer))
            {
                foreach (CodeNamespace codeNamespace in compileUnit.Namespaces)
                {
                    GenerateCodeFromNamespace(codeNamespace, indentedWriter);
                    indentedWriter.WriteLine();
                }
            }
        }

        void GenerateCodeFromNamespace(CodeNamespace codeNamespace, IndentedTextWriter writer)
        {
            var hasDeclaration = !string.IsNullOrEmpty(codeNamespace.Name);
            if (hasDeclaration)
            {
                writer.WriteLine($"namespace {codeNamespace.Name}");
                writer.WriteLine("{");
                writer.Indent++;
            }

            foreach (var import in codeNamespace.Imports.Cast<CodeNamespaceImport>().OrderBy(ns => ns.Namespace))
            {
                WriteNamespaceImport(import, writer);
            }

            foreach (var type in codeNamespace.Types.Cast<CodeTypeDeclaration>())
            {
                WriteTypeDeclaration(type, writer);
            }

            if (hasDeclaration)
            {
                writer.Indent--;
                writer.WriteLine("}");
            }
        }

        void WriteNamespaceImport(CodeNamespaceImport import, IndentedTextWriter writer)
        {
            writer.WriteLine($"using {import.Namespace};");
        }

        void WriteTypeDeclaration(CodeTypeDeclaration type, IndentedTextWriter writer)
        {
            if (string.IsNullOrEmpty(type.Name)) return;
            foreach (var attribute in type.CustomAttributes.Cast<CodeAttributeDeclaration>())
            {
                WriteAttributeDeclaration(attribute, writer);
            }

            var typeKeyword = type.IsClass ? "class" : "struct";
            var baseTypes = string.Empty;
            if (type.BaseTypes.Count > 0)
            {
                baseTypes = $" : {string.Join(", ", type.BaseTypes.Cast<CodeTypeReference>().Select(baseType => GetTypeOutput(baseType)))}";
            }

            writer.WriteLine($"public {typeKeyword} {type.Name}{baseTypes}");
            writer.WriteLine("{");
            writer.Indent++;

            var properties = new List<CodeMemberProperty>();
            var methods = new List<CodeMemberMethod>();
            foreach (var member in type.Members.Cast<CodeTypeMember>())
            {
                if (member is CodeMemberProperty property) properties.Add(property);
                else if (member is CodeMemberMethod method)
                {
                    methods.Add(method);
                }
            }

            foreach (var property in properties)
            {
                WritePropertyDeclaration(property, writer);
                writer.WriteLine();
            }

            foreach (var method in methods)
            {
                WriteMethodDeclaration(method, writer);
            }

            writer.Indent--;
            writer.WriteLine("}");
        }

        void WriteAttributeDeclaration(CodeAttributeDeclaration attribute, IndentedTextWriter writer)
        {
            if (string.IsNullOrEmpty(attribute.Name)) return;
            var arguments = string.Empty;
            if (attribute.Arguments.Count > 0)
            {
                var argumentValues = attribute.Arguments.Cast<CodeAttributeArgument>().Select(argument =>
                {
                    if (argument.Value is CodePrimitiveExpression primitive)
                    {
                        if (primitive.Value == null) return "null";
                        else if (primitive.Value is string text) return $"\"{text}\"";
                        else return primitive.Value.ToString();
                    }
                    else if (argument.Value is CodeTypeOfExpression typeOf) return $"typeof({GetTypeOutput(typeOf.Type)})";
                    return string.Empty;
                });
                arguments = $"({string.Join(", ", argumentValues)})";
            }
            writer.WriteLine($"[{attribute.Name}{arguments}]");
        }

        void WritePropertyDeclaration(CodeMemberProperty property, IndentedTextWriter writer)
        {
            if (string.IsNullOrEmpty(property.Name) || !property.HasGet) return;
            foreach (var attribute in property.CustomAttributes.Cast<CodeAttributeDeclaration>())
            {
                WriteAttributeDeclaration(attribute, writer);
            }

            var getterSetter = property.HasSet ? "{ get; set; }" : "{ get; }";
            writer.WriteLine($"public {GetTypeOutput(property.Type)} {property.Name} {getterSetter}");
        }

        void WriteMethodDeclaration(CodeMemberMethod method, IndentedTextWriter writer)
        {
            if (string.IsNullOrEmpty(method.Name)) return;
            foreach (var attribute in method.CustomAttributes.Cast<CodeAttributeDeclaration>())
            {
                WriteAttributeDeclaration(attribute, writer);
            }

            var prefix = false;
            var typeParameters = string.Empty;
            if (method.TypeParameters.Count > 0)
            {
                typeParameters = $"<{string.Join(", ", method.TypeParameters.Cast<CodeTypeParameter>().Select(p => p.Name))}>";
            }

            writer.Write($"public {GetTypeOutput(method.ReturnType)} {method.Name}{typeParameters}(");
            foreach (var parameter in method.Parameters.Cast<CodeParameterDeclarationExpression>())
            {
                if (prefix) writer.Write(", ");
                WriteParameterDeclaration(parameter, writer);
                prefix = true;
            }
            writer.WriteLine(");");
        }

        void WriteParameterDeclaration(CodeParameterDeclarationExpression parameter, IndentedTextWriter writer)
        {
            string direction;
            switch (parameter.Direction)
            {
                case FieldDirection.Out: direction = "out "; break;
                case FieldDirection.Ref: direction = "ref "; break;
                case FieldDirection.In:
                default: direction = string.Empty; break;
            }
            writer.Write($"{direction}{GetTypeOutput(parameter.Type)} {parameter.Name}");
        }
    }
}