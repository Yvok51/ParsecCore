using ParsecCore;
using ParsecCore.EitherNS;
using ParsecCore.Input;
using Xunit;

namespace ParsecCoreTests
{
    public class CharParserTests
    {
        [Theory]
        [InlineData("a", 'a')]
        [InlineData("g", 'g')]
        [InlineData("7", '7')]
        public void ParseSingleLetter(string inputString, char toParse)
        {
            var parser = Parsers.Char(toParse);
            var input = ParserInput.Create(inputString);

            IEither<ParseError, char> result = parser(input);

            Assert.True(result.IsResult);
            Assert.Equal(toParse, result.Result);
        }

        [Theory]
        [InlineData("abcd", 'a')]
        [InlineData("ghjk", 'g')]
        [InlineData("71025", '7')]
        public void ParseMltipleLetters(string inputString, char toParse)
        {
            var parser = Parsers.Char(toParse);
            var input = ParserInput.Create(inputString);

            IEither<ParseError, char> result = parser(input);

            Assert.True(result.IsResult);
            Assert.Equal(toParse, result.Result);
        }

        [Theory]
        [InlineData("abcd", 'a')]
        [InlineData("ghjk", 'g')]
        [InlineData("71025", '7')]
        public void CorrectInputSizeAfterParse(string inputString, char toParse)
        {
            var parser = Parsers.Char(toParse);
            var input = ParserInput.Create(inputString);

            IEither<ParseError, char> result = parser(input);

            Assert.True(result.IsResult);
            Assert.Equal(toParse, result.Result);

            Assert.False(input.EndOfInput);
            Assert.Equal(1, input.Position.Offset);
        }

        [Theory]
        [InlineData("abcd", 'n')]
        [InlineData("ghjk", 'y')]
        [InlineData("71025", 'c')]
        public void ParseCorrectlyFails(string inputString, char toParse)
        {
            var parser = Parsers.Char(toParse);
            var input = ParserInput.Create(inputString);

            IEither<ParseError, char> result = parser(input);

            Assert.True(result.IsError);
        }
    }
}
