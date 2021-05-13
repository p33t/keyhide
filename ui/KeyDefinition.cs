using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.VisualBasic;

namespace ui
{
    public record KeyDefinition : IValidatableObject
    {
        public void ReadFields(KeyDefinition source)
        {
            KeyString = source.KeyString;
            SampleKeyStrings = source.SampleKeyStrings;
            CharSet = source.CharSet;
            Separator = source.Separator;
            CustomCharset = source.CustomCharset;
            PrefixLength = source.PrefixLength;
            SuffixLength = source.SuffixLength;
        }

        [Required] public string KeyString { get; set; } = string.Empty;

        public string KeyStringBody => ExtractBody(KeyString);

        private string ExtractBody(string keyString)
        {
            return keyString.Skip(PrefixLengthSafe).Reverse().Skip(SuffixLengthSafe).Reverse().AsString();
        }

        public IEnumerable<string> CompleteSampleBody =>
            CompleteSample
                .Select(ExtractBody)
                .Where(s => !string.IsNullOrEmpty(s));

        public IEnumerable<string> CompleteSample =>
            SampleKeyStrings
                .Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .Append(KeyString);

        public string EffectiveKeyString => Separator == null
            ? KeyStringBody
            : KeyStringBody.Replace(SeparatorStr, "");

        public int PrefixLength { get; set; }

        private int PrefixLengthSafe => Math.Max(PrefixLength, 0);

        public int SuffixLength { get; set; }

        private int SuffixLengthSafe => Math.Max(SuffixLength, 0);

        public string SampleKeyStrings { get; set; } = string.Empty;
        public KeyCharSetEnum? CharSet { get; set; }

        public char? Separator { get; set; }

        public string SeparatorStr
        {
            get => Separator?.ToString() ?? string.Empty;
            set => Separator = value.Length == 0 ? null : value[0];
        }

        public string? CustomCharset { get; set; }

        public string Prefix => KeyString.Take(PrefixLengthSafe).AsString();

        public string Suffix => KeyString.Reverse().Take(SuffixLengthSafe).Reverse().AsString();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var def = (KeyDefinition) validationContext.ObjectInstance;
            var errors = new List<ValidationResult>();

            void AddError(string msg, params string[] fields)
            {
                errors!.Add(new ValidationResult(msg, fields));
            }

            if (def.CharSet == null && string.IsNullOrEmpty(def.CustomCharset))
                AddError("Need pre-defined or custom character set", nameof(CharSet), nameof(CustomCharset));

            if (def.CharSet != null && !string.IsNullOrEmpty(def.CustomCharset))
                AddError("Cannot specify both pre-defined and custom character set", nameof(CharSet),
                    nameof(CustomCharset));

            var charSet = def.CharSet != null
                ? KeyAnalyzer.CharSetFor(def.CharSet!.Value)
                : def.CustomCharset != null
                    ? new HashSet<char>(def.CustomCharset)
                    : null;

            if (def.Separator != null && charSet != null && charSet.Contains(def.Separator!.Value))
                AddError("Character set contains the separator",
                    nameof(Separator));

            if (def.PrefixLength < 0)
                AddError("Prefix length must be > 0", nameof(KeyDefinition.PrefixLength));

            if (def.SuffixLength < 0)
                AddError("Suffix length must be > 0", nameof(KeyDefinition.SuffixLength));

            var keyStringChars = new HashSet<char>(def.KeyStringBody);
            if (string.IsNullOrEmpty(def.KeyString))
                AddError("Need key string", nameof(KeyDefinition.KeyString));
            else if (string.IsNullOrEmpty(def.KeyStringBody))
                AddError("Key string is entirely prefix/suffix",
                    nameof(KeyDefinition.KeyString), nameof(PrefixLength), nameof(SuffixLength));
            else if (def.Separator != null && !keyStringChars.Contains(def.Separator!.Value))
                AddError("Key string does not contain the separator",
                    nameof(Separator));

            if (charSet != null)
            {
                var sansSeparator = def.Separator == null
                    ? keyStringChars
                    : keyStringChars.Where(ch => ch != def.Separator).ToHashSet();

                if (!sansSeparator.IsSubsetOf(charSet))
                    AddError("Key string contains characters not present in character set",
                        nameof(KeyString));
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