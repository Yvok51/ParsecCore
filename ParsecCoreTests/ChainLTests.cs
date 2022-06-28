using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;
using ParsecCore;
using ParsecCore.Input;

namespace ParsecCoreTests
{
    public class ChainLTests
    {
        private static Func<int, int, int> addition = (int a, int b) => a + b;
        private static Func<int, int, int> multiplication = (int a, int b) => a * b;

        private static Parser<Func<int, int, int>> additionParser =
            from _ in Parsers.Symbol("+")
            select addition;

        private static Parser<Func<int, int, int>> multiplicationParser =
            from _ in Parsers.Symbol("*")
            select multiplication;

        [Fact]
        public void TwoPlusTwo()
        {
            var input = ParserInput.Create("2 + 2");
            var parser = Combinators.ChainL(Parsers.DecimalInteger, additionParser, 0);

            var result = parser(input);

            Assert.True(result.HasRight);
            Assert.Equal(4, result.Right);
        }

        [Fact]
        public void MixedOperators()
        {
            var input = ParserInput.Create("2 + 2 * 3");
            var parser = Combinators.ChainL(Parsers.DecimalInteger, Combinators.Choice(additionParser, multiplicationParser), 0);

            var result = parser(input);

            Assert.True(result.HasRight);
            Assert.Equal(12, result.Right);
        }

        [Fact]
        public void SingleValue()
        {
            var input = ParserInput.Create("3");
            var parser = Combinators.ChainL(Parsers.DecimalInteger, additionParser, 0);

            var result = parser(input);

            Assert.True(result.HasRight);
            Assert.Equal(3, result.Right);
        }

        [Fact]
        public void DefaultValue()
        {
            var input = ParserInput.Create("abc");
            var parser = Combinators.ChainL(Parsers.DecimalInteger, additionParser, 0);

            var result = parser(input);

            Assert.True(result.HasRight);
            Assert.Equal(0, result.Right);
        }

        [Fact]
        public void FailedParse()
        {
            var input = ParserInput.Create("2 +");
            var parser = Combinators.ChainL(Parsers.DecimalInteger, additionParser, 0);

            var result = parser(input);

            Assert.True(result.HasLeft);
        }
    }
}
