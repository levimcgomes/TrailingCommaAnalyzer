﻿// Allow undocumented code
#pragma warning disable CS1591
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace TrailingCommaAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class TrailingCommaAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "TCA001";

        // You can change these strings in the Resources.resx file. If you do not want your
        // analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Localizing%20Analyzers.md
        // for more on localization
        private static readonly string Title = "Missing trailing comma.";
        private static readonly string MessageFormat = "Missing trailing comma.";
        private static readonly string Description = "Trailing commas should always be added.";
        private const string Category = "Maintainability";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: Description
        );

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(Rule); }
        }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(
                AnalyzeObjectInitializerExpression,
                // node is InitializerExpressionSyntax
                SyntaxKind.ObjectInitializerExpression,
                SyntaxKind.ArrayInitializerExpression,
                // node is AnonymousObjectCreationExpressionSyntax
                SyntaxKind.AnonymousObjectCreationExpression
            );
        }

        private void AnalyzeObjectInitializerExpression(SyntaxNodeAnalysisContext context)
        {
            var separated = GetSeparated(context.Node);
            if (separated.Count < 1)
                return;
            var lastItem = separated.Last();

            // Exit early if there already is a trailing comma
            if (lastItem.IsToken)
                return;

            var lastNode = lastItem.AsNode();
            // Check if an added comma would be the last token on its line
            if (!CommaWouldBeLastToken(lastNode))
                return;

            context.ReportDiagnostic(Diagnostic.Create(Rule, lastNode.GetLocation()));
        }

        public static SyntaxNodeOrTokenList GetSeparated(SyntaxNode node)
        {
            return node switch
            {
                InitializerExpressionSyntax initializerExpression
                    => initializerExpression.Expressions.GetWithSeparators(),
                AnonymousObjectCreationExpressionSyntax anonymousObjectCreationExpression
                    => anonymousObjectCreationExpression.Initializers.GetWithSeparators(),
                _
                    => throw new NotSupportedException(
                        $"Unable to get list with separators for syntax kind {node.Kind()}"
                    ),
            };
        }

        private bool CommaWouldBeLastToken(SyntaxNode lastNode)
        {
            var lines = lastNode.SyntaxTree.GetText().Lines;
            // Get the last token on this node
            var lastTokenInNode = lastNode.DescendantTokens().Last();
            // Get the next token
            var nextToken = lastTokenInNode.GetNextToken();
            // Check if it actually exists
            if (nextToken == default)
            {
                return true;
            }

            // Get the lines of both tokens
            var lineOfLastTokenInNode = lines
                .GetLineFromPosition(lastTokenInNode.Span.End)
                .LineNumber;
            var lineOfNextToken = lines.GetLineFromPosition(nextToken.Span.Start).LineNumber;
            // If the last token and the next token are on different lines,
            // then an added comma would be the last token on its line
            return lineOfLastTokenInNode != lineOfNextToken;
        }
    }
}
