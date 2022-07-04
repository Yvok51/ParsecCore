using System;
using Xunit;

using ParsecCore;
using ParsecCore.Input;
using ParsecCore.EitherNS;

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
            Parser<char> parser = Parsers.Char(toParse);
            IParserInput input = ParserInput.Create(inputString);

            IEither<ParseError, char> result = parser(input);

            Assert.True(result.HasRight);
            Assert.Equal(toParse, result.Right);
        }

        [Theory]
        [InlineData("abcd", 'a')]
        [InlineData("ghjk", 'g')]
        [InlineData("71025", '7')]
        public void ParseMltipleLetters(string inputString, char toParse)
        {
            Parser<char> parser = Parsers.Char(toParse);
            IParserInput input = ParserInput.Create(inputString);

            IEither<ParseError, char> result = parser(input);

            Assert.True(result.HasRight);
            Assert.Equal(toParse, result.Right);
        }

        [Theory]
        [InlineData("abcd", 'a')]
        [InlineData("ghjk", 'g')]
        [InlineData("71025", '7')]
        public void CorrectInputSizeAfterParse(string inputString, char toParse)
        {
            Parser<char> parser = Parsers.Char(toParse);
            IParserInput input = ParserInput.Create(inputString);

            IEither<ParseError, char> result = parser(input);

            Assert.True(result.HasRight);
            Assert.Equal(toParse, result.Right);

            Assert.False(input.EndOfInput);
            Assert.Equal(1, input.Position.Offset);
        }

        [Theory]
        [InlineData("abcd", 'n')]
        [InlineData("ghjk", 'y')]
        [InlineData("71025", 'c')]
        public void ParseCorrectlyFails(string inputString, char toParse)
        {
            Parser<char> parser = Parsers.Char(toParse);
            IParserInput input = ParserInput.Create(inputString);

            IEither<ParseError, char> result = parser(input);

            Assert.True(result.HasLeft);
        }
    }
}
