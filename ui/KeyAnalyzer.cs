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
            return SpaceChars
                .Select(ch => (char?) ch) // no separator option
                .Prepend(null)
                // no separator option
                .SelectMany(sepOpt => Enum.GetValues<KeyCharSetEnum>()
                    .Select(charSet => new KeyDefinition
                    {
                        KeyString = keyString,
                        Separator = sepOpt,
                        CharSet = charSet
                    }))
                .Select(IdentifyFragments)
                .FirstOrDefault(def => ValidateKeyDefinition(def).Count == 0) ?? new KeyDefinition // last resort
            {
                KeyString = keyString,
                CustomCharset = string.Join(null, new HashSet<char>(keyString))
            };
        }

        /// <summary>
        /// Returns a list of string describing any problems with the given KeyDefinition.
        /// </summary>
        public static IList<string> ValidateKeyDefinition(KeyDefinition def)
        {
            var errors = new List<string>();

            if (string.IsNullOrEmpty(def.KeyString))
                errors.Add("Need key string");
            
            if (def.CharSet == null && string.IsNullOrEmpty(def.CustomCharset))
                errors.Add("Need pre-defined or custom character set");

            if (def.CharSet != null && !string.IsNullOrEmpty(def.CustomCharset))
                errors.Add("Cannot specify both pre-defined and custom character set");
            
            if (def.Separator == null && def.FragmentCycle != null)
                errors.Add("Need a separator if dividing into fragments");
            
            if (def.Separator != null && def.FragmentCycle == null)
                errors.Add("Need fragment cycles if dividing into fragments");
            
            var charSet = def.CharSet != null
                ? CharSetFor(def.CharSet!.Value) 
                : def.CustomCharset != null 
                    ? new HashSet<char>(def.CustomCharset) 
                    : null;
            
            if (def.Separator != null && charSet != null && charSet.Contains(def.Separator!.Value))
                errors.Add("Character set contains the separator");
            
            var keyStringChars = new HashSet<char>(def.KeyString);
            if (def.Separator != null && keyStringChars.Contains(def.Separator!.Value))
                errors.Add("Key string contains the separator");

            if (charSet != null && !keyStringChars.IsSubsetOf(charSet))
                errors.Add("Key string contains characters not present in character set");
            
            if (def.FragmentCycle != null && ParseFragmentCycles(def.FragmentCycle) == null)
                errors.Add("Fragment cycles could not be parsed.  Needs to be comma-separated numbers.  E.g. 3,5,3");
            
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