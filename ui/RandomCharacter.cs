using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace ui
{
    public static class RandomCharacter
    {
        private static readonly Random RandomInstance = new Random();
        public static IEnumerable<double> PseudoRandom()
        {
            while (true)
                yield return RandomInstance.NextDouble();
            // ReSharper disable once IteratorNeverReturns
        }
        
        /// <summary>
        /// Generate random characters, preferring characters that are not already chosen already
        /// so as the result will have a reasonably random appearance spread.
        /// </summary>
        public static IList<char> GenerateCharacters(IEnumerable<char> already, int targetSize, ISet<char> charSet,
            IEnumerator<double> randomDoubles)
        {
            var spread = CalculateSpread(already, targetSize, charSet);

            var spreadSum = spread.Values.Sum();
            var spreadList = spread.ToList();
            var result = new List<char>();
            while (result.Count < targetSize)
            {
                if (!randomDoubles.MoveNext())
                    throw new Exception($"Have run out of random numbers at {result.Count}");
                var slug = (float) (randomDoubles.Current * spreadSum);
                var winner = ChooseWinner(spreadList, slug);
                result.Add(winner);
            }

            return result;
        }

        /// <summary>
        /// Calculate a relative chance that a character will be chosen given the results should steer away from already present chars.
        /// </summary>
        /// <param name="already">Chars that are already chosen which may be multiple</param>
        /// <param name="targetSize">Target size of the final list of characters.  Resulting spread will be used targetSize - already.Count times</param>
        /// <param name="charSet">Complete set of characters allowed</param>
        /// <param name="maxVsAvgOccurenceRatio">Max occurrences of a character as compared to avg occurrences.  Default 3.</param>
        /// <returns>Characters and their relative probability of being chosen</returns>
        public static Dictionary<char, float> CalculateSpread(IEnumerable<char> already, int targetSize,
            ISet<char> charSet, int maxVsAvgOccurenceRatio = 3)
        {
            var maxOccurrence = targetSize * maxVsAvgOccurenceRatio / (float) charSet.Count;

            var tally = new Dictionary<char, int>();
            foreach (var c in already)
            {
                tally.TryGetValue(c, out var oldCount);
                tally[c] = oldCount + 1;
            }

            var spread = new Dictionary<char, float>();
            foreach (var c in charSet)
            {
                tally.TryGetValue(c, out var alreadyCount);
                var remaining = maxOccurrence - alreadyCount;
                if (remaining > 0f)
                {
                    // have not reached max occurence yet
                    spread[c] = remaining;
                }

                // else, have already reached max occurrences for this char
            }

            return spread;
        }

        /// <summary>
        /// Return the winner of the random value draw.
        /// </summary>
        /// <param name="spreadList">The chance that the given char will be chosen in comparison with other candidates</param>
        /// <param name="randomValue">The random value identifying the winner that is 0 &lt;= randomValue &lt; spreadList.Values.Sum</param>
        /// <returns>The winning char or throws ArgumentException if randomValue is too large or small, or spread contains float &lt;= 0</returns>
        public static char ChooseWinner(IList<KeyValuePair<char, float>> spreadList, float randomValue)
        {
            if (randomValue < 0f)
                throw new ArgumentException("Value too small", nameof(randomValue));

            var slug = randomValue;
            foreach (var (ch, remaining) in spreadList)
            {
                if (remaining <= 0f)
                    throw new ArgumentException($"'{ch}' has relative probability {remaining}", nameof(spreadList));

                slug -= remaining;
                if (slug < 0f)
                    return ch;
            }

            if (slug > float.Epsilon)
                throw new ArgumentException("Value is too large", nameof(randomValue));

            return spreadList[^1].Key;
        }
    }
}