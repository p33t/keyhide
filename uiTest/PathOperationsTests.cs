using System;
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
            grid[Coord(2, 3)] = '1';
            grid[Coord(3, 3)] = '2';
            grid[Coord(4, 3)] = '3';
            var last = Coord(4, 4);
            grid[last] = '4';
            var actual = PathOperations.CoordIsAvailable(Coord(candidateCol, candidateRow), grid, last,
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

        public static TheoryData<CellCoord[]> TraceLegFixtures => new()
        {
            new[]
            {
                Coord(0, 0)
            },
            new[]
            {
                Coord(0, 0),
                Coord(1, 1)
            },
            new[]
            {
                Coord(0, 0),
                Coord(0, 1)
            },
            new[]
            {
                Coord(0, 0),
                Coord(1, 0)
            },
            new[]
            {
                Coord(0, 0),
                Coord(1, 1),
                Coord(2, 1)
            },
            new[]
            {
                Coord(2, 1),
                Coord(1, 0),
                Coord(0, 0)
            },
            new[]
            {
                Coord(3, 3),
                Coord(2, 3),
                Coord(1, 3),
                Coord(0, 3),
            }
        };

        private static CellCoord Coord(int colIndex, int rowIndex)
        {
            return CellCoord.Create(colIndex, rowIndex);
        }

        [Theory]
        [MemberData(nameof(TraceLegFixtures))]
        public void TraceLegWorks(CellCoord[] expected)
        {
            var from = expected[0];
            var to = expected[^1];
            var actual = PathOperations.TraceLeg(from, to).ToArray();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TraceWorks()
        {
            void Check(CellCoord[] input, CellCoord[] expected)
            {
                var actual = PathOperations.Trace(input);
                Assert.Equal(expected, actual);
            }

            Check(Array.Empty<CellCoord>(), Array.Empty<CellCoord>());
            Check(new[] {Coord(0, 0)}, new[] {Coord(0, 0)});
            Check(new[] {Coord(0, 0), Coord(1, 1)}, new[] {Coord(0, 0), Coord(1, 1)});
            Check(new[]
            {
                Coord(0, 0),
                Coord(2, 1),
                Coord(2, 2)
            }, new[]
            {
                Coord(0, 0),
                Coord(1, 1),
                Coord(2, 1),
                Coord(2, 2),
            });
        }
    }
}