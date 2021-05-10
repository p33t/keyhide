using System;

namespace ui
{
    public record CellCoord
    {
        public static readonly CellCoord Origin = Create(0, 0);
        private readonly int _colIndex;
        private readonly int _rowIndex;

        public static CellCoord Create(int colIndex, int rowIndex) => new()
        {
            ColIndex = colIndex,
            RowIndex = rowIndex
        };

        public int ColIndex
        {
            get => _colIndex;
            private init
            {
                if (value < 0) throw new ArgumentOutOfRangeException(nameof(value));
                _colIndex = value;
            }
        }

        public int RowIndex
        {
            get => _rowIndex;
            init
            {
                if (value < 0) throw new ArgumentOutOfRangeException(nameof(value));
                _rowIndex = value;
            }
        }

        public override string ToString() => $"{PathOperations.ColName(ColIndex + 1)}{RowIndex + 1}";
    }
}