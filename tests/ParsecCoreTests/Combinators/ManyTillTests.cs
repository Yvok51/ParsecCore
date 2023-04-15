using ParsecCore;
using System.Collections.Generic;
using Xunit;

namespace ParsecCoreTests
{
    public class ManyTillTests
    {
        [Fact]
        public void ImmediateTillParsed()
        {
            var input = ParserInput.Create("|");
            var valueParser = Parsers.Digit;
            var tillParser = Parsers.Char('|');
            var parser = Parsers.ManyTill(valueParser, tillParser);
            var initialPosition = input.Position;

            var result = parser(input);

            Assert.True(result.IsResult);
            Assert.Equal(0, result.Result.Count);
            Assert.NotEqual(initialPosition, result.UnconsumedInput.Position);
        }

        [Fact]
        public void SingleValueParsed()
        {
            var input = ParserInput.Create("1|");
            var valueParser = Parsers.Digit;
            var tillParser = Parsers.Char('|');
            var parser = Parsers.ManyTill(valueParser, tillParser);
            var initialPosition = input.Position;

            var expected = new List<char>();
            expected.Add('1');

            var result = parser(input);

            Assert.True(result.IsResult);
            Assert.Equal(expected.Count, result.Result.Count);
            Assert.NotEqual(initialPosition, result.UnconsumedInput.Position);

            for (int i = 0; i < expected.Count; i++)
            {
                Assert.Equal(expected[i], result.Result[i]);
            }
        }

        [Fact]
        public void MultipleValuesParsed()
        {
            var input = ParserInput.Create("123|");
            var valueParser = Parsers.Digit;
            var tillParser = Parsers.Char('|');
            var parser = Parsers.ManyTill(valueParser, tillParser);
            var initialPosition = input.Position;

            var expected = new List<char>();
            expected.Add('1');
            expected.Add('2');
            expected.Add('3');

            var result = parser(input);

            Assert.True(result.IsResult);
            Assert.Equal(expected.Count, result.Result.Count);
            Assert.NotEqual(initialPosition, result.UnconsumedInput.Position);

            for (int i = 0; i < expected.Count; i++)
            {
                Assert.Equal(expected[i], result.Result[i]);
            }
        }

        [Fact]
        public void NoTill()
        {
            var input = ParserInput.Create("123");
            var valueParser = Parsers.Digit;
            var tillParser = Parsers.Char('|');
            var parser = Parsers.ManyTill(valueParser, tillParser);

            var result = parser(input);

            Assert.True(result.IsError);
        }

        [Fact]
        public void TillTooLate()
        {
            var input = ParserInput.Create("123a|");
            var valueParser = Parsers.Digit;
            var tillParser = Parsers.Char('|');
            var parser = Parsers.ManyTill(valueParser, tillParser);

            var result = parser(input);

            Assert.True(result.IsError);
        }
    }
}
