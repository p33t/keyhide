using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ui
{
    public record PathDefinition : IValidatableObject
    {
        public const int DefaultColCount = 24;
        public const int DefaultRowCount = 32;

        [Required] [Range(10, 100)] public int ColCount { get; set; } = DefaultColCount;

        [Required] [Range(10, 100)] public int RowCount { get; set; } = DefaultRowCount;

        public IEnumerable<CellCoord> Coords { get; set; } = Enumerable.Empty<CellCoord>();

        public string? KeyString { get; set; }

        public PathDefinition DeepCopy()
        {
            // Not mutable properties ATM
            return this with { };
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var errors = new List<ValidationResult>();
            var model = (PathDefinition) validationContext.ObjectInstance;
            if (!Coords.Any())
                errors.Add(new ValidationResult("No path elements", new[] {nameof(Coords)}));

            if (string.IsNullOrEmpty(KeyString))
                errors.Add(new ValidationResult("No key has been supplied", new[] {nameof(KeyString)}));
            else
                ValidatePath(model, errors);
            
            return errors;
        }

        private void ValidatePath(PathDefinition model, List<ValidationResult> errors)
        {
            var grid = new CoordGrid<char?>(model.ColCount, model.RowCount);
            string remaining = KeyString!;

            // consume a key character OR return false if an error was registered
            bool Consume(CellCoord coord)
            {
                if (remaining.Length > 0)
                {
                    var head = remaining[0];
                    remaining = remaining.Substring(1);
                    var current = grid![coord];
                    if (current == null)
                        grid[coord] = head;
                    else if (current != head)
                    {
                        errors!.Add(new ValidationResult($"Collision at {coord} with {remaining.Length + 1} to go",
                            new[] {nameof(Coords)}));
                        return false;
                    }
                }
                // else ignore remaining path
                
                return true;
            }

            CellCoord? prev = null;
            foreach (var coord in model.Coords)
            {
                if (0 > coord.RowIndex || coord.RowIndex >= model.RowCount ||
                    0 > coord.ColIndex || coord.ColIndex >= model.ColCount)
                {
                    errors.Add(new ValidationResult($"Illegal coordinate {coord}", new[] {nameof(Coords)}));
                    return;
                }

                var isSingleCellLeg = prev is null || Equals(prev.Value, coord);
                IEnumerable<CellCoord> leg = isSingleCellLeg 
                    ? new[] {coord} 
                    : PathOperations.Trace(prev!.Value, coord).Skip(1);
                if (leg.Any(legCoord => !Consume(legCoord)))
                    return;
                
                prev = coord;
            }

            if (remaining.Length > 0)
                errors.Add(new ValidationResult($"Still have {remaining.Length} to go", new [] {nameof(Coords)}));
        }
    }
}