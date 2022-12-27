using ParsecCore.Indentation;
using Xunit;

namespace ParsecCoreTests.Indentation
{
    public class RelationTests
    {
        [Theory]
        [InlineData(0, 0)]
        [InlineData(1, 1)]
        [InlineData(-1, -1)]
        public void EqualRelationSatisfy(int reference, int actual)
        {
            Assert.True(Relation.EQ.Satisfies((IndentLevel)reference, (IndentLevel)actual));
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(0, 5)]
        [InlineData(1, 2)]
        [InlineData(-1, 0)]
        public void GreaterThanRelationSatisfy(int reference, int actual)
        {
            Assert.True(Relation.GT.Satisfies((IndentLevel)reference, (IndentLevel)actual));
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(0, 5)]
        [InlineData(1, 2)]
        [InlineData(-1, 0)]
        [InlineData(0, 0)]
        [InlineData(1, 1)]
        [InlineData(-1, -1)]
        public void GreaterOrEqualToRelationSatisfy(int reference, int actual)
        {
            Assert.True(Relation.GE.Satisfies((IndentLevel)reference, (IndentLevel)actual));
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(1, 0)]
        [InlineData(1, 1)]
        [InlineData(0, 0)]
        [InlineData(-1, 0)]
        [InlineData(1, -1)]
        [InlineData(-1, -1)]
        public void AnyRelationSatisfy(int reference, int actual)
        {
            Assert.True(Relation.ANY.Satisfies((IndentLevel)reference, (IndentLevel)actual));
        }
    }
}
