using System.Collections.Generic;
using ui;
using Xunit;
using Xunit.Sdk;

namespace uiTest
{
    public class KeyAnalyzerTests
    {
        [Fact]
        public void CreateFinalModelWorks()
        {
            var keyDefinition = new KeyDefinition
            {
                Prefix = "pref",
                Suffix = "suff",
                Separator = '_',
                KeyString = "ab_cd_e"
            };
            var pathDefinition = new PathDefinition
            {
                /*
                 * ....c.
                 * ...bd.
                 * ..a.e.
                 * ......
                 * ......
                 */
                Coords = new[] {CellCoord.Create(2, 2), CellCoord.Create(4, 0), CellCoord.Create(4, 4)},
                ColCount = 6,
                RowCount = 5,
                EffectiveKeyString = "abcde"
            };

            IEnumerable<char> Xs()
            {
                while (true)
                    yield return '.';
                // ReSharper disable once IteratorNeverReturns
            }

            var actual = KeyAnalyzer.CreateFinalModel(pathDefinition, keyDefinition, Xs());
            Assert.Equal(new[]
            {
                "....c.",
                "...bd.",
                "..a.e.",
                "......",
                "......",
            }, actual.Grid);
            Assert.Equal(keyDefinition.Prefix, actual.Prefix);
            Assert.Equal(keyDefinition.Suffix, actual.Suffix);
            Assert.Equal(keyDefinition.SeparatorStr, actual.FragmentSeparator);
            Assert.Equal(2, actual.Subtract); // coords length - effectiveKeyString.Length
            Assert.Equal(new[] {2}, actual.FragmentCycles);
        }

        [Theory]
        [InlineData("abc")]
        [InlineData("abc-", 3)]
        [InlineData("-abc", 0, 3)]
        [InlineData("abc-def", 3)]
        [InlineData("abc-de", 3)]
        [InlineData("abc-def-ghi", 3)]
        [InlineData("abc-def-gh", 3)]
        [InlineData("abc-defg", 3, 4)]
        [InlineData("abc-defg-abc-defg", 3, 4)]
        [InlineData("abc-defg-abc-defg-abc", 3, 4)]
        [InlineData("abc-defg-abc-defg-abc-defg-a", 3, 4)]
        [InlineData("abc-defg-hijkl", 3, 4, 5)]
        [InlineData("abc-defg-hijkl-abc", 3, 4, 5)]
        [InlineData("abc-defg-hijkl-abc-", 3, 4, 5)]
        public void CalcFragmentCyclesWorks(string keyString, params int[] expected)
        {
            var actual = KeyAnalyzer.CalcFragmentCycles(keyString, '-');
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(KeyCharSetEnum.AlphaNumeric, "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ")]
        [InlineData(KeyCharSetEnum.Numeric, "0123456789")]
        [InlineData(KeyCharSetEnum.AlphabetLower, "abcdefghijklmnopqrstuvwxyz")]
        [InlineData(KeyCharSetEnum.HexadecimalUpper, "ABCDEF0123456789")]
        public void CharSetForWorks(KeyCharSetEnum charSetEnum, string expected)
        {
            var actual = KeyAnalyzer.CharSetFor(charSetEnum);
            var expectedSet = new HashSet<char>(expected);
            Assert.Subset(expectedSet, actual);
            Assert.Subset(actual, expectedSet);
        }

        public static readonly TheoryData<string, KeyDefinition> AnalyzeCases = new()
        {
            {
                "7681aff2-843b-4003-803f-71c9231c9c6e", new KeyDefinition
                {
                    KeyString = "7681aff2-843b-4003-803f-71c9231c9c6e",
                    Separator = '-',
                    CharSet = KeyCharSetEnum.HexadecimalLower,
                }
            },
            {
                "7681AFF2 843B 4003 803F 71C9231C9C6E", new KeyDefinition
                {
                    KeyString = "7681AFF2 843B 4003 803F 71C9231C9C6E",
                    Separator = ' ',
                    CharSet = KeyCharSetEnum.HexadecimalUpper,
                }
            },
            {
                "092384329932457", new KeyDefinition
                {
                    KeyString = "092384329932457",
                    CharSet = KeyCharSetEnum.Numeric
                }
            },
            {
                "09238.43299.32457", new KeyDefinition
                {
                    KeyString = "09238.43299.32457",
                    Separator = '.',
                    CharSet = KeyCharSetEnum.Numeric,
                }
            },
            {
                "9843cpahcp9mw84m9q348rmcp984328m3pijfp", new KeyDefinition
                {
                    KeyString = "9843cpahcp9mw84m9q348rmcp984328m3pijfp",
                    CharSet = KeyCharSetEnum.AlphaNumeric
                }
            }
        };

        [Theory]
        [MemberData(nameof(AnalyzeCases))]
        public void AnalyzeKeyStringWorks(string keyString, KeyDefinition expected)
        {
            var actual = KeyAnalyzer.AnalyzeKeyString(keyString);
            Assert.Equal(expected, actual);
        }

        public static readonly TheoryData<string, int[]?> ParseFragmentCyclesFixtures = new()
        {
            {"1", new[] {1}},
            {"1,2", new[] {1, 2}},
            {"1, 2", new[] {1, 2}},
            {"1 ,2", new[] {1, 2}},
            {",1", null},
            {"1,", null},
            {"0", null},
            {"1,b", null},
            {"1,,2", null},
        };

        [Theory]
        [MemberData(nameof(ParseFragmentCyclesFixtures))]
        public void ParseFragmentCyclesWorks(string input, int[]? expected)
        {
            var actual = KeyAnalyzer.ParseFragmentCycles(input);
            Assert.Equal(expected, actual);
        }
    }
}