using ParsecCore;
using ParsecCore.EitherNS;
using ParsecCore.Input;
using ParsecCore.Permutations;
using Xunit;

namespace ParsecCoreTests.Permutations
{
    public class PermutationsTests
    {
        [Theory]
        [InlineData("a", 'a')]
        [InlineData("g", 'g')]
        [InlineData("7", '7')]
        public void ParsingSingleMember(string inputString, char toParse)
        {
            var parser = Permutation.Permute(Permutation.PermuteParser(Parsers.Char(toParse)), (c) => c);
            var input = ParserInput.Create(inputString);

            IEither<ParseError, char> result = parser(input);

            Assert.True(result.IsResult);
            Assert.Equal(toParse, result.Result);
        }

        [Theory]
        [InlineData("abcdefg", 'a')]
        [InlineData("good morning, Vietnam!", 'g')]
        [InlineData("7 sins", '7')]
        public void ParsingSingleMemberFromLongerInput(string inputString, char toParse)
        {
            var parser = Permutation.Permute(Permutation.PermuteParser(Parsers.Char(toParse)), (c) => c);
            var input = ParserInput.Create(inputString);

            IEither<ParseError, char> result = parser(input);

            Assert.True(result.IsResult);
            Assert.Equal(toParse, result.Result);
        }

        [Theory]
        [InlineData("a", 'a', 'b')]
        [InlineData("g", 'g', 'h')]
        [InlineData("7", '7', '8')]
        public void FinalTransformationFunctionIsAppliedOnSingleMember(string inputString, char toParse, char transformed)
        {
            var parser = Permutation.Permute(Permutation.PermuteParser(Parsers.Char(toParse)), (c) => (char)(c + 1));
            var input = ParserInput.Create(inputString);

            IEither<ParseError, char> result = parser(input);

            Assert.True(result.IsResult);
            Assert.Equal(transformed, result.Result);
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
            var parser = Permutation.Permute(
                Parsers.Char('a').PermuteParser(),
                Parsers.Char('b').PermuteParser(),
                Parsers.Char('c').PermuteParser(),
                (a, b, c) => (a, b, c));
            var input = ParserInput.Create(inputString);

            IEither<ParseError, (char, char, char)> result = parser(input);

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
            var parser = Permutation.Permute(
                Parsers.Char('a').PermuteParser(),
                Parsers.Char('b').OptionalPermuteParser('d'),
                Parsers.Char('c').PermuteParser(),
                (a, b, c) => (a, b, c));
            var input = ParserInput.Create(inputString);

            IEither<ParseError, (char, char, char)> result = parser(input);

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
    }
}
