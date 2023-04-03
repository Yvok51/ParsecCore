using ParsecCore;
using ParsecCore.Input;
using Xunit;

namespace ParsecCoreTests
{
    public class BetweenParserTests
    {
        [Fact]
        public void MiddleParsed()
        {
            var leftParser = Parsers.Char('[');
            var rightParser = Parsers.Char('}');
            var middleParser = Parsers.String("Hello there");
            var parser = Parsers.Between(leftParser, middleParser, rightParser);
            var input = ParserInput.Create("[Hello there}");

            var result = parser(input);

            Assert.True(result.IsResult);
            Assert.Equal("Hello there", result.Result);
        }

        [Fact]
        public void LeftParserFails()
        {
            var leftParser = Parsers.Char('[');
            var rightParser = Parsers.Char('}');
            var middleParser = Parsers.String("Hello there");
            var parser = Parsers.Between(leftParser, middleParser, rightParser);
            var input = ParserInput.Create("Hello there}");

            var result = parser(input);

            Assert.True(result.IsError);
        }

        [Fact]
        public void MiddleParserFails()
        {
            var leftParser = Parsers.Char('[');
            var rightParser = Parsers.Char('}');
            var middleParser = Parsers.String("Hello there");
            var parser = Parsers.Between(leftParser, middleParser, rightParser);
            var input = ParserInput.Create("[Hello tfere}");

            var result = parser(input);

            Assert.True(result.IsError);
        }

        [Fact]
        public void RightParserFails()
        {
            var leftParser = Parsers.Char('[');
            var rightParser = Parsers.Char('}');
            var middleParser = Parsers.String("Hello there");
            var parser = Parsers.Between(leftParser, middleParser, rightParser);
            var input = ParserInput.Create("[Hello there]");

            var result = parser(input);

            Assert.True(result.IsError);
        }
    }
}
