using Xunit;
using JSONtoXML;
using ParsecCore.Input;

namespace JSONtoXMLTests.Parsers
{
    public class NullValueTests
    {
        [Fact]
        public void NullValueParsed()
        {
            var input = ParserInput.Create("null");
            var result = JSONParsers.NullValue(input);

            Assert.True(result.HasRight);
            Assert.Equal(new NullValue(), result.Right);
        }

        [Fact]
        public void NullValueParsedWithWhitespace()
        {
            var input = ParserInput.Create(" \t null    \n ");
            var result = JSONParsers.NullValue(input);

            Assert.True(result.HasRight);
            Assert.Equal(new NullValue(), result.Right);
        }

        [Fact]
        public void NullValueFails()
        {
            var input = ParserInput.Create("nuxll");
            var result = JSONParsers.NullValue(input);

            Assert.True(result.HasLeft);
        }
    }
}
