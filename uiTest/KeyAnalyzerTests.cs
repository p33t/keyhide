using System.Collections.Generic;
using ui;
using Xunit;
using Xunit.Sdk;

namespace uiTest
{
    public class KeyAnalyzerTests
    {
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
                    KeyString = "7681aff2843b4003803f71c9231c9c6e",
                    Separator = '-',
                    CharSet = KeyCharSetEnum.HexadecimalLower,
                    FragmentCycle = "8,4,4,4,12"
                }
            },
            {
                "7681AFF2 843B 4003 803F 71C9231C9C6E", new KeyDefinition
                {
                    KeyString = "7681AFF2843B4003803F71C9231C9C6E",
                    Separator = ' ',
                    CharSet = KeyCharSetEnum.HexadecimalUpper,
                    FragmentCycle = "8,4,4,4,12"
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
                    KeyString = "092384329932457",
                    Separator = '.',
                    CharSet = KeyCharSetEnum.Numeric,
                    FragmentCycle = "5,5,5"
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

        private static readonly KeyDefinition GuidKeyDefinition = new KeyDefinition
        {
            Separator = '-',
            CharSet = KeyCharSetEnum.HexadecimalUpper,
            FragmentCycle = "8,4,4,4",
            KeyString = "0C9FAEDE13BB4B92ADD2696859ABA823"
        };

        public static readonly TheoryData<KeyDefinition, int> KeyDefinitionFixtures = new()
        {
            {GuidKeyDefinition, 0},
            {GuidKeyDefinition with {KeyString = "x"}, 1},
            {GuidKeyDefinition with {Separator = null}, 1},
            {GuidKeyDefinition with {KeyString = ""}, 1},
            {GuidKeyDefinition with {CharSet = null}, 1},
            {GuidKeyDefinition with {CharSet = null, CustomCharset = "23423"}, 1},
            {GuidKeyDefinition with {CustomCharset = "23423"}, 1},
            {GuidKeyDefinition with {Separator = 'A'}, 2},
            {GuidKeyDefinition with {FragmentCycle = null}, 1},
            {GuidKeyDefinition with {FragmentCycle = "0"}, 1},
            {GuidKeyDefinition with {FragmentCycle = "b"}, 1},
        };

        [Theory]
        [MemberData(nameof(KeyDefinitionFixtures))]
        public void ValidateKeyDefinitionDetectsErrors(KeyDefinition input, int expCount)
        {
            var actual = KeyAnalyzer.ValidateKeyDefinition(input);
            if (actual.Count != expCount)
                throw new XunitException($"Expected {expCount} errors but got: [\n  {string.Join("\n  ", actual)}\n]");
        }

        public static readonly TheoryData<string, int[]?> ParseFragmentCyclesFixtures = new()
        {
            {"1", new[] {1}},
            {"1,2", new[] {1, 2}},
            {"1, 2", new [] {1, 2}},
            {"1 ,2", new [] {1, 2}},
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