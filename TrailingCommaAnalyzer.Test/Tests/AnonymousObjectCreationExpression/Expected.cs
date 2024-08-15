namespace TrailingCommaAnalyzerTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var missingACommaStruct = new
            {
                A = 10,
                B = 20,
                C = 30,
            };
        }
    }
}
