namespace ParsecCore.Input
{
    public class ParserInput
    {
        public static IParserInput Create(string inputString)
        {
            return new StringParserInput(inputString);
        }
    }
}
