﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Reflection;
using System.Text;

namespace ClassLibrary.Generator;

[Generator(LanguageNames.CSharp)]
public class MyActionDispatcherGenerator : IIncrementalGenerator
{
    private const string FullyQualifiedMetadataName = "ClassLibrary.MyActionAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Identify methods with the [MyAction] attribute
        var methodsWithMyAction = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                FullyQualifiedMetadataName,
                static (node, _) => node is MethodDeclarationSyntax method &&
                                     method.Modifiers.Any(SyntaxKind.StaticKeyword) &&
                                     method.ReturnType.ToString() is "string" or "System.String",
                static (context, _) => ExtractMethodData(context)
            )
            .Where(static method => method is not null)
            .Collect();

        // Register the source output
        context.RegisterSourceOutput(methodsWithMyAction, GenerateDispatcher);
    }

    private static (string? ClassName, string? MethodName, ImmutableArray<IParameterSymbol> Parameters)? ExtractMethodData(GeneratorAttributeSyntaxContext context)
    {
        if (context.TargetNode is not MethodDeclarationSyntax methodSyntax ||
            methodSyntax.Parent is not ClassDeclarationSyntax classSyntax ||
            context.SemanticModel.GetDeclaredSymbol(methodSyntax) is not IMethodSymbol methodSymbol)
        {
            return null;
        }

        var className = classSyntax.Identifier.Text;
        return (className, methodSymbol.Name, methodSymbol.Parameters);
    }

    private static void GenerateDispatcher(SourceProductionContext context, ImmutableArray<(string? ClassName, string? MethodName, ImmutableArray<IParameterSymbol> Parameters)?> methods)
    {
        var validMethods = methods.Where(m => m is not null).Select(m => m!.Value).ToArray();

        var sourceBuilder = new StringBuilder();

        AppendHeader(sourceBuilder);
        AppendDispatchMethod(sourceBuilder, validMethods);
        AppendFooter(sourceBuilder);

        context.AddSource("MyActionDispatcher.g.cs", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
    }

    private static void AppendHeader(StringBuilder builder)
    {
        string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        builder.AppendLine("""
            using System;
            using System.Collections.Generic;
            using System.Globalization;
            using System.CodeDom.Compiler;

            namespace ClassLibrary;

            /*
            ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            This class is auto-generated by MyActionDispatcherGenerator.
            Please DO NOT modify it manually!
            ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            */
            """);
        builder.AppendLine($"[GeneratedCodeAttribute(\"ClassLibrary.Generator.MyActionDispatcherGenerator\", \"{version}\")]");
        builder.AppendLine("""    
            public class MyGeneratedActionDispatcher : IActionDispatcher
            {
            """);
    }

    private static void AppendDispatchMethod(StringBuilder builder, (string? ClassName, string? MethodName, ImmutableArray<IParameterSymbol> Parameters)[] methods)
    {
        builder.AppendLine("""
            public string Dispatch(string actionName, Dictionary<string, string> args)
            {
        """);

        if (methods.Length == 0)
        {
            builder.AppendLine("            throw new NotSupportedException(\"There are no valid methods to be called via the ActionDispatcher.\");");
        }
        else
        {
            builder.AppendLine("""                
                        switch (actionName)
                        {
                """);

            foreach (var (className, methodName, parameters) in methods)
            {
                builder.AppendLine($"            case \"{methodName}\":");
                builder.AppendLine("            {");

                if (parameters.Length > 0)
                {
                    var parsedArgs = BuildArgumentParsing(parameters);
                    builder.AppendLine(parsedArgs);
                }

                builder.AppendLine($"                return {className}.{methodName}({BuildArgumentNames(parameters)});");
                builder.AppendLine("            }");
            }

            builder.AppendLine("""
                        default:
                        {
                            throw new ArgumentException($"No action found for: {actionName}");
                        }
                    }
            """);
        }
    }

    private static string BuildArgumentParsing(ImmutableArray<IParameterSymbol> parameters)
    {
        var parsedArgs = new StringBuilder();

        foreach (var parameter in parameters)
        {
            var paramName = parameter.Name;
            var parseMethod = GetParseMethod(parameter.Type, parameter.NullableAnnotation == NullableAnnotation.Annotated);
            var defaultValue = parameter.HasExplicitDefaultValue
                ? GetDefaultValueLiteral(parameter)
                : "default";

            if (parameter.IsOptional)
            {
                parsedArgs.AppendLine($"""
                                    var arg_{paramName} = args.TryGetValue("{paramName}", out var value_{paramName}) && !String.IsNullOrWhiteSpace(value_{paramName})
                                        ? {parseMethod}(value_{paramName})
                                        : {defaultValue.ToLowerInvariant()};
                    """);
            }
            else
            {
                parsedArgs.AppendLine($"""
                                    if (!args.TryGetValue("{paramName}", out var value_{paramName}) || String.IsNullOrWhiteSpace(value_{paramName}))
                                        throw new ArgumentException("Missing required argument: {paramName}");
                                    var arg_{paramName} = {parseMethod}(value_{paramName});
                    """);
            }
        }

        return parsedArgs.ToString();
    }

    private static string BuildArgumentNames(ImmutableArray<IParameterSymbol> parameters)
    {
        var argumentNames = new StringBuilder();

        for (int i = 0; i < parameters.Length; i++)
        {
            var parameter = parameters[i];
            argumentNames.Append($"arg_{parameter.Name}");
            if (i < parameters.Length - 1)
            {
                argumentNames.Append(", ");
            }
        }

        return argumentNames.ToString();
    }

    private static string? GetParseMethod(ITypeSymbol typeSymbol, bool isNullable)
    {
        var typeName = typeSymbol.ToDisplayString();

        return typeName switch
        {
            "int" or "System.Int32" => isNullable ? "(string s) => String.IsNullOrWhiteSpace(s) ? (int?)null : Int32.Parse(s)" : "Int32.Parse",
            "float" or "System.Single" => isNullable ? "(string s) => String.IsNullOrWhiteSpace(s) ? (float?)null : Single.Parse(s)" : "Single.Parse",
            "double" or "System.Double" => isNullable ? "(string s) => String.IsNullOrWhiteSpace(s) ? (double?)null : Double.Parse(s)" : "Double.Parse",
            "decimal" or "System.Decimal" => isNullable ? "(string s) => String.IsNullOrWhiteSpace(s) ? (decimal?)null : Decimal.Parse(s)" : "Decimal.Parse",
            "bool" or "System.Boolean" => "Boolean.Parse",
            "System.DateTime" => isNullable ? "(string s) => String.IsNullOrWhiteSpace(s) ? (DateTime?)null : DateTime.Parse(s)" : "DateTime.Parse",
            "string" or "System.String" => String.Empty,
            _ => throw new NotSupportedException($"Type '{typeName}' is not supported."),
        };
    }

    private static string GetDefaultValueLiteral(IParameterSymbol parameter)
    {
        if (parameter.HasExplicitDefaultValue)
        {
            return parameter.ExplicitDefaultValue switch
            {
                null => "null",
                string s => $"\"{s}\"",
                bool b => b ? "true" : "false",
                _ => parameter.ExplicitDefaultValue.ToString() ?? "default"
            };
        }

        return "default";
    }

    private static void AppendFooter(StringBuilder builder)
    {
        builder.AppendLine("    }");
        builder.AppendLine("}");
    }
}