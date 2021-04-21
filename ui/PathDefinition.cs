using System.ComponentModel.DataAnnotations;

namespace ui
{
    public class PathDefinition
    {
        public CellCoord[] Coords { get; set; } = System.Array.Empty<CellCoord>();
    }
}