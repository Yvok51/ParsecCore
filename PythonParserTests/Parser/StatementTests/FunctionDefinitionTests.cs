using ParsecCore.Input;
using ParsecCore.MaybeNS;
using PythonParser.Parser;
using PythonParser.Structures;

namespace PythonParserTests.Parser.StatementTests
{
    public class FunctionDefinitionTests
    {
        [Fact]
        public void DefineFunctionWithNoArguments()
        {
            var input = ParserInput.Create("def func():\n  pass\n");
            var result = Statements.Statement(input);

            Assert.True(result.IsResult);
            Assert.Equal(
                new Function(
                    new IdentifierLiteral("func"),
                    Array.Empty<IdentifierLiteral>(),
                    Array.Empty<(IdentifierLiteral, Expr)>(),
                    new Suite(new List<Stmt>() { new Suite(new List<Stmt>() { new Pass() })})
                ),
                result.Result
            );
        }

        [Fact]
        public void DefineFunctionWithArguments()
        {
            var input = ParserInput.Create("def func(a, b, c):\n  pass\n");
            var result = Statements.Statement(input);

            Assert.True(result.IsResult);
            Assert.Equal(
                new Function(
                    new IdentifierLiteral("func"),
                    new List<IdentifierLiteral>() 
                    {
                        new IdentifierLiteral("a"),
                        new IdentifierLiteral("b"),
                        new IdentifierLiteral("c")
                    },
                    Array.Empty<(IdentifierLiteral, Expr)>(),
                    new Suite(new List<Stmt>() { new Suite(new List<Stmt>() { new Pass() }) })
                ),
                result.Result
            );
        }

        [Fact]
        public void MultipleStatementsInBody()
        {
            var input = ParserInput.Create("def func():\n  x = 1\n  return\n");
            var result = Statements.Statement(input);

            Assert.True(result.IsResult);
            Assert.Equal(
                new Function(
                    new IdentifierLiteral("func"),
                    Array.Empty<IdentifierLiteral>(),
                    Array.Empty<(IdentifierLiteral, Expr)>(),
                    new Suite(new List<Stmt>() 
                    {
                        new Suite(new List<Stmt>() {new Assignment(
                            new List<List<Expr>>() { new List<Expr>() { new IdentifierLiteral("x") } },
                            new List<Expr>() { new IntegerLiteral(1)}
                        )}),
                        new Suite(new List<Stmt>() {new Return(Maybe.Nothing<IReadOnlyList<Expr>>())})
                    })
                ),
                result.Result
            );
        }

        [Fact]
        public void MultipleStatementsInBodyWithEmptyLines()
        {
            var input = ParserInput.Create("def func():\n  x = 1\n\n\n \n  \n  return\n");
            var result = Statements.Statement(input);

            Assert.True(result.IsResult);
            Assert.Equal(
                new Function(
                    new IdentifierLiteral("func"),
                    Array.Empty<IdentifierLiteral>(),
                    Array.Empty<(IdentifierLiteral, Expr)>(),
                    new Suite(new List<Stmt>()
                    {
                        new Suite(new List<Stmt>() {new Assignment(
                            new List<List<Expr>>() { new List<Expr>() { new IdentifierLiteral("x") } },
                            new List<Expr>() { new IntegerLiteral(1)}
                        )}),
                        new Suite(new List<Stmt>() {new Return(Maybe.Nothing<IReadOnlyList<Expr>>())})
                    })
                ),
                result.Result
            );
        }
    }
}
