
using ParsecCore;
using ParsecCore.EitherNS;
using ParsecCore.Input;
using Xunit;

namespace ParsecCoreTests
{
    public class StringParserTests
    {
        [Theory]
        [InlineData("a", "a")]
        [InlineData("g", "g")]
        [InlineData("7", "7")]
        public void ParseSingleLetter(string inputString, string toParse)
        {
            Parser<string> parser = Parsers.String(toParse);
            var input = ParserInput.Create(inputString);

            IEither<ParseError, string> result = parser(input);

            Assert.True(result.IsResult);
            Assert.Equal(toParse, result.Result);
        }

        [Theory]
        [InlineData("abcd", "ab")]
        [InlineData("ghjk", "ghjk")]
        [InlineData("71025", "710")]
        public void ParseMltipleLetters(string inputString, string toParse)
        {
            Parser<string> parser = Parsers.String(toParse);
            var input = ParserInput.Create(inputString);

            IEither<ParseError, string> result = parser(input);

            Assert.True(result.IsResult);
            Assert.Equal(toParse, result.Result);
        }

        [Theory]
        [InlineData("abcd", "ab")]
        [InlineData("ghjk", "g")]
        [InlineData("71025", "710")]
        public void CorrectInputSizeAfterParse(string inputString, string toParse)
        {
            Parser<string> parser = Parsers.String(toParse);
            var input = ParserInput.Create(inputString);

            IEither<ParseError, string> result = parser(input);

            Assert.True(result.IsResult);
            Assert.Equal(toParse, result.Result);

            Assert.False(input.EndOfInput);
            Assert.Equal(toParse.Length, input.Position.Offset);
        }

        [Theory]
        [InlineData("abcd", "abcd")]
        [InlineData("ghjk\nasf", "ghjk\nasf")]
        [InlineData("71025", "71025")]
        public void InputCorrectlySpentAfterParse(string inputString, string toParse)
        {
            Parser<string> parser = Parsers.String(toParse);
            var input = ParserInput.Create(inputString);

            IEither<ParseError, string> result = parser(input);

            Assert.True(result.IsResult);
            Assert.Equal(toParse, result.Result);

            Assert.True(input.EndOfInput);
        }

        [Theory]
        [InlineData("abcd", "hello")]
        [InlineData("ghjk\nasf", "there")]
        [InlineData("71025", "general")]
        public void ParseCorrectlyFailed(string inputString, string toParse)
        {
            Parser<string> parser = Parsers.String(toParse);
            var input = ParserInput.Create(inputString);

            IEither<ParseError, string> result = parser(input);

            Assert.True(result.IsError);
        }

        [Fact]
        public void ParsingFailureLocationCorrect()
        {
            Parser<string> parser = Parsers.String("abd");
            var input = ParserInput.Create("abcd");

            IEither<ParseError, string> result = parser(input);

            Assert.True(result.IsError);
            Assert.Equal(2, result.Error.Position.Offset);
        }
    }
}
