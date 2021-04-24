namespace ui
{
    public record CellCoord
    {
        public static readonly CellCoord Origin = Create(0, 0);

        public static CellCoord Create(int colIndex, int rowIndex) => new()
        {
            ColIndex = colIndex,
            RowIndex = rowIndex
        };

        public int ColIndex { get; init; }

        public int RowIndex { get; init; }

        public override string ToString() => $"{PathOperations.ColName(ColIndex + 1)}{RowIndex + 1}";
    }
}