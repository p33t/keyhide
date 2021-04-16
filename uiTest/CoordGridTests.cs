using System;
using ui;
using Xunit;

namespace uiTest
{
    public class CoordGridTests
    {
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