
using ParsecCore.Input;
using PythonParser.Parser;
using PythonParser.Structures;

namespace PythonParserTests.Parser.ExpressionTests
{
    public class UnaryTests
    {
        [Theory]
        [InlineData("-2", UnaryOperator.Minus, 2)]
        [InlineData("- 10", UnaryOperator.Minus, 10)]
        [InlineData("+ 10", UnaryOperator.Plus, 10)]
        internal void Unary(string inputString, UnaryOperator op, int num)
        {
            var input = ParserInput.Create(inputString);
            var result = Expressions.Expression(Control.Lexeme)(input);

            Assert.True(result.IsResult);
            Assert.True(result.Result is Unary);
            Assert.Equal(
                new Unary(new IntegerLiteral(num), op),
                result.Result
            );
        }

        [Fact]
        internal void ComplexUnary()
        {
            var input = ParserInput.Create("-2 ** -3");
            var result = Expressions.Expression(Control.Lexeme)(input);

            Assert.True(result.IsResult);
            Assert.True(result.Result is Unary);
            Assert.Equal(
                new Unary(
                    new Binary(
                        new IntegerLiteral(2),
                        BinaryOperator.DoubleStar,
                        new Unary(new IntegerLiteral(3), UnaryOperator.Minus)
                    ),
                    UnaryOperator.Minus
                ),
                result.Result
            );
        }
    }
}
