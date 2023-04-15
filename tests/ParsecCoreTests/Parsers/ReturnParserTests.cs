using ParsecCore;
using Xunit;

namespace ParsecCoreTests
{
    public class ReturnParserTests
    {
        [Fact]
        public void ValueReturned()
        {
            var input = ParserInput.Create("Do you know the tragedy of...");
            var parser = Parsers.Return<int, char>(100);

            var result = parser(input);

            Assert.True(result.IsResult);
            Assert.Equal(100, result.Result);
        }

        [Fact]
        public void InputNotConsumed()
        {
            var input = ParserInput.Create("not just the men, but the women and the children, too");
            var parser = Parsers.Return<int, char>(100);

            var _ = parser(input);

            Assert.Equal(0, input.Offset);
        }
    }
}
