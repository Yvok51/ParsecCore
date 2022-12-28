﻿using ParsecCore;
using ParsecCore.Indentation;
using ParsecCore.Input;
using ParsecCore.MaybeNS;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ParsecCoreTests.Indentation
{
    public class ManyIndentBlockTests
    {
        public static Parser<string, char> Word = Parsers.Letter.Many1();

        [Theory]
        [InlineData("def")]
        [InlineData("def   ")]
        [InlineData("def \t  ")]
        [InlineData("def\n  \n  ")]
        public void ManyIndentBlockParsesZeroItems(string inputStr)
        {
            var items = Array.Empty<string>();
            var input = ParserInput.Create(inputStr);
            var parser = from list in ParsecCore.Indentation.Indentation.IndentBlockMany(
                Parsers.Spaces, Parsers.String("def"), Maybe.Nothing<IndentLevel>(), (head, list) => list, Word)
                         select list;
            var res = parser(input);

            Assert.True(res.IsResult);
            Assert.True(Enumerable.SequenceEqual(items, res.Result));
        }

        [Theory]
        [InlineData("def\n hey\n hello\n hi", "hey hello hi")]
        [InlineData("def\n  hey\n  hello\n  hi", "hey hello hi")]
        [InlineData("def\n\they\n\thello\n\thi", "hey hello hi")]
        public void ManyIndentBlockParsesSameIndentedItems(string inputStr, string parsedItems)
        {
            var items = parsedItems.Split(' ');
            var input = ParserInput.Create(inputStr);
            var parser = from list in ParsecCore.Indentation.Indentation.IndentBlockMany(
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
        public void ManyIndentBlockRejectsDifferentlyIndentedItems(string inputStr)
        {
            var input = ParserInput.Create(inputStr);
            var parser = from list in ParsecCore.Indentation.Indentation.IndentBlockMany(
                Parsers.Spaces, Parsers.String("def"), Maybe.Nothing<IndentLevel>(), (head, list) => list, Word)
                         select list;
            var res = parser(input);

            Assert.True(res.IsError);
        }

        [Fact]
        public void ManyIndentBlockParsesSpecifiedIndentation()
        {
            var items = new List<string>() { "hey", "hello", "hi" };
            var input = ParserInput.Create("def\n  hey\n  hello\n  hi");
            var parser = from list in ParsecCore.Indentation.Indentation.IndentBlockMany(
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
        public void ManyIndentBlockRejectsNonSpecifiedIndentation(string inputStr)
        {
            var input = ParserInput.Create(inputStr);
            var parser = from list in ParsecCore.Indentation.Indentation.IndentBlockMany(
                Parsers.Spaces, Parsers.String("def"), Maybe.FromValue((IndentLevel)3), (head, list) => list, Word)
                         select list;
            var res = parser(input);

            Assert.True(res.IsError);
        }
    }
}
