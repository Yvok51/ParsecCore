using ParsecCore.Input;
using PythonParser.Parser;
using PythonParser.Structures;

namespace PythonParserTests.Parser.ExpressionTests
{
    public class DisplayTests
    {
        [Theory]
        [InlineData("[]")]
        [InlineData("[  ]")]
        [InlineData("[ \t  ]")]
        [InlineData("[  \n ]")]
        public void EmptyListDisplay(string inputString)
        {
            var input = ParserInput.Create(inputString);
            var result = Expressions.Expression(Control.Lexeme)(input);

            Assert.True(result.IsResult);
            Assert.True(result.Result is ListDisplay);
            Assert.Equal(new ListDisplay(new List<Expr>()), result.Result);
        }

        [Theory]
        [InlineData("[a, b, c]")]
        [InlineData("[a,    b,c]")]
        [InlineData("[a,\n b,\n c]")]
        public void UniformListDisplay(string inputString)
        {

            var input = ParserInput.Create(inputString);
            var result = Expressions.Expression(Control.Lexeme)(input);

            Assert.True(result.IsResult);
            Assert.True(result.Result is ListDisplay);
            Assert.Equal(
                new ListDisplay(new List<Expr>() { new IdentifierLiteral("a"), new IdentifierLiteral("b"), new IdentifierLiteral("c") }),
                result.Result
            );
        }

        [Theory]
        [InlineData("[a, \"b\", 4]")]
        [InlineData("[a,    \"b\",4]")]
        [InlineData("[a,\n \"b\",\n 4]")]
        public void NonUniformListDisplay(string inputString)
        {

            var input = ParserInput.Create(inputString);
            var result = Expressions.Expression(Control.Lexeme)(input);

            Assert.True(result.IsResult);
            Assert.True(result.Result is ListDisplay);
            Assert.Equal(
                new ListDisplay(new List<Expr>() { new IdentifierLiteral("a"), new StringLiteral("", "b"), new IntegerLiteral(4) }),
                result.Result
            );
        }

        [Theory]
        [InlineData("{a, b, c}")]
        [InlineData("{a,    b,c}")]
        [InlineData("{a,\n b,\n c}")]
        public void UniformSetDisplay(string inputString)
        {

            var input = ParserInput.Create(inputString);
            var result = Expressions.Expression(Control.Lexeme)(input);

            Assert.True(result.IsResult);
            Assert.True(result.Result is SetDisplay);
            Assert.Equal(
                new SetDisplay(new List<Expr>() { new IdentifierLiteral("a"), new IdentifierLiteral("b"), new IdentifierLiteral("c") }),
                result.Result
            );
        }

        [Theory]
        [InlineData("{a, \"b\", 4}")]
        [InlineData("{a,    \"b\",4}")]
        [InlineData("{a,\n \"b\",\n 4}")]
        public void NonUniformSetDisplay(string inputString)
        {

            var input = ParserInput.Create(inputString);
            var result = Expressions.Expression(Control.Lexeme)(input);

            Assert.True(result.IsResult);
            Assert.True(result.Result is SetDisplay);
            Assert.Equal(
                new SetDisplay(new List<Expr>() { new IdentifierLiteral("a"), new StringLiteral("", "b"), new IntegerLiteral(4) }),
                result.Result
            );
        }

        [Theory]
        [InlineData("{}")]
        [InlineData("{  }")]
        [InlineData("{ \t  }")]
        [InlineData("{  \n }")]
        public void EmptyDictDisplay(string inputString)
        {
            var input = ParserInput.Create(inputString);
            var result = Expressions.Expression(Control.Lexeme)(input);

            Assert.True(result.IsResult);
            Assert.True(result.Result is DictDisplay);
            Assert.Equal(new DictDisplay(new List<KeyDatum>()), result.Result);
        }

        [Theory]
        [InlineData("{\"a\": a, \"b\": b, \"c\": c}")]
        [InlineData("{\"a\": a,\t  \"b\": b,\"c\": c}")]
        [InlineData("{\"a\": a,\n \"b\": b,\n \"c\": c}")]
        public void UniformDictDisplay(string inputString)
        {

            var input = ParserInput.Create(inputString);
            var result = Expressions.Expression(Control.Lexeme)(input);

            Assert.True(result.IsResult);
            Assert.True(result.Result is DictDisplay);
            Assert.Equal(
                new DictDisplay(new List<KeyDatum>() {
                    new KeyDatum(new StringLiteral("", "a"), new IdentifierLiteral("a")),
                    new KeyDatum(new StringLiteral("", "b"), new IdentifierLiteral("b")),
                    new KeyDatum(new StringLiteral("", "c"), new IdentifierLiteral("c"))
                }),
                result.Result
            );
        }

        [Theory]
        [InlineData("{\"a\": a, \"b\": \"b\", \"c\": 4}")]
        [InlineData("{\"a\": a,   \"b\": \"b\",\"c\": 4}")]
        [InlineData("{\"a\": a,\n \"b\": \"b\",\n \"c\": 4}")]
        public void NonUniformDictDisplay(string inputString)
        {

            var input = ParserInput.Create(inputString);
            var result = Expressions.Expression(Control.Lexeme)(input);

            Assert.True(result.IsResult);
            Assert.True(result.Result is DictDisplay);
            Assert.Equal(
                new DictDisplay(new List<KeyDatum>() {
                    new KeyDatum(new StringLiteral("", "a"), new IdentifierLiteral("a")),
                    new KeyDatum(new StringLiteral("", "b"), new StringLiteral("", "b")),
                    new KeyDatum(new StringLiteral("", "c"), new IntegerLiteral(4))
                }),
                result.Result
            );
        }
    }
}
