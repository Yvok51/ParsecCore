using ParsecCore.Input;
using PythonParser.Parser;
using PythonParser.Structures;

namespace PythonParserTests.Parser.ExpressionTests
{
    public class ComparisonTests
    {
        [Theory]
        [InlineData("x < y", BinaryOperator.LT)]
        [InlineData("x > y", BinaryOperator.GT)]
        [InlineData("x <= y", BinaryOperator.LE)]
        [InlineData("x >= y", BinaryOperator.GE)]
        [InlineData("x == y", BinaryOperator.Equal)]
        [InlineData("x != y", BinaryOperator.NotEqual)]
        [InlineData("x is y", BinaryOperator.Is)]
        [InlineData("x is not y", BinaryOperator.IsNot)]
        [InlineData("x in y", BinaryOperator.In)]
        [InlineData("x not in y", BinaryOperator.NotIn)]
        internal void BasicComparisonExpression(string inputString, BinaryOperator op)
        {
            var input = ParserInput.Create(inputString);
            var result = Expressions.Expression(Control.Lexeme)(input);

            Assert.True(result.IsResult);
            Assert.True(result.Result is Binary);
            Assert.Equal(
                new Binary(new IdentifierLiteral("x"), op, new IdentifierLiteral("y")),
                result.Result
            );
        }
    }
}
