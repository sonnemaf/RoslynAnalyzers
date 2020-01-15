using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

namespace Analyzer1 {
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ProtectedConstructorInAbstractClassAnalyzer : DiagnosticAnalyzer {

        public const string DiagnosticId = "CA1012";

        private static readonly LocalizableString _title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString _messageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString _description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Common Practices and Code Improvements";

        private static readonly DiagnosticDescriptor _rule = new DiagnosticDescriptor(DiagnosticId, _title, _messageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: _description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(_rule); } }

        public override void Initialize(AnalysisContext context) {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.Method);
        }

        private static void AnalyzeSymbol(SymbolAnalysisContext context) {
            var methodSymbol = (IMethodSymbol)context.Symbol;

            // Find public and internal constructor in abstract class
            if (methodSymbol.MethodKind == MethodKind.Constructor &&
               (methodSymbol.DeclaredAccessibility == Accessibility.Public ||
                methodSymbol.DeclaredAccessibility == Accessibility.Internal) &&
               (methodSymbol.ContainingType?.IsAbstract ?? false)) {

                // For all such symbols, produce a diagnostic.
                var diagnostic = Diagnostic.Create(_rule, methodSymbol.Locations[0], methodSymbol.ContainingType.Name);

                context.ReportDiagnostic(diagnostic);
            }
        }
    }

}
