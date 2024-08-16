namespace TrailingCommaAnalyzerTest
{
    internal class Program
    {
        enum MissingAComma
        {
            EnumMember1,
            EnumMember2,
            [|EnumMember3|]
        };
    }
}
