namespace TrailingCommaAnalyzerTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var missingACommaStruct = 1 switch
            {
                1 => 10,
                2 => 20,
                3 => 30,
            };
        }
    }
}
