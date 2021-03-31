using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ui;
using Xunit;
#nullable enable

namespace uiTest
{
    public class RandomCharacterTests
    {
        [Theory]
        [InlineData(0f, 'a')]
        [InlineData(0.09999f, 'a')]
        [InlineData(0.1f, 'b')]
        [InlineData(0.29999f, 'b')]
        [InlineData(0.3f, 'c')]
        [InlineData(0.6f, 'c')]
        public void ChooseWinnerWorks(float randomValue, char expected)
        {
            var spread = new List<KeyValuePair<char, float>>
            {
                new('a', 0.1f),
                new('b', 0.2f),
                new('c', 0.3f)
            };
            var actual = RandomCharacter.ChooseWinner(spread, randomValue);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ChooseWinnerThrowsArgumentExceptionIfRandomValueIsTooLarge()
        {
            var spread = new List<KeyValuePair<char, float>>
            {
                new('a', 0.1f),
                new('b', 0.2f),
                new('c', 0.3f)
            };
            var ex = Assert.Throws<ArgumentException>(() => RandomCharacter.ChooseWinner(spread, 0.6001f));
            Assert.Equal("randomValue", ex.ParamName);
        }

        [Fact]
        public void ChooseWinnerThrowsArgumentExceptionIfRandomValueIsTooSmall()
        {
            var spread = new List<KeyValuePair<char, float>>
            {
                new('a', 0.1f),
                new('b', 0.2f),
                new('c', 0.3f)
            };
            var ex = Assert.Throws<ArgumentException>(() => RandomCharacter.ChooseWinner(spread, -0.00001f));
            Assert.Equal("randomValue", ex.ParamName);
        }

        [Fact]
        public void ChooseWinnerThrowsArgumentExceptionIfProbabilityTooSmall()
        {
            var spread = new List<KeyValuePair<char, float>>
            {
                new('a', 0.1f),
                new('b', 0f),
                new('c', 0.3f)
            };
            var ex = Assert.Throws<ArgumentException>(() => RandomCharacter.ChooseWinner(spread, 0.2f));
            Assert.Equal("spreadList", ex.ParamName);
        }

        [Theory]
        [InlineData("", 3f, 3f, 3f)]
        [InlineData("aaa", null, 3f, 3f)]
        [InlineData("a", 2f, 3f, 3f)]
        [InlineData("abc", 2f, 2f, 2f)]
        [InlineData("abcabc", 1f, 1f, 1f)]
        [InlineData("abbccc", 2f, 1f, null)]
        public void CalculateSpreadWorks(string alreadyInput, float? aExp, float? bExp, float? cExp)
        {
            var spread = RandomCharacter.CalculateSpread(alreadyInput, 8, new HashSet<char>("abcdefgh"));

            [AssertionMethod]
            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            void Check(float? exp, char ch)
            {
                var isPresent = spread.TryGetValue(ch, out var actual);
                if (!isPresent)
                    Assert.Null(exp);
                else
                    Assert.Equal(exp, actual);
            }
            Check(aExp, 'a');
            Check(bExp, 'b');
            Check(cExp, 'c');
        }
    }
}