using System.ComponentModel.DataAnnotations;

namespace ui
{
    public class PathDefinition
    {
        [Range(10, 100)]
        public int ColCount { get; set; } = 26;
        
        [Range(10, 100)]
        public int RowCount { get; set; } = 32;

        public CellCoord[] Coords { get; set; } = System.Array.Empty<CellCoord>();
    }
}