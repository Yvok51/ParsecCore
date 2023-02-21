using ParsecCore.Input;
using PythonParser.Parser;

namespace PythonParserTests.Parser.LiteralTests
{
    public class IdentifierTests
    {
        [Theory]
        [InlineData("a", "a")]
        [InlineData("abc", "abc")]
        [InlineData("ab0c", "ab0c")]
        [InlineData("_a", "_a")]
        [InlineData("a_b_c_", "a_b_c_")]
        public void ParsedCorrectly(string inputString, string expected)
        {
            var input = ParserInput.Create(inputString);
            var result = Literals.Identifier(Control.Lexeme)(input);

            Assert.True(result.IsResult);
            Assert.Equal(expected, result.Result.Name);
        }

        [Theory]
        [InlineData("")]
        [InlineData(".name")]
        [InlineData("-name")]
        [InlineData("0name")]
        public void ParsingFails(string inputString)
        {
            var input = ParserInput.Create(inputString);
            var result = Literals.Identifier(Control.Lexeme)(input);

            Assert.True(result.IsError);
        }
    }
}
