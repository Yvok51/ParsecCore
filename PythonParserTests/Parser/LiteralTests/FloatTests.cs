using PythonParser.Parser;
using ParsecCore;
using ParsecCore.Input;

namespace PythonParserTests.Parser.LiteralTests
{
    public class FloatTests
    {
        [Theory]
        [InlineData("3.14", 3.14)]
        [InlineData("10.", 10.0)]
        [InlineData(".001", .001)]
        [InlineData("1e20", 1e20)]
        [InlineData("3.14e-5", 3.14e-5)]
        [InlineData("0e0", 0e0)]
        [InlineData("3.14_15_93", 3.14_15_93)]
        public void ParsedCorrectly(string inputString, double expected)
        {
            var input = ParserInput.Create(inputString);
            var result = Literals.Float(input);

            Assert.True(result.IsResult);
            Assert.Equal(expected, result.Result);
        }

        [Theory]
        [InlineData(".")]
        [InlineData("0")]
        [InlineData("10")]
        [InlineData("12.5.")]
        public void ParsingFails(string inputString)
        {
            var input = ParserInput.Create(inputString);
            var result = Literals.Float(input);

            Assert.True(result.IsError);
        }
    }
}
