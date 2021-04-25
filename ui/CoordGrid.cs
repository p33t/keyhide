using System;
using System.Collections.Generic;
using System.Linq;

namespace ui
{
    public class CoordGrid<T>
    {
        private readonly IDictionary<CellCoord, T> _data = new Dictionary<CellCoord, T>();

        public CoordGrid(int colCount, int rowCount)
        {
            RowCount = rowCount;
            ColCount = colCount;
        }

        public int ColCount { get; }
        public int RowCount { get; }

        public T? this[CellCoord coord]
        {
            get
            {
                CheckContains(coord);

                return _data.TryGetValue(coord, out var t) ? t : default;
            }
            set
            {
                CheckContains(coord);
                if (value == null)
                {
                    _data.Remove(coord);
                }
                else
                {
                    _data[coord] = value;
                }
            }
        }

        private void CheckContains(CellCoord coord)
        {
            if (!Contains(coord))
            {
                var max = CellCoord.Create(ColCount - 1, RowCount - 1);
                throw new IndexOutOfRangeException($"{coord} is outside range of grid {max}");
            }
        }

        public IEnumerable<CellCoord> AllCoords() =>
            Enumerable.Range(0, ColCount)
                .SelectMany(ixCol => Enumerable.Range(0, RowCount)
                    .Select(ixRow => CellCoord.Create(ixCol, ixRow)));

        public void Reset()
        {
            _data.Clear();
        }

        public bool Contains(CellCoord coord) =>
            0 <= coord.RowIndex && coord.RowIndex < RowCount && 0 <= coord.ColIndex && coord.ColIndex < ColCount;
    }
}