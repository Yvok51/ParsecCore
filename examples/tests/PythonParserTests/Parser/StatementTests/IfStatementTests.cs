using ParsecCore;
using PythonParser.Parser;
using PythonParser.Structures;

namespace PythonParserTests.Parser.StatementTests
{
    public class IfStatementTests
    {
        [Theory]
        [InlineData("if x:\n  pass\n\n")]
        [InlineData("if x:\n  pass\n")]
        public void SimpleIfStatement(string inputString)
        {
            var input = ParserInput.Create(inputString);
            var result = Statements.Statement(input);

            Assert.True(result.IsResult);
            Assert.Equal(
                new If(
                    new IdentifierLiteral("x"),
                    new Suite(new List<Stmt>()
                    {
                        new Suite( new List<Stmt>() {new Pass() })
                    }),
                    Array.Empty<(Expr, Suite)>(),
                    Maybe.Nothing<Suite>()
                ),
                result.Result
            );
        }

        [Theory]
        [InlineData("if x:\n  call()\n  return\n\n")]
        [InlineData("if x:\n  call()\n  return\n")]
        public void MultipleStatementsInThenBlock(string inputString)
        {
            var input = ParserInput.Create(inputString);
            var result = Statements.Statement(input);

            Assert.True(result.IsResult);
            Assert.Equal(
                new If(
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
                    Array.Empty<(Expr, Suite)>(),
                    Maybe.Nothing<Suite>()
                ),
                result.Result
            );
        }

        [Fact]
        public void LongerCondition()
        {
            var input = ParserInput.Create("if x and y:\n  pass\n");
            var result = Statements.Statement(input);

            Assert.True(result.IsResult);
            Assert.Equal(
                new If(
                    new Binary(new IdentifierLiteral("x"), BinaryOperator.And, new IdentifierLiteral("y")),
                    new Suite(new List<Stmt>()
                    {
                        new Suite( new List<Stmt>() {new Pass() })
                    }),
                    Array.Empty<(Expr, Suite)>(),
                    Maybe.Nothing<Suite>()
                ),
                result.Result
            );
        }

        [Fact]
        public void ElseBranch()
        {
            var input = ParserInput.Create("if x:\n  pass\nelse:\n  pass");
            var result = Statements.Statement(input);

            Assert.True(result.IsResult);
            Assert.Equal(
                new If(
                    new IdentifierLiteral("x"),
                    new Suite(new List<Stmt>()
                    {
                        new Suite( new List<Stmt>() { new Pass() })
                    }),
                    Array.Empty<(Expr, Suite)>(),
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

        [Fact]
        public void ElifBranch()
        {
            var input = ParserInput.Create("if x:\n  pass\nelif y:\n  pass");
            var result = Statements.Statement(input);

            Assert.True(result.IsResult);
            Assert.Equal(
                new If(
                    new IdentifierLiteral("x"),
                    new Suite(new List<Stmt>()
                    {
                        new Suite( new List<Stmt>() { new Pass() })
                    }),
                    new List<(Expr, Suite)>()
                    {
                        (
                            new IdentifierLiteral("y"),
                            new Suite(new List<Stmt>() { new Suite(new List<Stmt>() { new Pass() })})
                        )
                    },
                    Maybe.Nothing<Suite>()
                ),
                result.Result
            );
        }

        [Fact]
        public void MultipleElifBranch()
        {
            var input = ParserInput.Create("if x:\n  pass\nelif y:\n  pass\nelif z:\n  pass");
            var result = Statements.Statement(input);

            Assert.True(result.IsResult);
            Assert.Equal(
                new If(
                    new IdentifierLiteral("x"),
                    new Suite(new List<Stmt>()
                    {
                        new Suite( new List<Stmt>() { new Pass() })
                    }),
                    new List<(Expr, Suite)>()
                    {
                        (
                            new IdentifierLiteral("y"),
                            new Suite(new List<Stmt>() { new Suite(new List<Stmt>() { new Pass() })})
                        ),
                        (
                            new IdentifierLiteral("z"),
                            new Suite(new List<Stmt>() { new Suite(new List<Stmt>() { new Pass() })})
                        )
                    },
                    Maybe.Nothing<Suite>()
                ),
                result.Result
            );
        }

        [Fact]
        public void ElifAndElseBranch()
        {
            var input = ParserInput.Create("if x:\n  pass\nelif y:\n  pass\nelse:\n  pass");
            var result = Statements.Statement(input);

            Assert.True(result.IsResult);
            Assert.Equal(
                new If(
                    new IdentifierLiteral("x"),
                    new Suite(new List<Stmt>()
                    {
                        new Suite( new List<Stmt>() { new Pass() })
                    }),
                    new List<(Expr, Suite)>()
                    {
                        (
                            new IdentifierLiteral("y"),
                            new Suite(new List<Stmt>() { new Suite(new List<Stmt>() { new Pass() })})
                        )
                    },
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

        [Theory]
        [InlineData("if x:\n  pass\n  elif y:\n  pass")]
        [InlineData("if x:\n  pass\n elif y:\n  pass")]
        [InlineData("  if x:\n  pass\nelif y:\n  pass")]
        public void ElifBranchAtDifferentIndentationFails(string inputString)
        {
            var input = ParserInput.Create(inputString);
            var result = Statements.Statement(input);

            Assert.True(result.IsError);
        }

        [Theory]
        [InlineData("if x:\n  pass\n  else:\n  pass")]
        [InlineData("if x:\n  pass\n else:\n  pass")]
        [InlineData("  if x:\n  pass\nelse:\n  pass")]
        public void ElseBranchAtDifferentIndentationFails(string inputString)
        {
            var input = ParserInput.Create(inputString);
            var result = Statements.Statement(input);

            Assert.True(result.IsError);
        }
    }
}
