using JSONtoXML;
using ParsecCore.Input;
using Xunit;

namespace JSONtoXMLTests.Parsers
{
    public class NumberValueTests
    {
        [Fact]
        public void IntegerParsed()
        {
            var input = ParserInput.Create("13");
            var result = JSONParsers.NumberValue(input);

            Assert.True(result.HasRight);
            Assert.Equal(new NumberValue(13), result.Right);
        }

        [Fact]
        public void NegativeIntegerParsed()
        {
            var input = ParserInput.Create("-13");
            var result = JSONParsers.NumberValue(input);

            Assert.True(result.HasRight);
            Assert.Equal(new NumberValue(-13), result.Right);
        }

        [Fact]
        public void FractionalParsed()
        {
            var input = ParserInput.Create("2.25");
            var result = JSONParsers.NumberValue(input);

            Assert.True(result.HasRight);
            Assert.Equal(new NumberValue(2.25), result.Right);
        }

        [Fact]
        public void ExponentialParsed()
        {
            var input = ParserInput.Create("2e6");
            var result = JSONParsers.NumberValue(input);

            Assert.True(result.HasRight);
            Assert.Equal(new NumberValue(2_000_000), result.Right);
        }

        [Fact]
        public void CombinedNumberParsed()
        {
            var input = ParserInput.Create("-2.5e-2");
            var result = JSONParsers.NumberValue(input);

            Assert.True(result.HasRight);
            Assert.Equal(new NumberValue(-0.025), result.Right);
        }

        [Fact]
        public void NumberParseFails()
        {
            var input = ParserInput.Create("abc");
            var result = JSONParsers.NumberValue(input);

            Assert.True(result.HasLeft);
        }
    }
}
