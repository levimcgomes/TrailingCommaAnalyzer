namespace TrailingCommaAnalyzerTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string[] missingACommaStruct =
            [
                "This is a very long string",
                "This is a very long string",
                [|"This is a very long string"|]
            ];
        }
    }
}
