using ParsecCore.EitherNS;

namespace ParsecCore.ParsersHelp
{
    /// <summary>
    /// Parser any character
    /// Fails if we are at the end of the input
    /// </summary>
    class AnyParser
    {
        public static Parser<char> Parser()
        {
            return SatisfyParser.Parser((c) => true, "any character");
        }
    }
}
