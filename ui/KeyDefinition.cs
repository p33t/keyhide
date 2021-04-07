using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ui
{
    public record KeyDefinition : IValidatableObject
    {
        public string KeyString { get; init; } = string.Empty;

        public KeyCharSetEnum? CharSet { get; init; }

        public char? Separator { get; init; }
        
        public string? CustomCharset { get; init; }
        
        public string? Prefix { get; init; }

        public string? Suffix { get; init; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return KeyAnalyzer.ValidateKeyDefinition((KeyDefinition) validationContext.ObjectInstance);
        }
    }
}