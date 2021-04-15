using System;

namespace ui
{
    public struct CellCoord
    {
        public static CellCoord Create(int colIndex, int rowIndex) => new()
        {
            ColIndex = colIndex,
            RowIndex = rowIndex
        };
        public int ColIndex { get; init; }
        
        public int RowIndex { get; init; }
    }
}