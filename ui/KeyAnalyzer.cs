using System;
using System.Collections.Generic;
using System.Linq;

namespace ui
{
    public static class KeyAnalyzer
    {
        private const string SpaceChars = " -_.";

        public static KeyDefinition AnalyzeKeyString(string keyString)
        {
            return SpaceChars.Select(ch => (char?) ch)
                .Prepend(null) // no separator option
                .SelectMany(sepOpt => Enum.GetValues<KeyCharSetEnum>()
                    .Select(charSet => new KeyDefinition
                    {
                        KeyString = keyString,
                        Separator = sepOpt,
                        CharSet = charSet
                    }))
                .Where(CharSetDoesNotContainSeparator)
                .Select(IdentifyFragments)
                .Where(KeyStringContainsValidChars)
                .FirstOrDefault() ?? new KeyDefinition // last resort
            {
                KeyString = keyString,
                CustomCharset = string.Join(null, new HashSet<char>(keyString))
            };
        }

        private static bool KeyStringContainsValidChars(KeyDefinition def)
        {
            var validChars = CharSetFor(def.CharSet!.Value);
            var keyStringChars = new HashSet<char>(def.KeyString);
            return keyStringChars.IsSubsetOf(validChars);
        }
        
        /// <summary>
        /// Return a KeyDefinition with separators removed from the keyString and fragmentCycles identified.
        /// E.G.  abc-def => abcdef & 3,3
        /// </summary>
        private static KeyDefinition IdentifyFragments(KeyDefinition def)
        {
            if (def.Separator == null)
                return def;
            var fragments = def.KeyString.Split(def.Separator!.Value);
            return def with
            {
                KeyString = string.Join(null, fragments),
                FragmentCycle = string.Join(',', fragments.Select(s => s.Length))
            };
        }

        /// <summary>
        /// Return 'true' if the given definition has a valid KeyCharSetEnum/Separator combination.
        /// </summary>
        private static bool CharSetDoesNotContainSeparator(KeyDefinition def)
        {
            return def.Separator == null ||
                   !CharSetFor(def.CharSet!.Value)
                       .Contains(def.Separator!.Value);
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