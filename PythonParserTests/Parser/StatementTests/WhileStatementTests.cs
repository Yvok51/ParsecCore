using ParsecCore.Input;
using ParsecCore.MaybeNS;
using PythonParser.Parser;
using PythonParser.Structures;

namespace PythonParserTests.Parser.StatementTests
{
    public class WhileStatementTests
    {
        [Theory]
        [InlineData("while x:\n  pass\n\n")]
        [InlineData("while x:\n  pass\n")]
        public void SimpleWhileStatement(string inputString)
        {
            var input = ParserInput.Create(inputString);
            var result = Statements.Statement(input);

            Assert.True(result.IsResult);
            Assert.Equal(
                new While(
                    new IdentifierLiteral("x"),
                    new Suite(new List<Stmt>()
                    {
                        new Suite( new List<Stmt>() {new Pass() })
                    }),
                    Maybe.Nothing<Suite>()
                ),
                result.Result
            );
        }

        [Theory]
        [InlineData("while x:\n  call()\n  return\n")]
        [InlineData("while x:\n  call()\n  return\n\n")]
        public void MultipleStatements(string inputString)
        {
            var input = ParserInput.Create(inputString);
            var result = Statements.Statement(input);

            Assert.True(result.IsResult);
            Assert.Equal(
                new While(
                    new IdentifierLiteral("x"),
                    new Suite(new List<Stmt>()
                    {
                        new Suite(
                            new List<Stmt>()
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
                                )
                            }
                        ),
                        new Suite( new List<Stmt>() { new Return(Maybe.Nothing<IReadOnlyList<Expr>>()) })
                    }),
                    Maybe.Nothing<Suite>()
                ),
                result.Result
            );
        }

        [Theory]
        [InlineData("while x:\n  call()\n\n  return\n")]
        [InlineData("while x:\n\n  call()\n\n  return\n")]
        [InlineData("while x:  \n\n \n  \n   \n  call()\n \n  \n   \n  return\n\n")]
        public void EmptyLines(string inputString)
        {
            var input = ParserInput.Create(inputString);
            var result = Statements.Statement(input);

            Assert.True(result.IsResult);
            Assert.Equal(
                new While(
                    new IdentifierLiteral("x"),
                    new Suite(new List<Stmt>()
                    {
                        new Suite(
                            new List<Stmt>()
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
                                )
                            }
                        ),
                        new Suite( new List<Stmt>() { new Return(Maybe.Nothing<IReadOnlyList<Expr>>()) })
                    }),
                    Maybe.Nothing<Suite>()
                ),
                result.Result
            );
        }

        [Fact]
        public void LongerCondition()
        {
            var input = ParserInput.Create("while x and y:\n  pass\n");
            var result = Statements.Statement(input);

            Assert.True(result.IsResult);
            Assert.Equal(
                new While(
                    new Binary(new IdentifierLiteral("x"), BinaryOperator.And, new IdentifierLiteral("y")),
                    new Suite(new List<Stmt>()
                    {
                        new Suite( new List<Stmt>() {new Pass() })
                    }),
                    Maybe.Nothing<Suite>()
                ),
                result.Result
            );
        }

        [Fact]
        public void ElseBranch()
        {
            var input = ParserInput.Create("while x:\n  pass\nelse:\n  pass");
            var result = Statements.Statement(input);

            Assert.True(result.IsResult);
            Assert.Equal(
                new While(
                    new IdentifierLiteral("x"),
                    new Suite(new List<Stmt>()
                    {
                        new Suite( new List<Stmt>() { new Pass() })
                    }),
                    Maybe.FromValue(
                        new Suite(new List<Stmt>()
                        {
                            new Suite(new List<Stmt>() { new Pass() })
                        })
                    )
                ),
                result.Result
            );
        }
    }
}
