using ParsecCore;
using ParsecCore.Permutations;
using Xunit;

namespace ParsecCoreTests.Permutations
{
    public class PermutationTests
    {
        [Theory]
        [InlineData("a", 'a')]
        [InlineData("g", 'g')]
        [InlineData("7", '7')]
        public void ParsingSingleMember(string inputString, char toParse)
        {
            var parser = Permutation.NewPermutation(Parsers.Char(toParse)).GetParser();
            var input = ParserInput.Create(inputString);

            IResult<char, char> result = parser(input);

            Assert.True(result.IsResult);
            Assert.Equal(toParse, result.Result);
        }

        [Theory]
        [InlineData("abcdefg", 'a')]
        [InlineData("good morning, Vietnam!", 'g')]
        [InlineData("7 sins", '7')]
        public void ParsingSingleMemberFromLongerInput(string inputString, char toParse)
        {
            var parser = Permutation.NewPermutation(Parsers.Char(toParse)).GetParser();
            var input = ParserInput.Create(inputString);

            IResult<char, char> result = parser(input);

            Assert.True(result.IsResult);
            Assert.Equal(toParse, result.Result);
        }

        [Theory]
        [InlineData("abc")]
        [InlineData("acb")]
        [InlineData("bac")]
        [InlineData("bca")]
        [InlineData("cab")]
        [InlineData("cba")]
        public void ParsingThreeMembers(string inputString)
        {
            var parser = Permutation.NewPermutation(Parsers.Char('a'))
                .Add(Parsers.Char('b'))
                .Add(Parsers.Char('c'), (pair, c) => (pair.Item1, pair.Item2, c))
                .GetParser();
            var input = ParserInput.Create(inputString);

            IResult<(char, char, char), char> result = parser(input);

            Assert.True(result.IsResult);
            Assert.Equal(('a', 'b', 'c'), result.Result);
        }

        [Theory]
        [InlineData("abc")]
        [InlineData("acb")]
        [InlineData("bac")]
        [InlineData("bca")]
        [InlineData("cab")]
        [InlineData("cba")]
        [InlineData("ca")]
        [InlineData("ac")]
        public void ParsingThreeMembersWithOptional(string inputString)
        {
            bool optionalNotInInput = inputString.Length < 3;
            var parser = Permutation.NewPermutation(Parsers.Char('a'))
                .AddOptional(Parsers.Char('b'), 'd')
                .Add(Parsers.Char('c'), (pair, c) => (pair.Item1, pair.Item2, c))
                .GetParser();
            var input = ParserInput.Create(inputString);

            IResult<(char, char, char), char> result = parser(input);

            Assert.True(result.IsResult);
            if (optionalNotInInput)
            {
                Assert.Equal(('a', 'd', 'c'), result.Result);
            }
            else
            {
                Assert.Equal(('a', 'b', 'c'), result.Result);
            }
        }

        [Theory]
        [InlineData("while  whence")]
        [InlineData("whence  while")]
        public void PermutingLongerParsers(string strInput)
        {
            var parser = Permutation.NewPermutation(Parsers.Symbol("while").Try())
                .Add(Parsers.Symbol("whence").Try())
                .GetParser();
            var input = ParserInput.Create(strInput);

            IResult<(string, string), char> result = parser(input);

            Assert.True(result.IsResult);
            Assert.Equal(("while", "whence"), result.Result);
        }

        [Fact]
        public void CommonPrefixFailsWithoutLookahead()
        {
            var parser = Permutation.NewPermutation(Parsers.Symbol("while"))
                .Add(Parsers.Symbol("whence"))
                .GetParser();
            var input1 = ParserInput.Create("whence  while");
            var input2 = ParserInput.Create("while whence");

            IResult<(string, string), char> result1 = parser(input1);
            IResult<(string, string), char> result2 = parser(input2);

            Assert.True(result1.IsError || result2.IsError);
        }

        [Theory]
        [InlineData("while  whence")]
        [InlineData("whence  while")]
        [InlineData("while")]
        public void PermutingLongerParsersWithOptional(string strInput)
        {
            var parser = Permutation.NewPermutation(Parsers.Symbol("while").Try())
                .AddOptional(Parsers.Symbol("whence").Try(), "whence")
                .GetParser();
            var input = ParserInput.Create(strInput);

            IResult<(string, string), char> result = parser(input);

            Assert.True(result.IsResult);
            Assert.Equal(("while", "whence"), result.Result);
        }
    }
}
