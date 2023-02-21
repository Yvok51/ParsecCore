
using ParsecCore.Input;
using ParsecCore.MaybeNS;
using PythonParser.Parser;
using PythonParser.Structures;

namespace PythonParserTests.Parser.ExpressionTests
{
    public class PrimaryTests
    {
        [Theory]
        [InlineData("abc.ref")]
        [InlineData("abc . ref")]
        public void AttributeRefToIdentifier(string inputString)
        {
            var input = ParserInput.Create(inputString);
            var result = Expressions.Expression(Control.Lexeme)(input);

            Assert.True(result.IsResult);
            Assert.True(result.Result is AttributeRef);
            Assert.Equal(new AttributeRef(new IdentifierLiteral("abc"), new IdentifierLiteral("ref")), result.Result);
        }

        [Theory]
        [InlineData("\"string\".ref")]
        [InlineData("\"string\" . ref")]
        public void AttributeRefToString(string inputString)
        {
            var input = ParserInput.Create(inputString);
            var result = Expressions.Expression(Control.Lexeme)(input);

            Assert.True(result.IsResult);
            Assert.True(result.Result is AttributeRef);
            Assert.Equal(new AttributeRef(new StringLiteral("", "string"), new IdentifierLiteral("ref")), result.Result);
        }

        [Theory]
        [InlineData("abc[0]", "abc", 0)]
        [InlineData("hello[2]", "hello", 2)]
        [InlineData("s[100]", "s", 100)]
        public void IntegerSubscript(string inputString, string identifier, int subscript)
        {
            var input = ParserInput.Create(inputString);
            var result = Expressions.Expression(Control.Lexeme)(input);

            Assert.True(result.IsResult);
            Assert.True(result.Result is Subscription);
            Assert.Equal(new Subscription(new IdentifierLiteral(identifier), new List<Expr>() { new IntegerLiteral(subscript) }), result.Result);
        }

        [Theory]
        [InlineData("abc[0:10:2]", "abc", 0, 10, 2)]
        [InlineData("abc[:10:2]", "abc", null, 10, 2)]
        [InlineData("abc[::2]", "abc", null, null, 2)]
        [InlineData("abc[0::2]", "abc", 0, null, 2)]
        [InlineData("abc[0:10]", "abc", 0, 10, null)]
        [InlineData("abc[0:]", "abc", 0, null, null)]
        [InlineData("abc[:10]", "abc", null, 10, null)]
        [InlineData("abc[:]", "abc", null, null, null)]
        public void IntegerSlice(string inputString, string identifier, int? lowerBound, int? upperBound, int? stride)
        {
            var input = ParserInput.Create(inputString);
            var result = Expressions.Expression(Control.Lexeme)(input);

            var nullableToMaybe = (int? nullable) => nullable.HasValue
                ? Maybe.FromValue<Expr>(new IntegerLiteral(nullable.Value))
                : Maybe.Nothing<Expr>();

            Assert.True(result.IsResult);
            Assert.True(result.Result is Slice);
            Assert.Equal(
                new Slice(
                    new IdentifierLiteral(identifier),
                    new List<Expr>() { 
                        new SliceItem(
                            nullableToMaybe(lowerBound),
                            nullableToMaybe(upperBound),
                            nullableToMaybe(stride)
                        ) 
                    }),
                result.Result
            );
        }

        [Fact]
        public void DoubleColonSliceNotAccepted()
        {
            var input = ParserInput.Create("abc[::]");
            var result = Expressions.Expression(Control.Lexeme)(input);

            Assert.True(result.IsError);
        }

        [Fact]
        public void EmptyCall()
        {
            var input = ParserInput.Create("func()");
            var result = Expressions.Expression(Control.Lexeme)(input);

            Assert.True(result.IsResult);
            Assert.True(result.Result is Call);
            Assert.Equal(
                new Call(
                    new IdentifierLiteral("func"),
                    Maybe.Nothing<IReadOnlyList<Expr>>(),
                    Maybe.Nothing<IReadOnlyList<KeywordArgument>>(),
                    Maybe.Nothing<Expr>(),
                    Maybe.Nothing<Expr>()
                ),
                result.Result
            );
        }

        [Fact]
        public void SimpleCall()
        {
            var input = ParserInput.Create("func(a, b, c)");
            var result = Expressions.Expression(Control.Lexeme)(input);

            Assert.True(result.IsResult);
            Assert.True(result.Result is Call);
            Assert.Equal(
                new Call(
                    new IdentifierLiteral("func"),
                    Maybe.FromValue<IReadOnlyList<Expr>>(new List<Expr>()
                    { 
                        new IdentifierLiteral("a"),
                        new IdentifierLiteral("b"),
                        new IdentifierLiteral("c") 
                    }),
                    Maybe.Nothing<IReadOnlyList<KeywordArgument>>(),
                    Maybe.Nothing<Expr>(),
                    Maybe.Nothing<Expr>()
                ),
                result.Result
            );
        }

        [Fact]
        public void ComplexPrimary()
        {
            var input = ParserInput.Create("a.ref[0](b, c)");
            var result = Expressions.Expression(Control.Lexeme)(input);

            Assert.True(result.IsResult);
            Assert.True(result.Result is Call);
            Assert.Equal(
                new Call(
                    new Subscription(
                        new AttributeRef(
                            new IdentifierLiteral("a"),
                            new IdentifierLiteral("ref")
                        ),
                        new List<Expr>() { new IntegerLiteral(0) }
                    ),
                    Maybe.FromValue<IReadOnlyList<Expr>>(new List<Expr>()
                    {
                        new IdentifierLiteral("b"),
                        new IdentifierLiteral("c")
                    }),
                    Maybe.Nothing<IReadOnlyList<KeywordArgument>>(),
                    Maybe.Nothing<Expr>(),
                    Maybe.Nothing<Expr>()
                ),
                result.Result
            );
        }
    }
}
