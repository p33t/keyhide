using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ui
{
    public record KeyDefinition : IValidatableObject
    {
        [Required] public string KeyString { get; set; } = string.Empty;

        public KeyCharSetEnum? CharSet { get; set; }

        public char? Separator { get; set; }

        public string SeparatorStr
        {
            get => Separator?.ToString() ?? string.Empty;
            set => Separator = value.Length == 0 ? null : value[0];
        }

        public string? CustomCharset { get; set; }

        public string? Prefix { get; set; }

        public string? Suffix { get; set; }

        
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var def = (KeyDefinition) validationContext.ObjectInstance;
            var errors = new List<ValidationResult>();

            if (def.CharSet == null && string.IsNullOrEmpty(def.CustomCharset))
                errors.Add(new ValidationResult("Need pre-defined or custom character set",
                    new[] {nameof(KeyDefinition.CharSet), nameof(KeyDefinition.CustomCharset)}));

            if (def.CharSet != null && !string.IsNullOrEmpty(def.CustomCharset))
                errors.Add(new ValidationResult("Cannot specify both pre-defined and custom character set",
                    new[] {nameof(KeyDefinition.CharSet), nameof(KeyDefinition.CustomCharset)}));

            var charSet = def.CharSet != null
                ? KeyAnalyzer.CharSetFor(def.CharSet!.Value)
                : def.CustomCharset != null
                    ? new HashSet<char>(def.CustomCharset)
                    : null;

            if (def.Separator != null && charSet != null && charSet.Contains(def.Separator!.Value))
                errors.Add(new ValidationResult("Character set contains the separator",
                    new[] {nameof(KeyDefinition.Separator)}));

            var keyStringChars = new HashSet<char>(def.KeyString);
            if (string.IsNullOrEmpty(def.KeyString))
                errors.Add(new ValidationResult("Need key string", new[] {nameof(KeyDefinition.KeyString)}));
            else if (def.Separator != null && !keyStringChars.Contains(def.Separator!.Value))
                errors.Add(new ValidationResult("Key string does not contain the separator",
                    new[] {nameof(KeyDefinition.Separator)}));

            if (charSet != null)
            {
                var sansSeparator = def.Separator == null
                    ? keyStringChars
                    : keyStringChars.Where(ch => ch != def.Separator).ToHashSet();

                if (!sansSeparator.IsSubsetOf(charSet))
                    errors.Add(new ValidationResult("Key string contains characters not present in character set",
                        new[] {nameof(KeyString)}));
            }

            return errors.AsReadOnly();
        }

        /// Creates a copy that can be modified independently of this instance
        public KeyDefinition DeepCopy()
        {
            // no mutable properties ATM
            return this with { };
        }
    }
}