using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ui;
using Xunit;
using Xunit.Sdk;

namespace uiTest
{
    public class KeyDefinitionTests
    {

        private static readonly KeyDefinition GuidKeyDefinition = new KeyDefinition
        {
            Separator = '-',
            CharSet = KeyCharSetEnum.HexadecimalUpper,
            KeyString = "2D5D0C89-7EEE-4EC9-A956-8E4918A035C8"
        };

        public static readonly TheoryData<KeyDefinition, int> KeyDefinitionFixtures = new()
        {
            {GuidKeyDefinition, 0},
            {GuidKeyDefinition with {KeyString = "x"}, 2},
            {GuidKeyDefinition with {Separator = null}, 1},
            {GuidKeyDefinition with {KeyString = ""}, 1},
            {GuidKeyDefinition with {CharSet = null}, 1},
            {GuidKeyDefinition with {CharSet = null, CustomCharset = "23423"}, 1},
            {GuidKeyDefinition with {CustomCharset = "23423"}, 1},
            {GuidKeyDefinition with {Separator = 'A'}, 2},
            {GuidKeyDefinition with {PrefixLength = -1}, 1},
            {GuidKeyDefinition with {PrefixLength = 50}, 1},
            {GuidKeyDefinition with {SuffixLength = -1}, 1},
            {GuidKeyDefinition with {SuffixLength = 50}, 1},
            {GuidKeyDefinition with {PrefixLength = 20, SuffixLength = 20}, 1},
        };

        [Theory]
        [MemberData(nameof(KeyDefinitionFixtures))]
        public void ValidateKeyDefinitionDetectsErrors(KeyDefinition input, int expCount)
        {
            var actual = new List<ValidationResult>();
            Validator.TryValidateObject(input, new ValidationContext(input), actual, true);
            if (actual.Count != expCount)
                throw new XunitException($"Expected {expCount} errors but got: [\n  {string.Join("\n  ", actual)}\n]");
        }

        [Theory]
        [InlineData("abcdef", "abcde", "", "bcdef")]
        [InlineData("abcdefghij", "abcde", "", "fghij")]
        [InlineData("abcdefghijk", "abcde", "f", "ghijk")]
        [InlineData("abcdexyzfghij", "abcde", "xyz", "fghij")]
        public void PrefixSuffixBodyCalculationsWork(string keyString, string expPrefix, string expBody, string expSuffix)
        {
            var keyDef = new KeyDefinition
            {
                KeyString = keyString,
                PrefixLength = 5,
                SuffixLength = 5
            };
            
            Assert.Equal(expPrefix, keyDef.Prefix);
            Assert.Equal(expSuffix, keyDef.Suffix);
            Assert.Equal(expBody, keyDef.KeyStringBody);
        }
    }
}