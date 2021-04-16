namespace ui
{
    public class CoordGrid<T>
    {
        private readonly T[,] _data;

        public CoordGrid(int colCount, int rowCount)
        {
            _data = new T[colCount, rowCount];
        }

        public T this[CellCoord coord]
        {
            get => _data[coord.ColIndex, coord.RowIndex];
            set => _data[coord.ColIndex, coord.RowIndex] = value;
        }
    }
}