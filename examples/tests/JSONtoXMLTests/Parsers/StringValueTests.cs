using JSONtoXML;
using ParsecCore.Input;
using Xunit;

namespace JSONtoXMLTests.Parsers
{
    public class StringValueTests
    {
        [Fact]
        public void StringParsed()
        {
            var input = ParserInput.Create("\"Ahoj\"");
            var result = JSONParsers.StringValue(input);

            Assert.True(result.IsResult);
            Assert.Equal(new StringValue("Ahoj"), result.Result);
        }

        [Fact]
        public void EscapedCharacter()
        {
            var input = ParserInput.Create(@"""Hello \n Hey""");
            var result = JSONParsers.StringValue(input);

            Assert.True(result.IsResult);
            Assert.Equal(new StringValue("Hello \n Hey"), result.Result);
        }

        [Fact]
        public void HexEncodedCharacter()
        {
            var input = ParserInput.Create("\"\\u0041\"");
            var result = JSONParsers.StringValue(input);

            Assert.True(result.IsResult);
            Assert.Equal(new StringValue("A"), result.Result);
        }

        [Fact]
        public void NoQoutesParseFails()
        {
            var input = ParserInput.Create("abc");
            var result = JSONParsers.StringValue(input);

            Assert.True(result.IsError);
        }

        [Fact]
        public void LineBreakInString()
        {
            var input = ParserInput.Create("ab\nc");
            var result = JSONParsers.StringValue(input);

            Assert.True(result.IsError);
        }
    }
}
