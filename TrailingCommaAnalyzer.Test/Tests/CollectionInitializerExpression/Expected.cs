using System.Collections.Generic;

namespace TrailingCommaAnalyzerTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var missingACommaStruct = new List<string>
            {
                "This is a very long string",
                "This is a very long string",
                "This is a very long string",
            };
        }
    }
}
