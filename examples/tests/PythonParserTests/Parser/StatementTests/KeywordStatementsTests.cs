using ParsecCore;
using PythonParser.Parser;
using PythonParser.Structures;

namespace PythonParserTests.Parser.StatementTests
{
    public class KeywordStatementsTests
    {
        [Fact]
        public void Pass()
        {
            var input = ParserInput.Create("pass\n");
            var result = Statements.Statement(input);

            Assert.True(result.IsResult);
            Assert.Equal(
                new Pass(),
                result.Result
            );
        }

        [Fact]
        public void NonEmptyPassFails()
        {
            var input = ParserInput.Create("pass 1 + 2\n");
            var result = Statements.Statement(input);

            Assert.True(result.IsError);
        }

        [Fact]
        public void Break()
        {
            var input = ParserInput.Create("break\n");
            var result = Statements.Statement(input);

            Assert.True(result.IsResult);
            Assert.Equal(
                new Break(),
                result.Result
            );
        }

        [Fact]
        public void NonEmptyBreakFails()
        {
            var input = ParserInput.Create("break 1 + 2\n");
            var result = Statements.Statement(input);

            Assert.True(result.IsError);
        }

        [Fact]
        public void Continue()
        {
            var input = ParserInput.Create("continue\n");
            var result = Statements.Statement(input);

            Assert.True(result.IsResult);
            Assert.Equal(
                new Continue(),
                result.Result
            );
        }

        [Fact]
        public void NonEmptyContinueFails()
        {
            var input = ParserInput.Create("continue 1 + 2\n");
            var result = Statements.Statement(input);

            Assert.True(result.IsError);
        }
    }
}
