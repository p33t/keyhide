using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ui
{
    public static class KeyAnalyzer
    {
        private const string SpaceChars = " -_.";

        /// <summary>
        /// Analyzes the given string for obvious fragments and preset character sets to achieve a key definition.
        /// If no common ones are found then a 'custom' character set with no fragments is returned.
        /// </summary>
        public static KeyDefinition AnalyzeKeyString(string keyString)
        {
            //////////////////////////////////////// Need to convert to 'raw key string' inside a KeyDefinition
            return SpaceChars
                .Select(ch => (char?) ch)
                // no separator option
                .Prepend(null)
                .SelectMany(sepOpt => Enum.GetValues<KeyCharSetEnum>()
                    .Select(charSet => new KeyDefinition
                    {
                        KeyString = keyString,
                        Separator = sepOpt,
                        CharSet = charSet
                    }))
                .FirstOrDefault(def => ValidateKeyDefinition(def).Count == 0) ?? new KeyDefinition // last resort
            {
                KeyString = keyString,
                CustomCharset = string.Join(null, new HashSet<char>(keyString))
            };
        }

        /// <summary>
        /// Returns a list of string describing any problems with the given KeyDefinition.
        /// </summary>
        public static IList<ValidationResult> ValidateKeyDefinition(KeyDefinition def)
        {
            var errors = new List<ValidationResult>();

            if (def.CharSet == null && string.IsNullOrEmpty(def.CustomCharset))
                errors.Add(new ValidationResult("Need pre-defined or custom character set",
                    new[] {nameof(KeyDefinition.CharSet), nameof(KeyDefinition.CustomCharset)}));

            if (def.CharSet != null && !string.IsNullOrEmpty(def.CustomCharset))
                errors.Add(new ValidationResult("Cannot specify both pre-defined and custom character set",
                    new[] {nameof(KeyDefinition.CharSet), nameof(KeyDefinition.CustomCharset)}));

            var charSet = def.CharSet != null
                ? CharSetFor(def.CharSet!.Value)
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
                        new[] {nameof(KeyDefinition.KeyString)}));
            }

            return errors.AsReadOnly();
        }

        /// <summary>
        /// Return an array of positive ints converted from the given string of comma-separated ints, or
        /// 'null' if not a valid string.
        /// </summary>
        public static int[]? ParseFragmentCycles(string fragmentCycles)
        {
            var ss = fragmentCycles.Split(',');
            try
            {
                var result = ss.Select(int.Parse).ToArray();
                if (result.Length == 0)
                    return null;

                return result.Any(i => i <= 0) ? null : result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Returns the set of characters for the specified charset.
        /// </summary>
        public static ISet<char> CharSetFor(KeyCharSetEnum charSetEnum)
        {
            var s = charSetEnum switch
            {
                KeyCharSetEnum.Numeric => "0123456789",
                KeyCharSetEnum.AlphabetLower => "abcdefghijklmnopqrstuvwxyz",
                KeyCharSetEnum.AlphabetUpper => "ABCDEFGHIJKLMNOPQRSTUVWXYZ",
                KeyCharSetEnum.AlphaNumeric => "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ",
                KeyCharSetEnum.HexadecimalLower => "0123456789abcdef",
                KeyCharSetEnum.HexadecimalUpper => "0123456789ABCDEF",
                _ => throw new ArgumentException($"Unrecognised option {charSetEnum}", nameof(charSetEnum))
            };

            return new HashSet<char>(s);
        }
    }
}