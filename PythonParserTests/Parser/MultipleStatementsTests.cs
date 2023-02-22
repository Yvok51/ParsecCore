using ParsecCore.Input;
using ParsecCore.MaybeNS;
using PythonParser.Parser;
using PythonParser.Structures;

namespace PythonParserTests.Parser
{
    public class MultipleStatementsTests
    {
        [Fact]
        public void ComplexFunctionDefinition()
        {
            var inputString =
@"def sign(num):
    if num > 0:
        return 1
    elif num == 0:
        return 0
    else:
        return -1
";
            var input = ParserInput.Create(inputString);
            var result = Statements.Statement(input);

            Assert.True(result.IsResult);
            Assert.Equal(
                new Function(
                    new IdentifierLiteral("sign"),
                    new List<IdentifierLiteral>() { new IdentifierLiteral("num") },
                    Array.Empty<(IdentifierLiteral, Expr)>(),
                    new Suite(new List<Stmt>()
                    {
                        new If(
                            new Binary(new IdentifierLiteral("num"), BinaryOperator.GT, new IntegerLiteral(0)),
                            new Suite(new List<Stmt>()
                            {
                                new Suite(new List<Stmt>()
                                {
                                    new Return(Maybe.FromValue<IReadOnlyList<Expr>>(new List<Expr>() { new IntegerLiteral(1) }))
                                })
                            }),
                            new List<(Expr, Suite)>()
                            {
                                (
                                    new Binary(new IdentifierLiteral("num"), BinaryOperator.Equal, new IntegerLiteral(0)),
                                    new Suite(new List<Stmt>()
                                    {
                                        new Suite(new List<Stmt>()
                                        {
                                            new Return(Maybe.FromValue<IReadOnlyList<Expr>>(new List<Expr>() { new IntegerLiteral(0) }))
                                        })
                                    })
                                )
                            },
                            Maybe.FromValue(new Suite(new List<Stmt>()
                            {
                                new Suite(new List<Stmt>()
                                    {
                                        new Return(Maybe.FromValue<IReadOnlyList<Expr>>(new List<Expr>() { new IntegerLiteral(-1) }))
                                    })
                            }))
                        )
                    })
                ),
                result.Result
            );
        }

        [Fact]
        public void NestedCall()
        {
            var inputString = "print(sign(0))";
            var input = ParserInput.Create(inputString);
            var result = Statements.Statement(input);

            Assert.True(result.IsResult);
            Assert.Equal(
                new Suite(new List<Stmt>()
                {
                    new ExpressionStmt(new List<Expr>()
                    {
                        new Call(
                            new IdentifierLiteral("print"),
                            Maybe.FromValue<IReadOnlyList<Expr>>(new List<Expr>()
                            {
                                new Call(
                                    new IdentifierLiteral("sign"),
                                    Maybe.FromValue<IReadOnlyList<Expr>>(new List<Expr>()
                                    {
                                        new IntegerLiteral(0)
                                    }),
                                    Maybe.Nothing<IReadOnlyList<KeywordArgument>>(),
                                    Maybe.Nothing<Expr>(),
                                    Maybe.Nothing<Expr>()
                                )
                            }),
                            Maybe.Nothing<IReadOnlyList<KeywordArgument>>(),
                            Maybe.Nothing<Expr>(),
                            Maybe.Nothing<Expr>()
                        )
                    })
                }),
                result.Result);
        }

        [Fact]
        public void FunctionDefinitionAndUsage()
        {
            var inputString =
@"

def sign(num):

    if num > 0:
        return 1
    elif num == 0:
        return 0
    else:
        return -1


print(sign(0))
";
            var input = ParserInput.Create(inputString);
            var result = TopLevelParser.PythonFile(input);

            Assert.True(result.IsResult);
            Assert.Equal(
                new List<Stmt>()
                {
                    new Function(
                        new IdentifierLiteral("sign"),
                        new List<IdentifierLiteral>() { new IdentifierLiteral("num") },
                        Array.Empty<(IdentifierLiteral, Expr)>(),
                        new Suite(new List<Stmt>()
                        {
                            new If(
                                new Binary(new IdentifierLiteral("num"), BinaryOperator.GT, new IntegerLiteral(0)),
                                new Suite(new List<Stmt>()
                                {
                                    new Suite(new List<Stmt>()
                                    {
                                        new Return(Maybe.FromValue<IReadOnlyList<Expr>>(new List<Expr>() { new IntegerLiteral(1) }))
                                    })
                                }),
                                new List<(Expr, Suite)>()
                                {
                                    (
                                        new Binary(new IdentifierLiteral("num"), BinaryOperator.Equal, new IntegerLiteral(0)),
                                        new Suite(new List<Stmt>()
                                        {
                                            new Suite(new List<Stmt>()
                                            {
                                                new Return(Maybe.FromValue<IReadOnlyList<Expr>>(new List<Expr>() { new IntegerLiteral(0) }))
                                            })
                                        })
                                    )
                                },
                                Maybe.FromValue(new Suite(new List<Stmt>()
                                {
                                    new Suite(new List<Stmt>()
                                        {
                                            new Return(Maybe.FromValue<IReadOnlyList<Expr>>(new List<Expr>() { new IntegerLiteral(-1) }))
                                        })
                                }))
                            )
                        })
                    ),
                    new Suite(new List<Stmt>()
                    {
                        new ExpressionStmt(new List<Expr>()
                        {
                            new Call(
                                new IdentifierLiteral("print"),
                                Maybe.FromValue<IReadOnlyList<Expr>>(new List<Expr>()
                                {
                                    new Call(
                                        new IdentifierLiteral("sign"),
                                        Maybe.FromValue<IReadOnlyList<Expr>>(new List<Expr>()
                                        {
                                            new IntegerLiteral(0)
                                        }),
                                        Maybe.Nothing<IReadOnlyList<KeywordArgument>>(),
                                        Maybe.Nothing<Expr>(),
                                        Maybe.Nothing<Expr>()
                                    )
                                }),
                                Maybe.Nothing<IReadOnlyList<KeywordArgument>>(),
                                Maybe.Nothing<Expr>(),
                                Maybe.Nothing<Expr>()
                            )
                        })
                    })
                },
                result.Result
            );
        }
    }
}
