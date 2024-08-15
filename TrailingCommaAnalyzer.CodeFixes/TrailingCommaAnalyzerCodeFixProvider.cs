// Allow undocumented code
#pragma warning disable CS1591
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Formatting;

namespace TrailingCommaAnalyzer
{
    [
        ExportCodeFixProvider(
            LanguageNames.CSharp,
            Name = nameof(TrailingCommaAnalyzerCodeFixProvider)
        ),
        Shared
    ]
    public class TrailingCommaAnalyzerCodeFixProvider : CodeFixProvider
    {
        private static readonly string Title = "Add trailing comma";

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(TrailingCommaAnalyzer.DiagnosticId); }
        }

        // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/FixAllProvider.md
        // for more information on Fix All Providers
        public sealed override FixAllProvider GetFixAllProvider() =>
            WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context
                .Document.GetSyntaxRootAsync(context.CancellationToken)
                .ConfigureAwait(false);

            // TODO: Replace the following code with your own analysis, generating a CodeAction for each fix to suggest
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find the initializer expression identified by the diagnostic.
            var declaration = root.FindToken(diagnosticSpan.Start)
                .Parent.AncestorsAndSelf()
                .OfType<InitializerExpressionSyntax>()
                .First();

            // Register a code action that will invoke the fix.
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: Title,
                    createChangedDocument: c =>
                        AddTrailingCommaAsync(context.Document, declaration, c),
                    equivalenceKey: Title
                ),
                diagnostic
            );
        }

        private async Task<Document> AddTrailingCommaAsync(
            Document document,
            InitializerExpressionSyntax initializerExpression,
            CancellationToken cancellationToken
        )
        {
            // Get the list with separators
            var listWithSeparators = initializerExpression.Expressions.GetWithSeparators();
            // Get the last item
            var lastItem = listWithSeparators.Last();
            // Create a comma token with the correct trivia
            var commaToken = SyntaxFactory.Token(
                leading: default,
                SyntaxKind.CommaToken,
                trailing: lastItem.GetTrailingTrivia()
            );
            // Insert it into the list
            listWithSeparators = listWithSeparators.ReplaceRange(
                lastItem,
                ImmutableArray.Create(lastItem.WithTrailingTrivia(), commaToken)
            );
            // Recreate an initializer expression with the new list
            var newInitializerExpression = initializerExpression
                .WithExpressions(SyntaxFactory.SeparatedList<ExpressionSyntax>(listWithSeparators))
                .WithAdditionalAnnotations(Formatter.Annotation);
            // Replace it with a document editor
            var documentEditor = await DocumentEditor.CreateAsync(document, cancellationToken);
            documentEditor.ReplaceNode(initializerExpression, newInitializerExpression);
            // Return the document
            return documentEditor.GetChangedDocument();
        }
    }
}
