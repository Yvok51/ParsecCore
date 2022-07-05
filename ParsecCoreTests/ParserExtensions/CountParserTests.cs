using ParsecCore;
using ParsecCore.Input;
using System.Collections.Generic;
using Xunit;

namespace ParsecCoreTests
{
    public class CountParserTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-45)]
        public void LessThenOneActsAsReturn(int count)
        {
            var input = ParserInput.Create("abc");
            var parser = Parsers.Digit.Count(count);

            var result = parser(input);

            Assert.True(result.HasRight);
            var parsedList = new List<char>(result.Right);
            Assert.Empty(parsedList);
        }

        [Fact]
        public void CountParsed()
        {
            var input = ParserInput.Create("12abc");
            var parser = Parsers.Digit.Count(2);
            var expected = new List<char>();
            expected.Add('1');
            expected.Add('2');

            var result = parser(input);

            Assert.True(result.HasRight);
            var parsedList = new List<char>(result.Right);

            Assert.Equal(expected.Count, parsedList.Count);
            for (int i = 0; i < parsedList.Count; i++)
            {
                Assert.Equal(expected[i], parsedList[i]);
            }
        }

        [Fact]
        public void NoMoreThanCountParsed()
        {
            var input = ParserInput.Create("12345bc");
            var parser = Parsers.Digit.Count(2);
            var expected = new List<char>();
            expected.Add('1');
            expected.Add('2');

            var result = parser(input);

            Assert.True(result.HasRight);
            var parsedList = new List<char>(result.Right);

            Assert.Equal(expected.Count, parsedList.Count);
            for (int i = 0; i < parsedList.Count; i++)
            {
                Assert.Equal(expected[i], parsedList[i]);
            }
        }

        [Fact]
        public void NotEnoughCount()
        {
            var input = ParserInput.Create("123bc");
            var parser = Parsers.Digit.Count(6);

            var result = parser(input);

            Assert.True(result.HasLeft);
        }
    }
}
