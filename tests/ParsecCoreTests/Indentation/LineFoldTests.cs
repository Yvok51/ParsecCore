using ParsecCore;
using System.Collections.Generic;
using Xunit;

namespace ParsecCoreTests.Indentation
{
    public class LineFoldTests
    {
        public LineFoldTests()
        {
            var nonEOLSpaces = Parsers.Satisfy(c => c == ' ' || c == '\t', "whitespace").Many();
            var Identifier = Parsers.Letter.Many1();
            var keyP =
                from key in Identifier
                from _ in Parsers.Char(':').Then(nonEOLSpaces)
                select key;

            HeaderParser = ParsecCore.Indentation.Indentation.LineFold(
                Parsers.Spaces,
                sc =>
                {
                    var valuesP = Parsers.SepBy1(Identifier, sc).FollowedBy(Parsers.Spaces);
                    return from key in keyP
                           from values in valuesP
                           select (key, values);
                }
            );
        }

        public Parser<(string key, IReadOnlyList<string> values), char> HeaderParser { get; set; }

        [Fact]
        public void LineFoldParsesInline()
        {
            var input = ParserInput.Create("key: valueA valueB valueC");
            var result = HeaderParser(input);

            Assert.True(result.IsResult);
            Assert.Equal("key", result.Result.key);
            Assert.Equal(3, result.Result.key.Length);
        }

        [Theory]
        [InlineData("key: valueA valueB\n valueC")]
        [InlineData("key: valueA valueB   \n valueC")]
        [InlineData("key: valueA valueB\n    valueC")]
        public void LineFoldParsesAcrossLines(string inputStr)
        {
            var input = ParserInput.Create(inputStr);
            var result = HeaderParser(input);

            Assert.True(result.IsResult);
            Assert.Equal("key", result.Result.key);
            Assert.Equal(3, result.Result.key.Length);
        }

        [Fact]
        public void LineFoldStops()
        {
            var input = ParserInput.Create("key: valueA valueB valueC\n\nkeyB: value");
            var result = HeaderParser(input);

            Assert.True(result.IsResult);
            Assert.Equal("key", result.Result.key);
            Assert.Equal(3, result.Result.key.Length);
            Assert.Equal(new Position(3, 1), result.UnconsumedInput.Position);
        }

        [Fact]
        public void LineFoldStopsAfterLineFolding()
        {
            var input = ParserInput.Create("key: valueA valueB\n  valueC\n\nkeyB: value");
            var result = HeaderParser(input);

            Assert.True(result.IsResult);
            Assert.Equal("key", result.Result.key);
            Assert.Equal(3, result.Result.key.Length);
            Assert.Equal(new Position(4, 1), result.UnconsumedInput.Position);
        }
    }
}
