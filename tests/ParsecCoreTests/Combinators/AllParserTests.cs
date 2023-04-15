using ParsecCore;
using System.Collections.Generic;
using Xunit;

namespace ParsecCoreTests
{
    public class AllParserTests
    {
        [Fact]
        public void AllParsersApplied()
        {
            var firstParser = Parsers.String("You underestimate");
            var secondParser = Parsers.String(" ");
            var thirdParser = Parsers.String("my power!");
            var parser = Parsers.All(firstParser, secondParser, thirdParser);
            var input = ParserInput.Create("You underestimate my power!");

            var result = parser(input);

            Assert.True(result.IsResult);
            var resultList = new List<string>(result.Result);

            Assert.Equal("You underestimate", resultList[0]);
            Assert.Equal(" ", resultList[1]);
            Assert.Equal("my power!", resultList[2]);
        }

        [Fact]
        public void ParsingFailsIfAnyParserFails()
        {
            var firstParser = Parsers.String("I ");
            var secondParser = Parsers.String("love ");
            var thirdParser = Parsers.String("sand");
            var parser = Parsers.All(firstParser, secondParser, thirdParser);
            var input = ParserInput.Create("I love democracy");

            var result = parser(input);

            Assert.True(result.IsError);
        }
    }
}
