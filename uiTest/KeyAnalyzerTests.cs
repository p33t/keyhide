using System;
using System.Collections.Generic;
using ui;
using Xunit;

namespace uiTest
{
    public class KeyAnalyzerTests
    {
        [Theory]
        [InlineData(KeyCharSetEnum.AlphaNumeric, KeyCaseEnum.LowerOnly, "0123456789abcdefghijklmnopqrstuvwxyz")]
        [InlineData(KeyCharSetEnum.AlphaNumeric, KeyCaseEnum.UpperOnly, "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ")]
        [InlineData(KeyCharSetEnum.AlphaNumeric, KeyCaseEnum.AnyCase, "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz")]
        [InlineData(KeyCharSetEnum.Numeric, KeyCaseEnum.AnyCase, "0123456789")]
        [InlineData(KeyCharSetEnum.Alphabet, KeyCaseEnum.AnyCase, "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz")]
        [InlineData(KeyCharSetEnum.Alphabet, KeyCaseEnum.LowerOnly, "abcdefghijklmnopqrstuvwxyz")]
        [InlineData(KeyCharSetEnum.Hexadecimal, KeyCaseEnum.UpperOnly, "ABCDEF0123456789")]
        public void CharSetForWorks(KeyCharSetEnum charSetEnum, KeyCaseEnum caseEnum, string expected)
        {
            var actual = KeyAnalyzer.CharSetFor(charSetEnum, caseEnum);
            var expectedSet = new HashSet<char>(expected);
            Assert.Subset(expectedSet, actual);
            Assert.Subset(actual, expectedSet);
        }
    }
}