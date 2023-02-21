using ParsecCore.Input;
using PythonParser.Parser;
using PythonParser.Structures;

namespace PythonParserTests.Parser.ExpressionTests
{
    public class MutiplicativeTests
    {
        [Theory]
        [InlineData("2 * 3", BinaryOperator.Star)]
        [InlineData("2 / 3", BinaryOperator.Slash)]
        [InlineData("2 // 3", BinaryOperator.DoubleSlash)]
        [InlineData("2 % 3", BinaryOperator.Modulo)]
        internal void BasicMultiplicativeExpression(string inputString, BinaryOperator op)
        {
            var input = ParserInput.Create(inputString);
            var result = Expressions.Expression(Control.Lexeme)(input);

            Assert.True(result.IsResult);
            Assert.True(result.Result is Binary);
            Assert.Equal(
                new Binary(new IntegerLiteral(2), op, new IntegerLiteral(3)),
                result.Result
            );
        }

        [Fact]
        internal void ComplexMultiplicativeExpression()
        {
            var input = ParserInput.Create("-2 * 3 ** 4");
            var result = Expressions.Expression(Control.Lexeme)(input);

            Assert.True(result.IsResult);
            Assert.True(result.Result is Binary);
            Assert.Equal(
                new Binary(
                    new Unary(new IntegerLiteral(2), UnaryOperator.Minus),
                    BinaryOperator.Star,
                    new Binary(new IntegerLiteral(3), BinaryOperator.DoubleStar, new IntegerLiteral(4))),
                result.Result
            );
        }
    }
}
