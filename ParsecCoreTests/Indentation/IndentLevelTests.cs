using ParsecCore.Indentation;
using Xunit;

namespace ParsecCoreTests.Indentation
{
    public class IndentLevelTests
    {
        [Theory]
        [InlineData(7)]
        [InlineData(0)]
        [InlineData(-1)]
        public void IndentLevelsEqual(int level)
        {
            var left = new IndentLevel(level);
            var right = new IndentLevel(level);
            Assert.True(left == right);
            Assert.True(left.Equals(right) && right.Equals(left));
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(1, 2)]
        [InlineData(1, 0)]
        public void IndentLevelsNotEqual(int leftLvl, int rightLvl)
        {
            var left = new IndentLevel(leftLvl);
            var right = new IndentLevel(rightLvl);
            Assert.True(left != right);
            Assert.True(!left.Equals(right) && !right.Equals(left));
        }

        [Theory]
        [InlineData(1, 0)]
        [InlineData(2, 1)]
        [InlineData(2, 0)]
        [InlineData(3, -1)]
        public void IndentLevelGreaterThan(int leftLvl, int rightLvl)
        {
            var left = new IndentLevel(leftLvl);
            var right = new IndentLevel(rightLvl);
            Assert.True(left > right);
        }

        [Theory]
        [InlineData(1, 0)]
        [InlineData(2, 1)]
        [InlineData(2, 0)]
        [InlineData(3, -1)]
        [InlineData(3, 3)]
        [InlineData(0, 0)]
        public void IndentLevelGreaterOrEqualTo(int leftLvl, int rightLvl)
        {
            var left = new IndentLevel(leftLvl);
            var right = new IndentLevel(rightLvl);
            Assert.True(left >= right);
        }
    }
}
