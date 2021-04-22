using System.Linq;
using ui;
using Xunit;

namespace uiTest
{
    public class PathOperationsTests
    {
        [Theory]
        [InlineData(4, 3, "567", false)]
        [InlineData(4, 3, "321", true)]
        [InlineData(4, 4, "4", true)]
        [InlineData(4, 4, "5", false)]
        [InlineData(3, 2, "2x", true)]
        [InlineData(3, 2, "2", true)] // not enough keyString is OK
        [InlineData(1, 3, "21x", true)]
        [InlineData(0, 0, "2xyz", true)]
        public void CoordinateIsAvailableWorks(int candidateCol, int candidateRow, string keyString, bool expAvailable)
        {
            var grid = new CoordGrid<char?>(5, 5);
            /*
            .....
            .....
            .....
            ..123
            ....4
             */
            grid[CellCoord.Create(2, 3)] = '1';
            grid[CellCoord.Create(3, 3)] = '2';
            grid[CellCoord.Create(4, 3)] = '3';
            var last = CellCoord.Create(4, 4);
            grid[last] = '4';
            var actual = PathOperations.CoordIsAvailable(CellCoord.Create(candidateCol, candidateRow), grid, last,
                keyString);
            Assert.Equal(expAvailable, actual);
        }

        [Theory]
        [InlineData(0, "")]
        [InlineData(1, "A")]
        [InlineData(26, "Z")]
        [InlineData(27, "AA")]
        [InlineData(26 * 2, "AZ")]
        [InlineData(26 * 27, "ZZ")]
        public void ColNameWorks(int position, string expected)
        {
            Assert.Equal(expected, PathOperations.ColName(position));
        }

        public static TheoryData<CellCoord[]> TraceFixtures => new()
        {
            new[]
            {
                CellCoord.Create(0, 0)
            },
            new[]
            {
                CellCoord.Create(0, 0),
                CellCoord.Create(1, 1)
            },
            new[]
            {
                CellCoord.Create(0, 0),
                CellCoord.Create(0, 1)
            },
            new[]
            {
                CellCoord.Create(0, 0),
                CellCoord.Create(1, 0)
            },
            new[]
            {
                CellCoord.Create(0, 0),
                CellCoord.Create(1, 1),
                CellCoord.Create(2, 1)
            },
            new[]
            {
                CellCoord.Create(2, 1),
                CellCoord.Create(1, 0),
                CellCoord.Create(0, 0)
            },
            new[]
            {
                CellCoord.Create(3, 3),
                CellCoord.Create(2, 3),
                CellCoord.Create(1, 3),
                CellCoord.Create(0, 3),
            }
        };

        [Theory]
        [MemberData(nameof(TraceFixtures))]
        public void TraceWorks(CellCoord[] expected)
        {
            var from = expected[0];
            var to = expected[^1];
            var actual = PathOperations.Trace(from, to).ToArray();
            Assert.Equal(expected, actual);
        }
    }
}