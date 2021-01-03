// -----------------------------------------------------------------------------------
// Copyright (C) Riccardo De Agostini and Buildvana contributors. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
//
// Part of this file may be third-party code, distributed under a compatible license.
// See THIRD-PARTY-NOTICES file in the project root for third-party copyright notices.
// -----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Buildvana.Sdk.CodeGeneration.Configuration;
using Buildvana.Sdk.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using static Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory;

namespace Buildvana.Sdk.CodeGeneration.Internal
{
    internal sealed class VisualBasicCodeGenerator : CodeGenerator<VisualBasicSyntaxNode>
    {
        protected override SyntaxTrivia GenerateLineComment(string comment)
            => CommentTrivia("' " + comment);

        protected override VisualBasicSyntaxNode GenerateAssemblyAttribute(AssemblyAttribute assemblyAttribute)
            => BuildAttribute(
                SyntaxKind.AssemblyKeyword,
                assemblyAttribute.Type,
                assemblyAttribute.OrderedParameters.Select(p => p.Value),
                assemblyAttribute.NamedParameters.Select(p => (p.Name, p.Value)));

        protected override VisualBasicSyntaxNode GenerateThisAssemblyClass(ThisAssemblyClass thisAssemblyClass)
        {
            var moduleBlock = ModuleBlock(
                    ModuleStatement(thisAssemblyClass.Name)
                        .AddModifiers(
                            Token(SyntaxKind.FriendKeyword),
                            Token(SyntaxKind.PartialKeyword))
                        .AddAttributeLists(
                            BuildAttribute("System.Runtime.CompilerServices.CompilerGenerated", null, null),
                            BuildAttribute("System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage", null, null)))
                .AddMembers(thisAssemblyClass.Constants.Select(PublicConstField).Cast<StatementSyntax>().ToArray());

            return StringUtility.IsNullOrEmpty(thisAssemblyClass.Namespace)
                ? moduleBlock
                : NamespaceBlock(NamespaceStatement(ParseName(thisAssemblyClass.Namespace))).AddMembers(moduleBlock);

            static FieldDeclarationSyntax PublicConstField(Constant constant)
                => FieldDeclaration(
                        VariableDeclarator(ModifiedIdentifier(constant.Name))
                            .WithAsClause(SimpleAsClause(TypeFromObject(constant.Value)))
                            .WithInitializer(EqualsValue(LiteralExpressionFromObject(constant.Value))))
                    .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.ConstKeyword)));
        }

        protected override VisualBasicSyntaxNode GenerateCompilationUnit(IEnumerable<VisualBasicSyntaxNode> syntaxNodes)
        {
            var nodes = syntaxNodes.ToArray();
            return CompilationUnit()
                .AddAttributes(AttributesStatement(List(nodes.OfType<AttributeListSyntax>())))
                .AddMembers(nodes.OfType<StatementSyntax>().ToArray());
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

                attribute = attribute.WithArgumentList(ArgumentList(argumentSeparatedList));
            }

            if (targetSpecifier != SyntaxKind.None)
            {
                attribute = attribute.WithTarget(AttributeTarget(Token(targetSpecifier)));
            }

            var attributeList = AttributeList(SingletonSeparatedList(attribute));

            return attributeList;
        }

        private static AttributeListSyntax BuildAttribute(string type, IEnumerable<object?>? orderedParameters, IEnumerable<(string Name, object? Value)>? namedParameters)
            => BuildAttribute(SyntaxKind.None, type, orderedParameters, namedParameters);

        private static ArgumentSyntax OrderedArgument(object? value)
            => SimpleArgument(LiteralExpressionFromObject(value));

        private static ArgumentSyntax NamedArgument((string Name, object? Value) nameAndValue)
            => SimpleArgument(NameColonEquals(IdentifierName(nameAndValue.Name)), LiteralExpressionFromObject(nameAndValue.Value));

        // NOTE: All supported types must be listed
        //       (check CodeGeneratorConfigurationUtility.AllowedConstantTypes)
        private static LiteralExpressionSyntax LiteralExpressionFromObject(object? obj)
            => obj switch {
                null => NothingLiteralExpression(Token(SyntaxKind.NothingKeyword)),
                byte byteValue => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(byteValue)),
                short shortValue => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(shortValue)),
                int intValue => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(intValue)),
                long longValue => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(longValue)),
                bool boolValue => boolValue
                    ? TrueLiteralExpression(Token(SyntaxKind.TrueKeyword))
                    : FalseLiteralExpression(Token(SyntaxKind.FalseKeyword)),
                string stringValue => LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(stringValue)),
                _ => throw new ArgumentException("Unsupported type for literal expression.", nameof(obj)),
            };

        // NOTE: All supported types must be listed
        //       (check CodeGeneratorConfigurationUtility.AllowedConstantTypes)
        private static TypeSyntax TypeFromObject(object? obj)
            => obj switch {
                byte => PredefinedType(Token(SyntaxKind.ByteKeyword)),
                short => PredefinedType(Token(SyntaxKind.ShortKeyword)),
                int => PredefinedType(Token(SyntaxKind.IntegerKeyword)),
                long => PredefinedType(Token(SyntaxKind.LongKeyword)),
                bool => PredefinedType(Token(SyntaxKind.BooleanKeyword)),
                string => PredefinedType(Token(SyntaxKind.StringKeyword)),
                null => throw new ArgumentNullException(nameof(obj)),
                _ => throw new ArgumentException("Unsupported type for literal expression.", nameof(obj)),
            };

        // Build a SeparatedSyntaxList, separating nodes (e.g. arguments) with separators (commas by default).
        private static SeparatedSyntaxList<T> BuildSeparatedList<T>(IEnumerable<T> nodes, SyntaxKind separator = SyntaxKind.CommaToken)
            where T : VisualBasicSyntaxNode
        {
            var nodesList = nodes.ToList();
            return SeparatedList(
                nodesList,
                Enumerable.Repeat(Token(separator), Math.Max(nodesList.Count - 1, 0)));
        }
    }
}