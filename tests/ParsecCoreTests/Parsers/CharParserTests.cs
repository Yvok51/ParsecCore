using ParsecCore;
using Xunit;

namespace ParsecCoreTests
{

    public enum Tokens
    {
        A,
        B,
        C,
    }

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

            var result = parser(input);

            Assert.True(result.IsResult);
            Assert.Equal(toParse, result.Result);
        }

        [Fact]
        public void ParseSingleToken()
        {
            var parser = Parsers.Char(Tokens.A);
            var input = ParserInput.Create(
                new Tokens[] { Tokens.A },
                (t, pos) => pos.WithIncreasedColumn().WithIncreasedOffset()
            );

            var result = parser(input);

            Assert.True(result.IsResult);
            Assert.Equal(Tokens.A, result.Result);
        }

        [Theory]
        [InlineData("abcd", 'a')]
        [InlineData("ghjk", 'g')]
        [InlineData("71025", '7')]
        public void ParseWithMultipleLetters(string inputString, char toParse)
        {
            var parser = Parsers.Char(toParse);
            var input = ParserInput.Create(inputString);

            var result = parser(input);

            Assert.True(result.IsResult);
            Assert.Equal(toParse, result.Result);
        }

        [Fact]
        public void ParseWithMultipleToken()
        {
            var parser = Parsers.Char(Tokens.A);
            var input = ParserInput.Create(
                new Tokens[] { Tokens.A, Tokens.B, Tokens.C },
                (t, pos) => pos.WithIncreasedColumn().WithIncreasedOffset()
            );

            var result = parser(input);

            Assert.True(result.IsResult);
            Assert.Equal(Tokens.A, result.Result);
        }


        [Theory]
        [InlineData("abcd", 'a')]
        [InlineData("ghjk", 'g')]
        [InlineData("71025", '7')]
        public void CorrectInputSizeAfterParse(string inputString, char toParse)
        {
            var parser = Parsers.Char(toParse);
            var input = ParserInput.Create(inputString);

            var result = parser(input);

            Assert.True(result.IsResult);
            Assert.Equal(toParse, result.Result);

            Assert.False(input.EndOfInput);
            Assert.Equal(1, result.UnconsumedInput.Position.Offset);
        }

        [Fact]
        public void CorrectTokenInputSizeAfterParse()
        {
            var parser = Parsers.Char(Tokens.B);
            var input = ParserInput.Create(
                new Tokens[] { Tokens.B, Tokens.B, Tokens.C },
                (t, pos) => pos.WithIncreasedColumn().WithIncreasedOffset()
            );

            var result = parser(input);

            Assert.True(result.IsResult);
            Assert.Equal(Tokens.B, result.Result);

            Assert.False(input.EndOfInput);
            Assert.Equal(1, result.UnconsumedInput.Position.Offset);
        }

        [Theory]
        [InlineData("abcd", 'n')]
        [InlineData("ghjk", 'y')]
        [InlineData("71025", 'c')]
        public void ParseCorrectlyFails(string inputString, char toParse)
        {
            var parser = Parsers.Char(toParse);
            var input = ParserInput.Create(inputString);

            var result = parser(input);

            Assert.True(result.IsError);
        }

        [Fact]
        public void ParseTokensCorrectlyFails()
        {
            var parser = Parsers.Char(Tokens.B);
            var input = ParserInput.Create(
                new Tokens[] { Tokens.A, Tokens.B, Tokens.C },
                (t, pos) => pos.WithIncreasedColumn().WithIncreasedOffset()
            );

            var result = parser(input);

            Assert.True(result.IsError);
        }
    }
}
