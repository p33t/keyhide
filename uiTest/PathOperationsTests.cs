using System.Linq;
using ui;
using Xunit;

namespace uiTest
{
    public class PathOperationsTests
    {
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
            new []
            {
                CellCoord.Create(0,0)
            },
            new []
            {
                CellCoord.Create(0,0),
                CellCoord.Create(1,1)
            },
            new []
            {
                CellCoord.Create(0,0),
                CellCoord.Create(0,1)
            },
            new []
            {
                CellCoord.Create(0,0),
                CellCoord.Create(1,0)
            },
            new []
            {
                CellCoord.Create(0,0),
                CellCoord.Create(1,1),
                CellCoord.Create(2,1)
            },
            new []
            {
                CellCoord.Create(2,1),
                CellCoord.Create(1,0),
                CellCoord.Create(0,0)
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