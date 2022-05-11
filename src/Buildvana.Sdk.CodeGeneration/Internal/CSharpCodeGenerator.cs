// ---------------------------------------------------------------------------------------
// Copyright (C) Riccardo De Agostini and contributors. All rights reserved.
// Licensed under the MIT license.
// See the LICENSE file in the project root for full license information.
//
// Part of this file may be third-party code, distributed under a compatible license.
// See the THIRD-PARTY-NOTICES file in the project root for third-party copyright notices.
// ---------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Buildvana.Sdk.CodeGeneration.Configuration;
using Buildvana.Sdk.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Buildvana.Sdk.CodeGeneration.Internal;

internal sealed class CSharpCodeGenerator : CodeGenerator<CSharpSyntaxNode>
{
    protected override SyntaxTrivia GenerateLineComment(string comment)
        => Comment("// " + comment);

    protected override CSharpSyntaxNode GenerateAssemblyAttribute(AssemblyAttributeFragment assemblyAttribute)
        => BuildAttribute(
            SyntaxKind.AssemblyKeyword,
            assemblyAttribute.Type,
            assemblyAttribute.OrderedParameters.Select(p => p.Value),
            assemblyAttribute.NamedParameters.Select(p => (p.Name, p.Value)));

    protected override CSharpSyntaxNode GenerateThisAssemblyClass(ThisAssemblyClassFragment thisAssemblyClass)
    {
        var classDeclaration = ClassDeclaration(thisAssemblyClass.Name)
            .AddModifiers(
                Token(SyntaxKind.InternalKeyword),
                Token(SyntaxKind.StaticKeyword),
                Token(SyntaxKind.PartialKeyword))
            .AddAttributeLists(
                BuildAttribute("System.Runtime.CompilerServices.CompilerGenerated", null, null),
                BuildAttribute("System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage", null, null))
            .AddMembers(thisAssemblyClass.Constants.Select(PublicConstField).Cast<MemberDeclarationSyntax>().ToArray());

        return StringUtility.IsNullOrEmpty(thisAssemblyClass.Namespace)
            ? classDeclaration
            : NamespaceDeclaration(ParseName(thisAssemblyClass.Namespace)).AddMembers(classDeclaration);

        static FieldDeclarationSyntax PublicConstField(Constant constant)
            => FieldDeclaration(
                    VariableDeclaration(TypeFromObject(constant.Value))
                        .WithVariables(SingletonSeparatedList(
                            VariableDeclarator(constant.Name)
                                .WithInitializer(EqualsValueClause(LiteralExpressionFromObject(constant.Value))))))
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.ConstKeyword)));
    }

    protected override CSharpSyntaxNode GenerateCompilationUnit(IEnumerable<CSharpSyntaxNode> syntaxNodes)
    {
        var nodes = syntaxNodes.ToArray();
        return CompilationUnit()
            .AddAttributeLists(nodes.OfType<AttributeListSyntax>().ToArray())
            .AddMembers(nodes.OfType<MemberDeclarationSyntax>().ToArray());
    }

    private static AttributeListSyntax BuildAttribute(
        SyntaxKind targetSpecifier,
        string type,
        IEnumerable<object?>? orderedParameters,
        IEnumerable<(string Name, object? Value)>? namedParameters)
    {
        var attribute = Attribute(ParseName(type));
        if (orderedParameters is not null || namedParameters is not null)
        {
            orderedParameters ??= Enumerable.Empty<object?>();
            namedParameters ??= Enumerable.Empty<(string Name, object? Value)>();
            var argumentSeparatedList = BuildSeparatedList(
                orderedParameters.Select(OrderedArgument)
                    .Concat(namedParameters.Select(NamedArgument)));

            attribute = attribute.WithArgumentList(AttributeArgumentList(argumentSeparatedList));
        }

        var attributeList = AttributeList(SingletonSeparatedList(attribute));
        if (targetSpecifier != SyntaxKind.None)
        {
            attributeList = attributeList.WithTarget(AttributeTargetSpecifier(Token(targetSpecifier)));
        }

        return attributeList;
    }

    private static AttributeListSyntax BuildAttribute(string type, IEnumerable<object?>? orderedParameters, IEnumerable<(string Name, object? Value)>? namedParameters)
        => BuildAttribute(SyntaxKind.None, type, orderedParameters, namedParameters);

    private static AttributeArgumentSyntax OrderedArgument(object? value)
        => AttributeArgument(LiteralExpressionFromObject(value));

    private static AttributeArgumentSyntax NamedArgument((string Name, object? Value) nameAndValue)
        => AttributeArgument(NameEquals(IdentifierName(nameAndValue.Name)), null, LiteralExpressionFromObject(nameAndValue.Value));

    // NOTE: All supported types must be listed
    //       (check CodeGeneratorConfigurationUtility.AllowedConstantTypes)
    private static LiteralExpressionSyntax LiteralExpressionFromObject(object? obj)
        => obj switch {
            null => LiteralExpression(SyntaxKind.NullLiteralExpression),
            byte byteValue => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(byteValue)),
            short shortValue => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(shortValue)),
            int intValue => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(intValue)),
            long longValue => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(longValue)),
            bool boolValue => LiteralExpression(boolValue ? SyntaxKind.TrueLiteralExpression : SyntaxKind.FalseLiteralExpression),
            string stringValue => LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(stringValue)),
            _ => throw new ArgumentException("Unsupported type for literal expression.", nameof(obj)),
        };

    // NOTE: All supported types must be listed
    //       (check CodeGeneratorConfigurationUtility.AllowedConstantTypes)
    private static TypeSyntax TypeFromObject(object? obj)
        => obj switch {
            byte => PredefinedType(Token(SyntaxKind.ByteKeyword)),
            short => PredefinedType(Token(SyntaxKind.ShortKeyword)),
            int => PredefinedType(Token(SyntaxKind.IntKeyword)),
            long => PredefinedType(Token(SyntaxKind.LongKeyword)),
            bool => PredefinedType(Token(SyntaxKind.BoolKeyword)),
            string => PredefinedType(Token(SyntaxKind.StringKeyword)),
            null => throw new ArgumentNullException(nameof(obj)),
            _ => throw new ArgumentException("Unsupported type for literal expression.", nameof(obj)),
        };

    // Build a SeparatedSyntaxList, separating nodes (e.g. arguments) with separators (commas by default).
    private static SeparatedSyntaxList<T> BuildSeparatedList<T>(IEnumerable<T> nodes, SyntaxKind separator = SyntaxKind.CommaToken)
        where T : CSharpSyntaxNode
    {
        var nodesList = nodes.ToList();
        return SeparatedList(
            nodesList,
            Enumerable.Repeat(Token(separator), Math.Max(nodesList.Count - 1, 0)));
    }
}
