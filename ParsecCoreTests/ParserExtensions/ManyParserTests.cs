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

            for (int i = 0; i < expected.Count; i++)
            {
                Assert.Equal(expected[i], result.Right[i]);
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

            for (int i = 0; i < expected.Count; i++)
            {
                Assert.Equal(expected[i], result.Right[i]);
            }
        }

        [Fact]
        public void ParsedTillTheEnd()
        {
            var input = ParserInput.Create("123");
            var parser = Parsers.Digit.Many();
            var expected = new List<char>();
            expected.Add('1');
            expected.Add('2');
            expected.Add('3');

            var result = parser(input);

            Assert.True(result.HasRight);
            Assert.Equal(expected.Count, result.Right.Length);

            for (int i = 0; i < expected.Count; i++)
            {
                Assert.Equal(expected[i], result.Right[i]);
            }
        }

        [Fact]
        public void ParsedTillTheEndString()
        {
            var input = ParserInput.Create("HelloHello");
            var parser = Parsers.String("Hello").Many();
            var expected = new List<string>();
            expected.Add("Hello");
            expected.Add("Hello");

            var result = parser(input);

            Assert.True(result.HasRight);
            Assert.Equal(expected.Count, result.Right.Count);

            for (int i = 0; i < expected.Count; i++)
            {
                Assert.Equal(expected[i], result.Right[i]);
            }
        }

        [Fact]
        public void ParsedTillTheEndChar()
        {
            var input = ParserInput.Create("aaa");
            var parser = Parsers.Char('a').Many();
            var expected = new List<char>();
            expected.Add('a');
            expected.Add('a');
            expected.Add('a');

            var result = parser(input);

            Assert.True(result.HasRight);
            Assert.Equal(expected.Count, result.Right.Length);

            for (int i = 0; i < expected.Count; i++)
            {
                Assert.Equal(expected[i], result.Right[i]);
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
