namespace TrailingCommaAnalyzerTest
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
        {
            var missingACommaStruct = new MissingACommaStruct { A = 10, B = 20 };
        }
    }
}
