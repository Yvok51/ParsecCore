using ParsecCore;
using ParsecCore.EitherNS;
using ParsecCore.Input;
using Xunit;

namespace ParsecCoreTests.Permutations
{
    public class PermutationsTests
    {
        [Theory]
        [InlineData("a", "a")]
        [InlineData("g", "g")]
        [InlineData("7", "7")]
        public void ParsingSingleMember(string inputString, string toParse)
        {
            var parser = Parsers.String(toParse);
            var input = ParserInput.Create(inputString);

            IEither<ParseError, string> result = parser(input);

            Assert.True(result.IsResult);
            Assert.Equal(toParse, result.Result);
        }
    }
}
