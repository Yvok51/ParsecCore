using ParsecCore;
using PythonParser.Parser;
using PythonParser.Structures;

namespace PythonParserTests.Parser.StatementTests
{
    public class ExpressionStatementTests
    {
        [Fact]
        public void CallExpressionStatement()
        {
            var input = ParserInput.Create("call()\n");
            var result = Statements.Statement(input);

            Assert.True(result.IsResult);
            Assert.Equal(
                new ExpressionStmt(new List<Expr>()
                {
                    new Call(
                        new IdentifierLiteral("call"),
                        Maybe.Nothing<IReadOnlyList<Expr>>(),
                        Maybe.Nothing<IReadOnlyList<KeywordArgument>>(),
                        Maybe.Nothing<Expr>(),
                        Maybe.Nothing<Expr>()
                    )
                }),
                result.Result
            );
        }

        [Theory]
        [InlineData("x; y; z;\n")]
        [InlineData("x; y; z\n")]
        public void StatementList(string inputString)
        {
            var input = ParserInput.Create(inputString);
            var result = Statements.Statement(input);

            Assert.True(result.IsResult);
            Assert.Equal(
                new Suite(new List<Stmt>()
                {
                    new ExpressionStmt(new List<Expr>()
                    {
                        new IdentifierLiteral("x")
                    }),
                    new ExpressionStmt(new List<Expr>()
                    {
                        new IdentifierLiteral("y")
                    }),
                    new ExpressionStmt(new List<Expr>()
                    {
                        new IdentifierLiteral("z")
                    })
                }),
                result.Result
            );
        }
    }
}
