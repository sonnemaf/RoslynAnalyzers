using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Analyzer1 {
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ProtectedConstructorInAbstractClassCodeFixProvider)), Shared]
    public class ProtectedConstructorInAbstractClassCodeFixProvider : CodeFixProvider {

        private const string _title = "Make protected";

        public sealed override ImmutableArray<string> FixableDiagnosticIds {
            get { return ImmutableArray.Create(ProtectedConstructorInAbstractClassAnalyzer.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context) {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            // TODO: Replace the following code with your own analysis, generating a CodeAction for each fix to suggest
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find the type declaration identified by the diagnostic.
            var constructor = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ConstructorDeclarationSyntax>().First();

            // Register a code action that will invoke the fix.
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: _title,
                    createChangedDocument: c => MakeProtectedAsync(context.Document, constructor, c),
                    equivalenceKey: _title),
                diagnostic);
        }

        private async Task<Document> MakeProtectedAsync(Document document, ConstructorDeclarationSyntax constructor, CancellationToken cancellationToken) {

            var generator = SyntaxGenerator.GetGenerator(document);
            var newStatement = generator.WithAccessibility(constructor, Accessibility.Protected);

            // Replace old (public/internal ctor) with the new (protected ctor)
            var oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var newRoot = oldRoot.ReplaceNode(constructor, newStatement);

            return document.WithSyntaxRoot(newRoot);
        }

    }
}
