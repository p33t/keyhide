using System;
using System.Collections.Generic;

namespace ui
{
    public class KeyAnalyzer
    {
        public static KeyDefinition AnalyzeKeyString(string keyString)
        {
            return new KeyDefinition();
        }

        /// <summary>
        /// Returns the set of characters for the specified charset.
        /// </summary>
        public static ISet<char> CharSetFor(KeyCharSetEnum charSetEnum, KeyCaseEnum caseEnum)
        {
            var s = charSetEnum switch
            {
                KeyCharSetEnum.Numeric => "0123456789",
                KeyCharSetEnum.Alphabet => "abcdefghijklmnopqrstuvwxyz",
                KeyCharSetEnum.AlphaNumeric => "0123456789abcdefghijklmnopqrstuvwxyz",
                KeyCharSetEnum.Hexadecimal => "0123456789abcdef",
                _ => throw new ArgumentException($"Unrecognised option {charSetEnum}", nameof(charSetEnum))
            };

            s = caseEnum switch
            {
                KeyCaseEnum.UpperOnly => s.ToUpperInvariant(),
                KeyCaseEnum.AnyCase => s.ToUpperInvariant() + s,
                KeyCaseEnum.LowerOnly => s,
                _ => throw new ArgumentOutOfRangeException(nameof(caseEnum), caseEnum, null)
            };

            return new HashSet<char>(s);
        }
    }
}