using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;

using ParsecCore;
using ParsecCore.Input;
using ParsecCore.EitherNS;

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
            IParser<string> parser = Parser.String(toParse);
            IParserInput input = ParserInput.Create(inputString);

            IEither<ParseError, string> result = parser.Parse(input);

            Assert.True(result.HasRight);
            Assert.Equal(toParse, result.Right);
        }

        [Theory]
        [InlineData("abcd", "ab")]
        [InlineData("ghjk", "ghjk")]
        [InlineData("71025", "710")]
        public void ParseMltipleLetters(string inputString, string toParse)
        {
            IParser<string> parser = Parser.String(toParse);
            IParserInput input = ParserInput.Create(inputString);

            IEither<ParseError, string> result = parser.Parse(input);

            Assert.True(result.HasRight);
            Assert.Equal(toParse, result.Right);
        }

        [Theory]
        [InlineData("abcd", "ab")]
        [InlineData("ghjk", "g")]
        [InlineData("71025", "710")]
        public void CorrectInputSizeAfterParse(string inputString, string toParse)
        {
            IParser<string> parser = Parser.String(toParse);
            IParserInput input = ParserInput.Create(inputString);

            IEither<ParseError, string> result = parser.Parse(input);

            Assert.True(result.HasRight);
            Assert.Equal(toParse, result.Right);

            Assert.False(input.EndOfInput);
            Assert.Equal(toParse.Length, input.Position.Offset);
        }

        [Theory]
        [InlineData("abcd", "abcd")]
        [InlineData("ghjk\nasf", "ghjk\nasf")]
        [InlineData("71025", "71025")]
        public void InputCorrectlySpentAfterParse(string inputString, string toParse)
        {
            IParser<string> parser = Parser.String(toParse);
            IParserInput input = ParserInput.Create(inputString);

            IEither<ParseError, string> result = parser.Parse(input);

            Assert.True(result.HasRight);
            Assert.Equal(toParse, result.Right);

            Assert.True(input.EndOfInput);
        }

        [Theory]
        [InlineData("abcd", "hello")]
        [InlineData("ghjk\nasf", "there")]
        [InlineData("71025", "general")]
        public void ParseCorrectlyFailed(string inputString, string toParse)
        {
            IParser<string> parser = Parser.String(toParse);
            IParserInput input = ParserInput.Create(inputString);

            IEither<ParseError, string> result = parser.Parse(input);

            Assert.True(result.HasLeft);
        }

        [Fact]
        public void ParsingFailureLocationCorrect()
        {
            IParser<string> parser = Parser.String("abd");
            IParserInput input = ParserInput.Create("abcd");

            IEither<ParseError, string> result = parser.Parse(input);

            Assert.True(result.HasLeft);
            Assert.Equal(2, result.Left.Position.Offset);
        }
    }
}
