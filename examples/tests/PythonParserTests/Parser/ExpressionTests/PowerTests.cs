using ParsecCore;
using PythonParser.Parser;
using PythonParser.Structures;

namespace PythonParserTests.Parser.ExpressionTests
{
    public class PowerTests
    {
        [Theory]
        [InlineData("1**2", 1, 2)]
        [InlineData("4 ** 20", 4, 20)]
        public void PowerParsed(string inputString, int basis, int exponent)
        {
            var input = ParserInput.Create(inputString);
            var result = Expressions.Expression(Control.Lexeme)(input);

            Assert.True(result.IsResult);
            Assert.True(result.Result is Binary);
            Assert.Equal(
                new Binary(new IntegerLiteral(basis), BinaryOperator.DoubleStar, new IntegerLiteral(exponent)),
                result.Result
            );
        }

        [Fact]
        public void MultiplePowers()
        {
            var input = ParserInput.Create("1 ** 2 ** 3 ** 4");
            var result = Expressions.Expression(Control.Lexeme)(input);

            Assert.True(result.IsResult);
            Assert.True(result.Result is Binary);
            Assert.Equal(
                new Binary(
                    new IntegerLiteral(1),
                    BinaryOperator.DoubleStar,
                    new Binary(
                        new IntegerLiteral(2),
                        BinaryOperator.DoubleStar,
                        new Binary(
                            new IntegerLiteral(3),
                            BinaryOperator.DoubleStar,
                            new IntegerLiteral(4)
                        )
                    )
                ),
                result.Result
            );
        }
    }
}
