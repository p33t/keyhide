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
            var result = new List<ValidationResult>();
            Validator.TryValidateObject(def, new ValidationContext(def), result, true);
            return result;
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

        public static FinalModel CreateFinalModel(PathDefinition pathDefinition, KeyDefinition keyDefinition,
            IEnumerable<char> filler)
        {
            var effectiveKeyString = pathDefinition.EffectiveKeyString!;
            var grid = new CoordGrid<char?>(pathDefinition.ColCount, pathDefinition.RowCount);

            var count = 0;
            foreach (var coord in PathOperations.Trace(pathDefinition.Coords))
            {
                if (count < effectiveKeyString.Length)
                    grid[coord] = effectiveKeyString[count];

                count++;
            }

            using var fillerator = filler.GetEnumerator();
            foreach (var coord in grid.AllCoords())
            {
                if (grid[coord] == null)
                {
                    if (!fillerator.MoveNext())
                        throw new InvalidOperationException($"Ran out of filler at {coord}");

                    grid[coord] = fillerator.Current;
                }
            }

            var rowStrings = new string[grid.RowCount];
            for (var ixRow = 0; ixRow < rowStrings.Length; ixRow++)
            {
                var row = string.Empty;
                for (var ixCol = 0; ixCol < grid.ColCount; ixCol++)
                {
                    row += grid[CellCoord.Create(ixCol, ixRow)];
                }

                rowStrings[ixRow] = row;
            }

            var fragmentCycles = keyDefinition.Separator == null 
                ? Array.Empty<int>() 
                : CalcFragmentCycles(keyDefinition.KeyString, keyDefinition.Separator!.Value);
            
            return new FinalModel
            {
                Subtract = count - effectiveKeyString.Length,
                Prefix = keyDefinition.Prefix ?? string.Empty,
                Suffix = keyDefinition.Suffix ?? string.Empty,
                FragmentSeparator = keyDefinition.SeparatorStr,
                FragmentCycles = fragmentCycles,
                Grid = rowStrings
            };
        }

        public static int[] CalcFragmentCycles(string keyString, char separator)
        {
            var lengths = keyString.Split(separator).Select(s => s.Length).ToList();
            if (lengths.Count <= 1)
                return Array.Empty<int>();

            // no need for last one if it's shorter than the first (due to cycling)
            if (lengths[^1] <= lengths[0])
                lengths.RemoveAt(lengths.Count - 1);

            for (var candidateCycleLength = 1; candidateCycleLength < lengths.Count; candidateCycleLength++)
            {
                var candidateCycle = lengths.GetRange(0, candidateCycleLength);
                bool success = true;
                for (var ixStart = candidateCycleLength; ixStart < lengths.Count; ixStart += candidateCycleLength)
                {
                    var nextCycle = lengths.Skip(ixStart).Take(candidateCycleLength).ToList();
                    var match = candidateCycle.Take(nextCycle.Count).SequenceEqual(nextCycle);
                    if (!match)
                    {
                        success = false;
                        break;
                    }
                }

                if (success)
                    return candidateCycle.ToArray();
            }

            return lengths.ToArray();
        }
    }
}