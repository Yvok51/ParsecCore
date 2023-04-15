using ParsecCore;
using ParsecCore.Expressions;
using System.Collections.Generic;
using Xunit;

namespace ParsecCoreTests.Expressions
{
    public class ExpressionTests
    {
        public ExpressionTests()
        {
            OperatorTable = OperatorTable<int, char>.Create(new List<List<Operator<int, char>>>
                {
                    new List<Operator<int, char>>() { Expression.PrefixOperator<int>("+", x => x), Expression.PrefixOperator<int>("-", x => -x) },
                    new List<Operator<int, char>>() { Expression.PostfixOperator<int>("++", x => x + 1) },
                    new List<Operator<int, char>>() { Expression.BinaryOperator<int>("*", (x, y) => x * y, Associativity.Left), Expression.BinaryOperator<int>("/", (x, y) => x / y, Associativity.Left) },
                    new List <Operator<int, char>>() { Expression.BinaryOperator < int >("+",(x, y) => x + y, Associativity.Left), Expression.BinaryOperator < int >("-",(x, y) => x - y, Associativity.Left) }
                }
            );

            var termParser = Parsers.Natural.FollowedBy(Parsers.Spaces)
                .Or(Parsers.Indirect(() => Parsers.Between(Parsers.Symbol("("), ExpressionParser, Parsers.Symbol(")"))));

            ExpressionParser = Expression.Build(OperatorTable, termParser);
        }

        public OperatorTable<int, char> OperatorTable { get; set; }
        public Parser<int, char> ExpressionParser { get; set; }

        [Theory]
        [InlineData("0", 0)]
        [InlineData("1", 1)]
        [InlineData("2", 2)]
        [InlineData("2  ", 2)]
        public void SingleNumber(string inputStr, int expectedResult)
        {
            var input = ParserInput.Create(inputStr);
            var result = ExpressionParser(input);

            Assert.True(result.IsResult);
            Assert.Equal(expectedResult, result.Result);
        }

        [Theory]
        [InlineData("+1", 1)]
        [InlineData("+ 2", 2)]
        [InlineData("-3", -3)]
        [InlineData("- 4", -4)]
        public void PrefixOperator(string inputStr, int expectedResult)
        {
            var input = ParserInput.Create(inputStr);
            var result = ExpressionParser(input);

            Assert.True(result.IsResult);
            Assert.Equal(expectedResult, result.Result);
        }

        [Theory]
        [InlineData("0++", 1)]
        [InlineData("1++", 2)]
        [InlineData("2 ++", 3)]
        public void PostfixOperator(string inputStr, int expectedResult)
        {
            var input = ParserInput.Create(inputStr);
            var result = ExpressionParser(input);

            Assert.True(result.IsResult);
            Assert.Equal(expectedResult, result.Result);
        }

        [Theory]
        [InlineData("1 + 1", 2)]
        [InlineData("5 - 2", 3)]
        [InlineData("2 * 2", 4)]
        [InlineData("2*2", 4)]
        [InlineData("4 / 2", 2)]
        [InlineData("5 / 2", 2)]
        public void BinaryOperator(string inputStr, int expectedResult)
        {
            var input = ParserInput.Create(inputStr);
            var result = ExpressionParser(input);

            Assert.True(result.IsResult);
            Assert.Equal(expectedResult, result.Result);
        }

        [Theory]
        [InlineData("5 - 2 - 1", 2)]
        [InlineData("10 / 2 / 2", 2)]
        public void AssociativityCorrect(string inputStr, int expectedResult)
        {
            var input = ParserInput.Create(inputStr);
            var result = ExpressionParser(input);

            Assert.True(result.IsResult);
            Assert.Equal(expectedResult, result.Result);
        }

        [Theory]
        [InlineData("5 * 2 + 3", 13)]
        [InlineData("2 + 4 / 2", 4)]
        [InlineData("2 + -4", -2)]
        [InlineData("-2 + 4", 2)]
        [InlineData("-2++", -1)]
        [InlineData("1++ + 2", 4)]
        public void PrecedenceCorrect(string inputStr, int expectedResult)
        {
            var input = ParserInput.Create(inputStr);
            var result = ExpressionParser(input);

            Assert.True(result.IsResult);
            Assert.Equal(expectedResult, result.Result);
        }

        [Theory]
        [InlineData("(2 + 3) * 2 / 4", 2)]
        [InlineData("2 * 3 * 4 + 10 / 3", 27)]
        [InlineData("(-2 * 10 - 15) / 7", -5)]
        [InlineData("(-2 * 10 - 15) / -7", 5)]
        public void Complex(string inputStr, int expectedResult)
        {
            var input = ParserInput.Create(inputStr);
            var result = ExpressionParser(input);

            Assert.True(result.IsResult);
            Assert.Equal(expectedResult, result.Result);
        }
    }
}
