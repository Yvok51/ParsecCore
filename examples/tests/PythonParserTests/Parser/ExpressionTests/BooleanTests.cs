using ParsecCore;
using PythonParser.Parser;
using PythonParser.Structures;

namespace PythonParserTests.Parser.ExpressionTests
{
    public class BooleanTests
    {
        [Theory]
        [InlineData("x and y", BinaryOperator.And)]
        [InlineData("x or y", BinaryOperator.Or)]
        internal void BasicBinaryBooleanExpression(string inputString, BinaryOperator op)
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
