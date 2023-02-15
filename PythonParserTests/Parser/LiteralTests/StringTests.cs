
using ParsecCore.Input;
using PythonParser.Parser;

namespace PythonParserTests.Parser.LiteralTests
{
    public class StringTests
    {
        [Theory]
        [InlineData("\"\"", "")]
        [InlineData("\"a\"", "a")]
        [InlineData("\"Hello World\"", "Hello World")]
        [InlineData("\"Hello\\nWorld\"", "Hello\nWorld")]
        public void ParsedCorrectly(string inputString, string expected)
        {
            var input = ParserInput.Create(inputString);
            var result = Literals.String(input);

            Assert.True(result.IsResult);
            Assert.Equal(expected, result.Result.Value);
        }

        [Theory]
        [InlineData("\"Unended string")]
        [InlineData("\"wrong escape \\ \"")]
        public void ParsingFails(string inputString)
        {
            var input = ParserInput.Create(inputString);
            var result = Literals.String(input);

            Assert.True(result.IsError);
        }
    }
}
