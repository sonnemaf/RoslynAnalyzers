using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TestHelper;
using Analyzer1;

namespace Analyzer1.Test {
    [TestClass]
    public class ProtectedConstructorInAbstractClassUnitTest : CodeFixVerifier {

        //Diagnostic and CodeFix both triggered and checked for
        [TestMethod]
        public void TestPublic() {
            var test = @"
    using System;

    namespace ConsoleApplication1
    {
        abstract class Demo
        {
            public Demo()
            {
            }
        }
    }";
            var expected = new DiagnosticResult {
                Id = "CA1012",
                Message = $"The constructor in the abstract class '{"Demo"}' must be protected",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 8, 20)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
    using System;

    namespace ConsoleApplication1
    {
        abstract class Demo
        {
        protected Demo()
            {
            }
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        //Diagnostic and CodeFix both triggered and checked for
        [TestMethod]
        public void TestInternal() {
            var test = @"
    using System;

    namespace ConsoleApplication1
    {
        abstract class Demo
        {
            internal Demo()
            {
            }
        }
    }";
            var expected = new DiagnosticResult {
                Id = "CA1012",
                Message = $"The constructor in the abstract class '{"Demo"}' must be protected",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 8, 22)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
    using System;

    namespace ConsoleApplication1
    {
        abstract class Demo
        {
        protected Demo()
            {
            }
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider() {
            return new ProtectedConstructorInAbstractClassCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() {
            return new ProtectedConstructorInAbstractClassAnalyzer();
        }
    }
}
