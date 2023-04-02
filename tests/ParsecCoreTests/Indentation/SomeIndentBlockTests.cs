using ParsecCore.Indentation;
using ParsecCore.Input;
using ParsecCore.MaybeNS;
using ParsecCore;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ParsecCoreTests.Indentation
{
    public class SomeIndentBlockTests
    {
        public static Parser<string, char> Word = Parsers.Letter.Many1();

        [Theory]
        [InlineData("def")]
        [InlineData("def   ")]
        [InlineData("def \t  ")]
        [InlineData("def\n  \n  ")]
        public void SomeIndentBlockRejectsZeroItems(string inputStr)
        {
            var input = ParserInput.Create(inputStr);
            var parser = from list in ParsecCore.Indentation.Indentation.IndentBlockSome(
                Parsers.Spaces, Parsers.String("def"), Maybe.Nothing<IndentLevel>(), (head, list) => list, Word)
                         select list;
            var res = parser(input);

            Assert.True(res.IsError);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("\n  hey\n  hello\n  hi")]
        public void SomeIndentBlockRejectsNoHead(string inputStr)
        {
            var input = ParserInput.Create(inputStr);
            var parser = from list in ParsecCore.Indentation.Indentation.IndentBlockSome(
                Parsers.Spaces, Parsers.String("def"), Maybe.Nothing<IndentLevel>(), (head, list) => list, Word)
                         select list;
            var res = parser(input);

            Assert.True(res.IsError);
        }

        [Theory]
        [InlineData("def\n hey\n hello\n hi", "hey hello hi")]
        [InlineData("def\n  hey\n  hello\n  hi", "hey hello hi")]
        [InlineData(" def\n  hey\n  hello\n  hi", "hey hello hi")]
        [InlineData("def\n\they\n\thello\n\thi", "hey hello hi")]
        public void SomeIndentBlockParsesSameIndentedItems(string inputStr, string parsedItems)
        {
            var items = parsedItems.Split(' ');
            var input = ParserInput.Create(inputStr);
            var parser = from list in ParsecCore.Indentation.Indentation.IndentBlockSome(
                Parsers.Spaces, Parsers.String("def"), Maybe.Nothing<IndentLevel>(), (head, list) => list, Word)
                         select list;
            var res = parser(input);

            Assert.True(res.IsResult);
            Assert.True(Enumerable.SequenceEqual(items, res.Result));
        }

        [Theory]
        [InlineData("def\n hey\n hello\n  hi")]
        [InlineData("def\n  hey\n  hello\n hi")]
        [InlineData("def\n\they\n\thello\n\t hi")]
        public void SomeIndentBlockRejectsDifferentlyIndentedItems(string inputStr)
        {
            var input = ParserInput.Create(inputStr);
            var parser = from list in ParsecCore.Indentation.Indentation.IndentBlockSome(
                Parsers.Spaces, Parsers.String("def"), Maybe.Nothing<IndentLevel>(), (head, list) => list, Word)
                         select list;
            var res = parser(input);

            Assert.True(res.IsError);
        }

        [Fact]
        public void SomeIndentBlockParsesSpecifiedIndentation()
        {
            var items = new List<string>() { "hey", "hello", "hi" };
            var input = ParserInput.Create("def\n  hey\n  hello\n  hi");
            var parser = from list in ParsecCore.Indentation.Indentation.IndentBlockSome(
                Parsers.Spaces, Parsers.String("def"), Maybe.FromValue((IndentLevel)3), (head, list) => list, Word)
                         select list;
            var res = parser(input);

            Assert.True(res.IsResult);
            Assert.True(Enumerable.SequenceEqual(items, res.Result));
        }

        [Theory]
        [InlineData("def\n  hey\n  hello\n hi")]
        [InlineData("def\n hey\n hello\n hi")]
        [InlineData("def\n  hey\n  hello\n   hi")]
        public void SomeIndentBlockRejectsNonSpecifiedIndentation(string inputStr)
        {
            var input = ParserInput.Create(inputStr);
            var parser = from list in ParsecCore.Indentation.Indentation.IndentBlockSome(
                Parsers.Spaces, Parsers.String("def"), Maybe.FromValue((IndentLevel)3), (head, list) => list, Word)
                         select list;
            var res = parser(input);

            Assert.True(res.IsError);
        }
    }
}
