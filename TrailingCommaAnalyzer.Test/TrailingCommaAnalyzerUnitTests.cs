using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.TestPlatform.Common.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static TrailingCommaAnalyzer.Test.TrailingCommaAnalyzerUnitTest;
using VerifyCS = TrailingCommaAnalyzer.Test.CSharpCodeFixVerifier<
    TrailingCommaAnalyzer.TrailingCommaAnalyzer,
    TrailingCommaAnalyzer.TrailingCommaAnalyzerCodeFixProvider
>;

namespace TrailingCommaAnalyzer.Test
{
    [TestClass]
    public class TrailingCommaAnalyzerUnitTest
    {
        public struct TestFileSet
        {
            public string Name;
            public string WithDiagnostic;
            public string WithoutDiagnostic;
            public string Expected;
        }

        public static List<TestFileSet> _testFileSets;

        public TestContext TestContext { get; set; }

        public static IEnumerable<object[]> GetTestData()
        {
            foreach (TestFileSet testFileSet in _testFileSets)
            {
                yield return new object[] { testFileSet };
            }
        }

        [ClassInitialize]
        public static void GetTestFileSets(TestContext _)
        {
            _testFileSets = new List<TestFileSet>();
            var testsFolder = Directory.EnumerateDirectories(@"Tests");
            foreach (var testFiles in testsFolder)
            {
                TestFileSet testFileSet = new TestFileSet
                {
                    Name = testFiles,
                    WithDiagnostic = "",
                    WithoutDiagnostic = "",
                    Expected = "",
                };
                if (File.Exists(testFiles + @"/Diagnostic.cs"))
                {
                    testFileSet.WithDiagnostic = File.ReadAllText(testFiles + @"/Diagnostic.cs");
                }
                if (File.Exists(testFiles + @"/NoDiagnostic.cs"))
                {
                    testFileSet.WithoutDiagnostic = File.ReadAllText(
                        testFiles + @"/NoDiagnostic.cs"
                    );
                }
                if (File.Exists(testFiles + @"/Expected.cs"))
                {
                    testFileSet.Expected = File.ReadAllText(testFiles + @"/Expected.cs");
                }
                _testFileSets.Add(testFileSet);
            }
        }

        public static string GetDisplayName(MethodInfo methodInfo, object[] values)
        {
            if (values[0] is TestFileSet testFileSet)
            {
                return methodInfo.Name switch
                {
                    nameof(DiagnosticFromFile) => $"With diagnostic on set {testFileSet.Name}",
                    nameof(NoDiagnosticFromFile) => $"Without diagnostic on set {testFileSet.Name}",
                    nameof(FixFromFile) => $"Fix on set {testFileSet.Name}",
                    _ => "Unreachable",
                };
            }
            else
            {
                return "Invalid data";
            }
        }

        [TestMethod]
        [DynamicData(
            nameof(GetTestData),
            DynamicDataSourceType.Method,
            DynamicDataDisplayName = nameof(GetDisplayName)
        )]
        public async Task DiagnosticFromFile(TestFileSet testFileSet)
        {
            if (testFileSet.WithDiagnostic.Length == 0)
                throw new ArgumentException("Missing test data", nameof(testFileSet));
            await VerifyCS.VerifyAnalyzerAsync(testFileSet.WithDiagnostic);
        }

        [TestMethod]
        [DynamicData(
            nameof(GetTestData),
            DynamicDataSourceType.Method,
            DynamicDataDisplayName = nameof(GetDisplayName)
        )]
        public async Task NoDiagnosticFromFile(TestFileSet testFileSet)
        {
            if (testFileSet.WithoutDiagnostic.Length == 0)
                throw new ArgumentException("Missing test data", nameof(testFileSet));
            await VerifyCS.VerifyAnalyzerAsync(testFileSet.WithoutDiagnostic);
        }

        [TestMethod]
        [DynamicData(
            nameof(GetTestData),
            DynamicDataSourceType.Method,
            DynamicDataDisplayName = nameof(GetDisplayName)
        )]
        public async Task FixFromFile(TestFileSet testFileSet)
        {
            if (testFileSet.WithDiagnostic.Length == 0 || testFileSet.Expected.Length == 0)
                throw new ArgumentException("Missing test data", nameof(testFileSet));
            await VerifyCS.VerifyCodeFixAsync(testFileSet.WithDiagnostic, testFileSet.Expected);
        }
    }
}
