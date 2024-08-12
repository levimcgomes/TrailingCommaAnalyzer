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
        public const string DiagnosticId = "TrailingCommaAnalyzer";

        // You can change these strings in the Resources.resx file. If you do not want your
        // analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Localizing%20Analyzers.md
        // for more on localization
        private static readonly LocalizableString Title = new LocalizableResourceString(
            nameof(Resources.AnalyzerTitle),
            Resources.ResourceManager,
            typeof(Resources)
        );
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(
            nameof(Resources.AnalyzerMessageFormat),
            Resources.ResourceManager,
            typeof(Resources)
        );
        private static readonly LocalizableString Description = new LocalizableResourceString(
            nameof(Resources.AnalyzerDescription),
            Resources.ResourceManager,
            typeof(Resources)
        );
        private const string Category = "Naming";

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
                SyntaxKind.ObjectInitializerExpression
            );
        }

        private void AnalyzeObjectInitializerExpression(SyntaxNodeAnalysisContext context)
        {
            var objectInitializerSyntax = (InitializerExpressionSyntax)context.Node;
            var lastExpression = objectInitializerSyntax.Expressions.Last();
            // If the last item doesn't have trailing newline trivia,
            // then the initializer is a single-liner, which doesn't
            // need a trailing comma
            if (
                !lastExpression.HasTrailingTrivia
                || !lastExpression
                    .GetTrailingTrivia()
                    .Any(t => t.IsKind(SyntaxKind.EndOfLineTrivia))
            )
            {
                return;
            }

            // The last token is always the closing brace, so check
            // if the previous one is a comma or something else
            if (
                objectInitializerSyntax
                    .ChildTokens()
                    .Last()
                    .GetPreviousToken()
                    .IsKind(SyntaxKind.CommaToken)
            )
            {
                return;
            }

            // There should be a trailing comma, so create a diagnostic
            context.ReportDiagnostic(Diagnostic.Create(Rule, lastExpression.GetLocation()));
        }
    }
}
