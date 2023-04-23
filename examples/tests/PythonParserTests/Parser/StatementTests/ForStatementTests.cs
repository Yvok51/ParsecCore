using ParsecCore;
using PythonParser.Parser;
using PythonParser.Structures;

namespace PythonParserTests.Parser.StatementTests
{
    public class ForStatementTests
    {
        [Theory]
        [InlineData("for x in xs:\n  pass\n\n")]
        [InlineData("for x in xs:\n  pass\n")]
        public void SimpleForStatement(string inputString)
        {
            var input = ParserInput.Create(inputString);
            var result = Statements.Statement(input);

            Assert.True(result.IsResult);
            Assert.Equal(
                new For(
                    new List<Expr>() { new IdentifierLiteral("x") },
                    new List<Expr>() { new IdentifierLiteral("xs") },
                    new Suite(new List<Stmt>() { new Pass() }),
                    Maybe.Nothing<Suite>()
                ),
                result.Result
            );
        }

        [Theory]
        [InlineData("for x in xs:\n  call()\n  return\n")]
        [InlineData("for x in xs:\n  call()\n  return\n\n")]
        public void MultipleStatements(string inputString)
        {
            var input = ParserInput.Create(inputString);
            var result = Statements.Statement(input);

            Assert.True(result.IsResult);
            Assert.Equal(
                new For(
                    new List<Expr>() { new IdentifierLiteral("x") },
                    new List<Expr>() { new IdentifierLiteral("xs") },
                    new Suite(new List<Stmt>()
                    {
                        new ExpressionStmt(
                            new List<Expr>()
                            {
                                new Call(
                                    new IdentifierLiteral("call"),
                                    Maybe.Nothing<IReadOnlyList<Expr>>(),
                                    Maybe.Nothing<IReadOnlyList<KeywordArgument>>(),
                                    Maybe.Nothing<Expr>(),
                                    Maybe.Nothing<Expr>()
                                )
                            }
                        ),
                        new Return(Maybe.Nothing<IReadOnlyList<Expr>>())
                    }),
                    Maybe.Nothing<Suite>()
                ),
                result.Result
            );
        }

        [Theory]
        [InlineData("for x in xs:\n\n  call()\n\n  return\n\n")]
        [InlineData("for x in xs:\n\n  call()\n\n\n  return\n\n")]
        [InlineData("for x in xs:  \n \n   \n  call() \n  \n   \n  return\n\n")]
        public void EmptyLines(string inputString)
        {
            var input = ParserInput.Create(inputString);
            var result = Statements.Statement(input);

            Assert.True(result.IsResult);
            Assert.Equal(
                new For(
                    new List<Expr>() { new IdentifierLiteral("x") },
                    new List<Expr>() { new IdentifierLiteral("xs") },
                    new Suite(new List<Stmt>()
                    {
                        new ExpressionStmt(
                            new List<Expr>()
                            {
                                new Call(
                                    new IdentifierLiteral("call"),
                                    Maybe.Nothing<IReadOnlyList<Expr>>(),
                                    Maybe.Nothing<IReadOnlyList<KeywordArgument>>(),
                                    Maybe.Nothing<Expr>(),
                                    Maybe.Nothing<Expr>()
                                )
                            }
                        ),
                        new Return (Maybe.Nothing < IReadOnlyList < Expr > >())
                    }),
                    Maybe.Nothing<Suite>()
                ),
                result.Result
            );
        }

        [Theory]
        [InlineData("for x in xs:\n  pass\n\n\nelse:\n  pass")]
        [InlineData("for x in xs:\n  pass\n\nelse:\n  pass")]
        [InlineData("for x in xs:\n  pass\nelse:\n  pass")]
        public void ElseBranch(string inputString)
        {
            var input = ParserInput.Create(inputString);
            var result = Statements.Statement(input);

            Assert.True(result.IsResult);
            Assert.Equal(
                new For(
                    new List<Expr>() { new IdentifierLiteral("x") },
                    new List<Expr>() { new IdentifierLiteral("xs") },
                    new Suite(new List<Stmt>()
                    {
                        new Pass ()
                    }),
                    Maybe.FromValue(
                        new Suite(new List<Stmt>()
                        {
                            new Pass()
                        })
                    )
                ),
                result.Result
            );
        }

        [Fact]
        public void ComplexHead()
        {
            var input = ParserInput.Create("for x, y in xs.list:\n  pass\n\n");
            var result = Statements.Statement(input);

            Assert.True(result.IsResult);
            Assert.Equal(
                new For(
                    new List<Expr>() { new IdentifierLiteral("x"), new IdentifierLiteral("y") },
                    new List<Expr>() { new AttributeRef(new IdentifierLiteral("xs"), new IdentifierLiteral("list")) },
                    new Suite(new List<Stmt>()
                    {
                        new Pass()
                    }),
                    Maybe.Nothing<Suite>()
                ),
                result.Result
            );
        }

        [Theory]
        [InlineData("for x in xs:\n  pass\n  else:\n  pass")]
        [InlineData("for x in xs:\n  pass\n else:\n  pass")]
        [InlineData("  for x in xs:\n  pass\nelse:\n  pass")]
        public void ElseBranchAtDifferentIndentationFails(string inputString)
        {
            var input = ParserInput.Create(inputString);
            var result = Statements.Statement(input);

            Assert.True(result.IsError);
        }
    }
}
