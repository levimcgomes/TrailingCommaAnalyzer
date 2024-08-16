// Allow undocumented code
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
        private enum TrailingCommaStyle
        {
            Never,
            Always,
            EndOfLine
        };

        public const string DiagnosticId = "TCA001";

        // You can change these strings in the Resources.resx file. If you do not want your
        // analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Localizing%20Analyzers.md
        // for more on localization
        private static readonly string Title = "Missing trailing comma";
        private static readonly string MessageFormat = "Missing trailing comma";
        private static readonly string Description = "Trailing commas should always be added.";
        private static readonly string StyleOptionName = "trailing_comma_style";
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
                SyntaxKind.WithInitializerExpression,
                // node is AnonymousObjectCreationExpressionSyntax
                SyntaxKind.AnonymousObjectCreationExpression,
                // node is CollectionExpressionSyntax
                SyntaxKind.CollectionExpression,
                // node is EnumDeclarationSyntax
                SyntaxKind.EnumDeclaration,
                // node is SwitchExpressionSyntax
                SyntaxKind.SwitchExpression,
                // node is PropertyPatternClauseSyntax
                SyntaxKind.PropertyPatternClause,
                // node is ListPatternSyntax
                SyntaxKind.ListPattern
            );
        }

        private void AnalyzeObjectInitializerExpression(SyntaxNodeAnalysisContext context)
        {
            TrailingCommaStyle trailingCommaStyle = GetConfig(context);

            if (trailingCommaStyle == TrailingCommaStyle.Never)
                return;

            var separated = GetSeparated(context.Node);
            if (separated.Count < 1)
                return;
            var lastItem = separated.Last();

            // Exit early if there already is a trailing comma
            if (lastItem.IsToken)
                return;

            var lastNode = lastItem.AsNode();
            // If trailing commas are only allowed at the endo fo lines,
            // check if an added comma would be the last token on its line
            if (
                trailingCommaStyle == TrailingCommaStyle.EndOfLine
                && !CommaWouldBeLastToken(lastNode)
            )
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Rule, lastNode.GetLocation()));
        }

        private TrailingCommaStyle GetConfig(SyntaxNodeAnalysisContext context)
        {
            var config = context.Options.AnalyzerConfigOptionsProvider.GetOptions(
                context.FilterTree
            );

            config.TryGetValue(
                $"dotnet_diagnostic.{DiagnosticId}.{StyleOptionName}",
                out var styleOption
            );

            return string.IsNullOrEmpty(styleOption)
                // Use EndOfLine as the default
                ? TrailingCommaStyle.EndOfLine
                : styleOption.ToLowerInvariant() switch
                {
                    "always" => TrailingCommaStyle.Always,
                    "never" => TrailingCommaStyle.Never,
                    "end_of_line" => TrailingCommaStyle.EndOfLine,
                    // Unknown values return the default
                    _ => TrailingCommaStyle.EndOfLine,
                };
        }

        public static SyntaxNodeOrTokenList GetSeparated(SyntaxNode node)
        {
            return node switch
            {
                InitializerExpressionSyntax initializerExpression
                    => initializerExpression.Expressions.GetWithSeparators(),
                AnonymousObjectCreationExpressionSyntax anonymousObjectCreationExpression
                    => anonymousObjectCreationExpression.Initializers.GetWithSeparators(),
                CollectionExpressionSyntax collectionExpression
                    => collectionExpression.Elements.GetWithSeparators(),
                EnumDeclarationSyntax enumDeclaration
                    => enumDeclaration.Members.GetWithSeparators(),
                SwitchExpressionSyntax switchExpression
                    => switchExpression.Arms.GetWithSeparators(),
                PropertyPatternClauseSyntax propertyPatternClause
                    => propertyPatternClause.Subpatterns.GetWithSeparators(),
                ListPatternSyntax listPattern => listPattern.Patterns.GetWithSeparators(),
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
