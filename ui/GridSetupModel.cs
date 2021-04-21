using System.ComponentModel.DataAnnotations;

namespace ui
{
    public record GridSetupModel
    {
        public const int DefaultColCount = 24;
        public const int DefaultRowCount = 32;

        [Required]
        [Range(10, 100)]
        public int ColCount { get; set; } = DefaultColCount;
        
        [Required]
        [Range(10, 100)]
        public int RowCount { get; set; } = DefaultRowCount;

        public GridSetupModel DeepCopy()
        {
            // Not mutable properties ATM
            return this with { };
        }
    }
}