﻿using ParsecCore;
using ParsecCore.Input;
using System.Collections.Generic;
using Xunit;

namespace ParsecCoreTests
{
    public class EndByTests
    {
        [Fact]
        public void SingleSeperatorParsed()
        {
            var valueParser = Parsers.Digits;
            var sepParser = Parsers.Char(',');
            Parser<IReadOnlyList<string>> parser = Combinators.EndBy(valueParser, sepParser);
            IParserInput input = ParserInput.Create(",");

            var result = parser(input);

            Assert.True(result.HasRight);
            Assert.Equal(0, result.Right.Count);
        }

        [Fact]
        public void SingleValueListParsed()
        {
            var valueParser = Parsers.Digits;
            var sepParser = Parsers.Char(',');
            Parser<IReadOnlyList<string>> parser = Combinators.EndBy(valueParser, sepParser);
            IParserInput input = ParserInput.Create("123,");

            var result = parser(input);

            Assert.True(result.HasRight);
            Assert.Equal(1, result.Right.Count);
            Assert.Equal("123", result.Right[0]);
        }

        [Fact]
        public void SeveralValueListParsed()
        {
            var valueParser = Parsers.Digits;
            var sepParser = Parsers.Char(',');
            Parser<IReadOnlyList<string>> parser = Combinators.EndBy(valueParser, sepParser);
            IParserInput input = ParserInput.Create("123,159,357,456,789,");

            var expected = new List<string>();
            expected.Add("123");
            expected.Add("159");
            expected.Add("357");
            expected.Add("456");
            expected.Add("789");

            var result = parser(input);

            Assert.True(result.HasRight);
            Assert.Equal(expected.Count, result.Right.Count);

            for (int i = 0; i < expected.Count; i++)
            {
                Assert.Equal(expected[i], result.Right[i]);
            }
        }

        [Fact]
        public void MissingSeperatorAtTheEndFailure()
        {
            var valueParser = Parsers.Digits;
            var sepParser = Parsers.Char(',');
            Parser<IReadOnlyList<string>> parser = Combinators.EndBy(valueParser, sepParser);
            IParserInput input = ParserInput.Create("123,159,357,456,789");

            var result = parser(input);

            Assert.True(result.HasLeft);
        }

        [Fact]
        public void ValueNotParsedFailure()
        {
            var valueParser = Parsers.Digits;
            var sepParser = Parsers.Char(',');
            Parser<IReadOnlyList<string>> parser = Combinators.EndBy(valueParser, sepParser);
            IParserInput input = ParserInput.Create("123,159,357,4a5,789");

            var result = parser(input);

            Assert.True(result.HasLeft);
        }
    }
}
