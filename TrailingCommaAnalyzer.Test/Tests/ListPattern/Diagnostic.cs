namespace TrailingCommaAnalyzerTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string[] a =
            [
                "This is a very long string",
                "This is a very long string",
                "This is a very long string",
            ];
            var missingAComma = a is [
                "Short",
                "Short",
                [|"Short"|]
            ];
        }
    }
}
