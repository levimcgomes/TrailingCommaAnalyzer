using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyCS = TrailingCommaAnalyzer.Test.CSharpCodeFixVerifier<
    TrailingCommaAnalyzer.TrailingCommaAnalyzer,
    TrailingCommaAnalyzer.TrailingCommaAnalyzerCodeFixProvider
>;

namespace TrailingCommaAnalyzer.Test
{
    [TestClass]
    public class TrailingCommaAnalyzerUnitTest
    {
        private static readonly string TestPrelude =
            @"namespace TrailingCommaAnalyzerTest
{
    internal class Program
    {
        struct MissingACommaStruct
        {
            public int A;
            public int B;
            public int C;
        }

        static void Main(string[] args)
        {";
        private static readonly string TestClosing =
            @"        }
    }
}
";

        [TestMethod]
        public async Task DiagnosticObjectInitializerExpression()
        {
            var test =
                TestPrelude
                + @"var missingACommaStruct = new MissingACommaStruct
{
    A = 10,
    B = 20,
    C = 30
};"
                + TestClosing;
            var expected = VerifyCS.Diagnostic("TCA001").WithSpan(17, 5, 17, 11);
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task NoDiagnosticObjectInitializerExpression()
        {
            var test =
                TestPrelude
                + @"var notMissingACommaStruct = new MissingACommaStruct { A = 10, B = 20 };"
                + TestClosing;
            await VerifyCS.VerifyAnalyzerAsync(test);
        }
    }
}
