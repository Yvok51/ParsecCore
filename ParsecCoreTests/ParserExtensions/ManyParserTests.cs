using System.Collections.Generic;
using Xunit;

using ParsecCore;
using ParsecCore.Input;

namespace ParsecCoreTests
{
    public class ManyParserTests
    {
        [Fact]
        public void NoMatchSuccessfullyParsed()
        {
            var input = ParserInput.Create("abc");
            var parser = Parsers.Digit.Many();

            var result = parser(input);

            Assert.True(result.HasRight);
            Assert.Equal(0, result.Right.Length);
        }

        [Fact]
        public void OneMatchParsed()
        {
            var input = ParserInput.Create("1abc");
            var parser = Parsers.Digit.Many();
            var expected = new List<char>();
            expected.Add('1');

            var result = parser(input);

            Assert.True(result.HasRight);
            Assert.Equal(expected.Count, result.Right.Length);

            int i = 0;
            foreach (var c in result.Right)
            {
                Assert.Equal(expected[i], c);
                i++;
            }
        }

        [Fact]
        public void ManyMatchesParsed()
        {
            var input = ParserInput.Create("123abc");
            var parser = Parsers.Digit.Many();
            var expected = new List<char>();
            expected.Add('1');
            expected.Add('2');
            expected.Add('3');

            var result = parser(input);

            Assert.True(result.HasRight);
            Assert.Equal(expected.Count, result.Right.Length);

            int i = 0;
            foreach (var c in result.Right)
            {
                Assert.Equal(expected[i], c);
                i++;
            }
        }

        [Fact]
        public void FailsInTheMiddleOfParse()
        {
            var input = ParserInput.Create("helaerg");
            var parser = Parsers.String("hello").Many();

            var result = parser(input);

            Assert.True(result.HasLeft);
        }

        [Fact]
        public void Many1FailsToParseAtLeastOne()
        {
            var input = ParserInput.Create("abc");
            var parser = Parsers.Digit.Many1();

            var result = parser(input);

            Assert.True(result.HasLeft);
        }
    }
}
