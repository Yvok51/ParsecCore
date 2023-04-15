using ParsecCore;
using PythonParser.Parser;
using PythonParser.Structures;

namespace PythonParserTests.Parser.LiteralTests
{
    public class KeywordLiteralTests
    {
        [Fact]
        public void TrueParsed()
        {
            var input = ParserInput.Create("True");
            var result = Expressions.Expression(Control.Lexeme)(input);

            Assert.True(result.IsResult);
            Assert.True(result.Result is BooleanLiteral);
            Assert.Equal(new BooleanLiteral(true), result.Result);
        }

        [Fact]
        public void FalseParsed()
        {
            var input = ParserInput.Create("False");
            var result = Expressions.Expression(Control.Lexeme)(input);

            Assert.True(result.IsResult);
            Assert.True(result.Result is BooleanLiteral);
            Assert.Equal(new BooleanLiteral(false), result.Result);
        }

        [Fact]
        public void NoneParsed()
        {
            var input = ParserInput.Create("None");
            var result = Expressions.Expression(Control.Lexeme)(input);

            Assert.True(result.IsResult);
            Assert.True(result.Result is NoneLiteral);
            Assert.Equal(new NoneLiteral(), result.Result);
        }
    }
}
