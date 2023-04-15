using JSONtoXML;
using ParsecCore;
using Xunit;

namespace JSONtoXMLTests.Parsers
{
    public class BoolValueTests
    {
        [Fact]
        public void TrueParsed()
        {
            var input = ParserInput.Create("true");
            var result = JSONParsers.BoolValue(input);

            Assert.True(result.IsResult);
            Assert.Equal(new BoolValue(true), result.Result);
        }

        [Fact]
        public void FalseParsed()
        {
            var input = ParserInput.Create("false");
            var result = JSONParsers.BoolValue(input);

            Assert.True(result.IsResult);
            Assert.Equal(new BoolValue(false), result.Result);
        }

        [Fact]
        public void BoolParseFails()
        {
            var input = ParserInput.Create("False");
            var result = JSONParsers.BoolValue(input);

            Assert.True(result.IsError);
        }
    }
}
