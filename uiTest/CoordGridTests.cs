using System;
using System.Linq;
using ui;
using Xunit;

namespace uiTest
{
    public class CoordGridTests
    {
        [Theory]
        [InlineData(0, 0)]
        [InlineData(1, 0)]
        [InlineData(0, 1)]
        [InlineData(1, 1)]
        [InlineData(2, 1)]
        [InlineData(1, 2)]
        [InlineData(2, 2)]
        public void AllCoordsWorks(int colCount, int rowCount)
        {
            var actual = new CoordGrid<int>(colCount, rowCount).AllCoords().ToArray();
            for (var ixCol = 0; ixCol < colCount; ixCol++)
            {
                for (var ixRow = 0; ixRow < rowCount; ixRow++)
                    Assert.Contains(CellCoord.Create(ixCol, ixRow), actual);
            }
        }

        [Fact]
        public void IndexerWorks()
        {
            var subject = new CoordGrid<char?>(1, 2);
            var good = CellCoord.Create(0, 1);
            var bad = CellCoord.Create(1, 0);
            Assert.Null(subject[good]);
            Assert.Throws<IndexOutOfRangeException>(() => subject[bad]);
            subject[good] = 'a';
            Assert.Equal('a', subject[good]);
            Assert.Throws<IndexOutOfRangeException>(() => subject[bad] = 'b');
        }
    }
}