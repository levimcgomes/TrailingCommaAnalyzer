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
            var a = new MissingACommaStruct
            {
                A = 10,
                B = 20,
                C = 30,
            };

            var missingACommaStruct = a with { A = 10, B = 20 };
        }
    }
}
