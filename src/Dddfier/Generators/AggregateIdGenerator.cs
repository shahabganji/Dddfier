﻿using Dddfier.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Dddfier.Generators;

[Generator]
public sealed class AggregateIdGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var result = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => Prediction(node),
                transform: static (syntaxContext, token) => Transformer(syntaxContext, token))
            .Where(x => x is not null);

        context.RegisterSourceOutput(result, Execute!);

        context.RegisterPostInitializationOutput(GenerateWithIdAttribute);
    }

    private static void Execute(
        SourceProductionContext productionContext,
        WithIdOfAttributeContextModel model)
    {
        var generatedIdClassName = GenerateIdClass(productionContext, model);

        productionContext.AddSource($"{model.Namespace}.{model.ClassName}.g.cs",
            $$"""
            // <auto-generated />

            namespace {{model.Namespace}};

            public partial class {{model.ClassName}}
            {
                public {{generatedIdClassName}} {{model.PropertyName}} { get; private set; } = default!;
            }

            """);
    }

    private static string GenerateIdClass(SourceProductionContext productionContext,
        WithIdOfAttributeContextModel model)
    {
        var className = $"{model.ClassName}Id";

        var newMethod = model.TypeName switch

        {
            "Guid" => "Guid.NewGuid()",
            _ => $"default({model.TypeName})"
        };

        productionContext.AddSource($"{model.Namespace}.{className}.g.cs",
            $$"""
            // <auto-generated />

            #nullable enable

            using global::System;

            namespace {{model.Namespace}};

            public readonly partial struct {{className}} : IComparable<{{className}}>, IEquatable<{{className}}>
            {
                private readonly {{model.TypeName}} _value;

                private {{className}}({{model.TypeName}} value)
                {
                    _value = value;
                }

                public bool Equals({{className}} other) => this._value.Equals(other._value);
                public int CompareTo({{className}} other) => _value.CompareTo(other._value);

                public override bool Equals(object? obj)
                {
                    if (obj is null) return false;

                    return obj is {{className}} other && Equals(other);
                }

                public override int GetHashCode() => _value.GetHashCode();
                public override string ToString() => _value.ToString();

                public static {{className}} New() => new {{className}}({{newMethod}});

                public static bool operator ==({{className}} left, {{className}} right) => left.Equals(right);
                public static bool operator !=({{className}} left, {{className}} right) => !(left == right);

                public static implicit operator {{model.TypeName}}({{className}} id) => id._value;
                public static explicit operator {{className}}({{model.TypeName}} id) => new(id);
            }

            """);

        return className;
    }

    private static WithIdOfAttributeContextModel? Transformer(
        GeneratorSyntaxContext syntaxContext,
        CancellationToken token)
    {
        var syntaxNode = syntaxContext.Node;

        if (syntaxContext.SemanticModel.GetDeclaredSymbol(syntaxNode, cancellationToken: token)
            is not INamedTypeSymbol classSymbol)
            return null;

        var withIdOfAttributeClassSymbol = syntaxContext.SemanticModel.Compilation.GetTypeByMetadataName(
            "Dddfier.GeneratedCode.Attributes.WithIdOfAttribute`1");
        if (withIdOfAttributeClassSymbol is null)
            return null;

        var attibutes = classSymbol.GetAttributes().SingleOrDefault();

        var withIdAttributeUsageSymbol = classSymbol.GetAttributes().SingleOrDefault(attr =>
            withIdOfAttributeClassSymbol.Equals(attr.AttributeClass?.OriginalDefinition,
                SymbolEqualityComparer.Default));

        if (withIdAttributeUsageSymbol is null)
            return null;

        var containingNamespace = classSymbol.ContainingNamespace.ToDisplayString();
        var className = classSymbol.Name;
        var propertyName = withIdAttributeUsageSymbol.NamedArguments.SingleOrDefault(x => x.Key == "PropertyName").Value.Value?
            .ToString() ?? "Id";

        var propertyType = withIdAttributeUsageSymbol.AttributeClass!.TypeArguments.Single().OriginalDefinition
            .ToDisplayString();

        return new WithIdOfAttributeContextModel(
            containingNamespace, className, propertyName, propertyType);
    }

    private static bool Prediction(SyntaxNode node) =>
        node is ClassDeclarationSyntax { AttributeLists: { } attributeLists }
        && attributeLists.Any();

    private static void GenerateWithIdAttribute(IncrementalGeneratorPostInitializationContext initializationContext)
    {
        initializationContext.AddSource("WithIdOfAttribute.g.cs",
            """
            // <auto-generated />
            
            namespace Dddfier.GeneratedCode.Attributes;
            
            public sealed class WithIdOfAttribute<T> : global::System.Attribute
            {
                public string PropertyName { get; set; } = "Id";
                public global::Dddfier.GeneratedCode.SetterModifier SetterModifier { get; set; } = global::Dddfier.GeneratedCode.SetterModifier.Private;
            }
            """);

        initializationContext.AddSource("SetterModifierEnum.g.cs",
            """
            // <auto-generated />
            
            namespace Dddfier.GeneratedCode;
            
            public enum SetterModifier
            {
                Public,
                Private,
                Protected,
            }
            """);
    }
}
