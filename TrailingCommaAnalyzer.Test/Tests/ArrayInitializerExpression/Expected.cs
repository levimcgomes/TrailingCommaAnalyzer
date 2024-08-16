namespace TrailingCommaAnalyzerTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var missingACommaStruct = new string[]
            {
                "This is a very long string",
                "This is a very long string",
                "This is a very long string",
            };
        }
    }
}
