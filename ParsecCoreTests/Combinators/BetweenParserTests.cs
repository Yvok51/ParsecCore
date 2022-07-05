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
            Parser<char> leftParser = Parsers.Char('[');
            Parser<char> rightParser = Parsers.Char('}');
            Parser<string> middleParser = Parsers.String("Hello there");
            Parser<string> parser = Combinators.Between(leftParser, middleParser, rightParser);
            IParserInput input = ParserInput.Create("[Hello there}");

            var result = parser(input);

            Assert.True(result.HasRight);
            Assert.Equal("Hello there", result.Right);
        }

        [Fact]
        public void LeftParserFails()
        {
            Parser<char> leftParser = Parsers.Char('[');
            Parser<char> rightParser = Parsers.Char('}');
            Parser<string> middleParser = Parsers.String("Hello there");
            Parser<string> parser = Combinators.Between(leftParser, middleParser, rightParser);
            IParserInput input = ParserInput.Create("Hello there}");

            var result = parser(input);

            Assert.True(result.HasLeft);
        }

        [Fact]
        public void MiddleParserFails()
        {
            Parser<char> leftParser = Parsers.Char('[');
            Parser<char> rightParser = Parsers.Char('}');
            Parser<string> middleParser = Parsers.String("Hello there");
            Parser<string> parser = Combinators.Between(leftParser, middleParser, rightParser);
            IParserInput input = ParserInput.Create("[Hello tfere}");

            var result = parser(input);

            Assert.True(result.HasLeft);
        }

        [Fact]
        public void RightParserFails()
        {
            Parser<char> leftParser = Parsers.Char('[');
            Parser<char> rightParser = Parsers.Char('}');
            Parser<string> middleParser = Parsers.String("Hello there");
            Parser<string> parser = Combinators.Between(leftParser, middleParser, rightParser);
            IParserInput input = ParserInput.Create("[Hello there]");

            var result = parser(input);

            Assert.True(result.HasLeft);
        }
    }
}
