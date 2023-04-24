using ParsecCore;
using ParsecCore.Indentation;
using Xunit;

namespace ParsecCoreTests.Indentation
{
    public class GuardTests
    {
        public static Parser<string, char> Word = Parsers.Letter.Many1();

        [Theory]
        [InlineData("input", 1)]
        [InlineData(" input", 2)]
        [InlineData("  input", 3)]
        [InlineData("   input", 4)]
        [InlineData("    input", 5)]
        [InlineData("\tinput", 5)]
        [InlineData("   \ninput", 1)]
        [InlineData("   \n  input", 3)]
        public void EqualityRelationAcceptsCorrectIndentation(string inputStr, int referenceIndentation)
        {
            var input = ParserInput.Create(inputStr, tabSize: 4);
            var parser = from indent in ParsecCore.Indentation.Indentation.Guard(
                            Parsers.Spaces, Relation.EQ, (IndentLevel)referenceIndentation)
                         select indent;
            var res = parser(input);

            Assert.True(res.IsResult);
            Assert.True(Relation.EQ.Satisfies((IndentLevel)referenceIndentation, res.Result));
        }

        [Theory]
        [InlineData("input", 0)]
        [InlineData(" input", 1)]
        [InlineData("  input", 2)]
        [InlineData("  input", 1)]
        [InlineData("   input", 3)]
        [InlineData("    input", 4)]
        [InlineData("\tinput", 4)]
        [InlineData("   \n input", 1)]
        [InlineData("   \n  input", 2)]
        public void GreaterThanRelationAcceptsCorrectIndentation(string inputStr, int referenceIndentation)
        {
            var input = ParserInput.Create(inputStr, tabSize: 4);
            var parser = from indent in ParsecCore.Indentation.Indentation.Guard(
                            Parsers.Spaces, Relation.GT, (IndentLevel)referenceIndentation)
                         select indent;
            var res = parser(input);

            Assert.True(res.IsResult);
            Assert.True(Relation.GT.Satisfies((IndentLevel)referenceIndentation, res.Result));
        }

        [Theory]
        [InlineData("input", 1)]
        [InlineData(" input", 2)]
        [InlineData(" input", 1)]
        [InlineData("  input", 3)]
        [InlineData("   input", 4)]
        [InlineData("   input", 1)]
        [InlineData("    input", 5)]
        [InlineData("\tinput", 5)]
        [InlineData("   \ninput", 1)]
        [InlineData("   \n  input", 2)]
        public void GreaterEqualToRelationAcceptsCorrectIndentation(string inputStr, int referenceIndentation)
        {
            var input = ParserInput.Create(inputStr, tabSize: 4);
            var parser = from indent in ParsecCore.Indentation.Indentation.Guard(
                            Parsers.Spaces, Relation.GE, (IndentLevel)referenceIndentation)
                         select indent;
            var res = parser(input);

            Assert.True(res.IsResult);
            Assert.True(Relation.GE.Satisfies((IndentLevel)referenceIndentation, res.Result));
        }

        [Theory]
        [InlineData("input", 2)]
        [InlineData(" input", 3)]
        [InlineData("    input", 10)]
        [InlineData("\tinput", 7)]
        [InlineData("   \ninput", 5)]
        [InlineData("   \n  input", 8)]
        public void EqualityRelationRejectsIncorrectIndentation(string inputStr, int referenceIndentation)
        {
            var input = ParserInput.Create(inputStr, tabSize: 4);
            var parser = from indent in ParsecCore.Indentation.Indentation.Guard(
                            Parsers.Spaces, Relation.EQ, (IndentLevel)referenceIndentation)
                         select indent;
            var res = parser(input);

            Assert.True(res.IsError);
        }

        [Theory]
        [InlineData("input", 10)]
        [InlineData(" input", 2)]
        [InlineData("  \ninput", 1)]
        [InlineData("  \n  input", 5)]
        [InlineData("\tinput", 5)]
        public void GreaterThanRelationRejectsIncorrectIndentation(string inputStr, int referenceIndentation)
        {
            var input = ParserInput.Create(inputStr, tabSize: 4);
            var parser = from indent in ParsecCore.Indentation.Indentation.Guard(
                            Parsers.Spaces, Relation.GT, (IndentLevel)referenceIndentation)
                         select indent;
            var res = parser(input);

            Assert.True(res.IsError);
        }

        [Theory]
        [InlineData("input", 2)]
        [InlineData(" input", 5)]
        [InlineData(" input", 3)]
        [InlineData("    input", 10)]
        [InlineData("\tinput", 6)]
        [InlineData("   \ninput", 2)]
        [InlineData("   \n  input", 4)]
        public void GreaterEqualToRelationRejectsIncorrectIndentation(string inputStr, int referenceIndentation)
        {
            var input = ParserInput.Create(inputStr, tabSize: 4);
            var parser = from indent in ParsecCore.Indentation.Indentation.Guard(
                            Parsers.Spaces, Relation.GE, (IndentLevel)referenceIndentation)
                         select indent;
            var res = parser(input);

            Assert.True(res.IsError);
        }
    }
}
