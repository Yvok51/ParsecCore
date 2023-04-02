using ParsecCore.Input;
using ParsecCore.MaybeNS;
using PythonParser.Parser;
using PythonParser.Structures;

namespace PythonParserTests.Parser.StatementTests
{
    public class ReturnTests
    {
        [Fact]
        public void EmptyReturns()
        {
            var input = ParserInput.Create("return\n");
            var result = Statements.Statement(input);

            Assert.True(result.IsResult);
            Assert.Equal(
                new Suite(new List<Stmt>()
                {
                    new Return(Maybe.Nothing<IReadOnlyList<Expr>>())
                }),
                result.Result
            );
        }

        [Fact]
        public void ReturnWithVariable()
        {
            var input = ParserInput.Create("return x\n");
            var result = Statements.Statement(input);

            Assert.True(result.IsResult);
            Assert.Equal(
                new Suite(new List<Stmt>()
                {
                    new Return(Maybe.FromValue<IReadOnlyList<Expr>>(new List<Expr>() { new IdentifierLiteral("x") }))
                }),
                result.Result
            );
        }

        [Fact]
        public void ReturnWithComplexExpression()
        {
            var input = ParserInput.Create("return 2 * x\n");
            var result = Statements.Statement(input);

            Assert.True(result.IsResult);
            Assert.Equal(
                new Suite(new List<Stmt>()
                {
                    new Return(Maybe.FromValue<IReadOnlyList<Expr>>(new List<Expr>()
                    { 
                        new Binary(new IntegerLiteral(2), BinaryOperator.Star, new IdentifierLiteral("x"))
                    }))
                }),
                result.Result
            );
        }
    }
}
