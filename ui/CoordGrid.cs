using System.Collections.Generic;
using System.Linq;

namespace ui
{
    public class CoordGrid<T>
    {
        private readonly T[,] _data;

        public CoordGrid(int colCount, int rowCount)
        {
            _data = new T[colCount, rowCount];
        }

        public int ColCount => _data.GetLength(0);
        public int RowCount => _data.GetLength(1);
        
        public T this[CellCoord coord]
        {
            get => _data[coord.ColIndex, coord.RowIndex];
            set => _data[coord.ColIndex, coord.RowIndex] = value;
        }

        public IEnumerable<CellCoord> AllCoords() =>
            Enumerable.Range(0, _data.GetLength(0))
                .SelectMany(ixCol => Enumerable.Range(0, _data.GetLength(1))
                    .Select(ixRow => CellCoord.Create(ixCol, ixRow)));
    }
}