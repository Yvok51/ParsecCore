﻿using ParsecCore;
using System;
using System.Linq;
using Xunit;

namespace ParsecCoreTests
{
    public class ChainLTests
    {
        private static Func<int, int, int> addition = (int a, int b) => a + b;
        private static Func<int, int, int> multiplication = (int a, int b) => a * b;

        private static Parser<Func<int, int, int>, char> additionParser =
            from _ in Parsers.Symbol("+")
            select addition;

        private static Parser<Func<int, int, int>, char> multiplicationParser =
            from _ in Parsers.Symbol("*")
            select multiplication;

        private static Parser<int, char> DecimalInteger = Parsers.DecimalInteger.FollowedBy(Parsers.Spaces);

        [Fact]
        public void TwoPlusTwo()
        {
            var input = ParserInput.Create("2 + 2");
            var parser = Parsers.ChainL(DecimalInteger, additionParser, 0);

            var result = parser(input);

            Assert.True(result.IsResult);
            Assert.Equal(4, result.Result);
        }

        [Fact]
        public void MixedOperators()
        {
            var input = ParserInput.Create("2 + 2 * 3");
            var parser = Parsers.ChainL(DecimalInteger, Parsers.Choice(additionParser, multiplicationParser), 0);

            var result = parser(input);

            Assert.True(result.IsResult);
            Assert.Equal(12, result.Result);
        }

        [Fact]
        public void SingleValue()
        {
            var input = ParserInput.Create("3");
            var parser = Parsers.ChainL(DecimalInteger, additionParser, 0);

            var result = parser(input);

            Assert.True(result.IsResult);
            Assert.Equal(3, result.Result);
        }

        [Fact]
        public void DefaultValue()
        {
            var input = ParserInput.Create("abc");
            var parser = Parsers.ChainL(DecimalInteger, additionParser, 0);

            var result = parser(input);

            Assert.True(result.IsResult);
            Assert.Equal(0, result.Result);
        }

        [Fact]
        public void FailedParse()
        {
            var input = ParserInput.Create("2 +");
            var parser = Parsers.ChainL(DecimalInteger, additionParser, 0);

            var result = parser(input);

            Assert.True(result.IsError);
        }
    }
}
