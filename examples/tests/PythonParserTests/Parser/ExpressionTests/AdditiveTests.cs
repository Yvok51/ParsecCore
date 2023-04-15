using ParsecCore;
using PythonParser.Parser;
using PythonParser.Structures;

namespace PythonParserTests.Parser.ExpressionTests
{
    public class AdditiveTests
    {
        [Theory]
        [InlineData("2 + 3", BinaryOperator.Plus)]
        [InlineData("2 - 3", BinaryOperator.Minus)]
        internal void BasicAdditiveExpression(string inputString, BinaryOperator op)
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
            var input = ParserInput.Create("-2 * 3 ** 4 + 10 - 6 * 7");
            var result = Expressions.Expression(Control.Lexeme)(input);

            Assert.True(result.IsResult);
            Assert.True(result.Result is Binary);
            Assert.Equal(
                new Binary(
                    new Binary(
                        new Binary(
                            new Unary(new IntegerLiteral(2), UnaryOperator.Minus),
                            BinaryOperator.Star,
                            new Binary(
                                new IntegerLiteral(3),
                                BinaryOperator.DoubleStar,
                                new IntegerLiteral(4)
                            )
                        ),
                        BinaryOperator.Plus,
                        new IntegerLiteral(10)
                    ),
                    BinaryOperator.Minus,
                    new Binary(
                        new IntegerLiteral(6),
                        BinaryOperator.Star,
                        new IntegerLiteral(7)
                    )
                ),
                result.Result
            );
        }
    }
}
