using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ui
{
    public record KeyDefinition : IValidatableObject
    {
        [Required]
        public string KeyString { get; set; } = string.Empty;

        public KeyCharSetEnum? CharSet { get; set; }

        public char? Separator { get; set; }

        public string SeparatorStr
        {
            get => Separator == null ? "" : Separator.ToString();
            set => Separator = value.Length == 0 ? null : value[0];
        }
        
        public string? CustomCharset { get; set; }
        
        public string? Prefix { get; set; }

        public string? Suffix { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return KeyAnalyzer.ValidateKeyDefinition((KeyDefinition) validationContext.ObjectInstance);
        }
    }
}