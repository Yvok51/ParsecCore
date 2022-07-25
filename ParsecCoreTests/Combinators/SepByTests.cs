using ParsecCore;
using ParsecCore.Input;
using System.Collections.Generic;
using Xunit;

namespace ParsecCoreTests
{
    public class SepByTests
    {
        [Fact]
        public void EmptyListParsed()
        {
            var valueParser = Parsers.Digits;
            var sepParser = Parsers.Char(',');
            Parser<IReadOnlyList<string>> parser = Combinators.SepBy(valueParser, sepParser);
            var input = ParserInput.Create("");

            var result = parser(input);

            Assert.True(result.IsResult);
            Assert.Equal(0, result.Result.Count);
        }

        [Fact]
        public void SingleValueListParsed()
        {
            var valueParser = Parsers.Digits;
            var sepParser = Parsers.Char(',');
            Parser<IReadOnlyList<string>> parser = Combinators.SepBy(valueParser, sepParser);
            var input = ParserInput.Create("123");

            var result = parser(input);

            Assert.True(result.IsResult);
            Assert.Equal(1, result.Result.Count);
            Assert.Equal("123", result.Result[0]);
        }

        [Fact]
        public void SeveralValueListParsed()
        {
            var valueParser = Parsers.Digits;
            var sepParser = Parsers.Char(',');
            Parser<IReadOnlyList<string>> parser = Combinators.SepBy(valueParser, sepParser);
            var input = ParserInput.Create("123,159,357,456,789");

            var expected = new List<string>();
            expected.Add("123");
            expected.Add("159");
            expected.Add("357");
            expected.Add("456");
            expected.Add("789");

            var result = parser(input);

            Assert.True(result.IsResult);
            Assert.Equal(expected.Count, result.Result.Count);

            for (int i = 0; i < expected.Count; i++)
            {
                Assert.Equal(expected[i], result.Result[i]);
            }
        }

        [Fact]
        public void SeperatorAtTheEndFailure()
        {
            var valueParser = Parsers.Digits;
            var sepParser = Parsers.Char(',');
            Parser<IReadOnlyList<string>> parser = Combinators.SepBy(valueParser, sepParser);
            var input = ParserInput.Create("123,159,357,456,789,");

            var result = parser(input);

            Assert.True(result.IsError);
        }

        [Fact]
        public void ValueNotParsedFailure()
        {
            var valueParser = Parsers.Digits;
            var sepParser = Parsers.Char(',');
            Parser<IReadOnlyList<string>> parser = Combinators.SepBy(valueParser, sepParser);
            var input = ParserInput.Create("123,159,357,a45,789");

            var result = parser(input);

            Assert.True(result.IsError);
        }
    }
}
