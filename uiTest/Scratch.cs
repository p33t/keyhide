using System;
using System.Collections.Generic;
using System.Linq;
using ui;
using Xunit;
using Xunit.Abstractions;

namespace uiTest
{
    public class Scratch
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public Scratch(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact(Skip = "Not generating ATM")]
        public void ScratchMethod()
        {
            // 1Password Secret Key
            // Prefix: A3-
            // FragmentCycles: 5
            // Key length: 30
            // Charset: Upper Alpha Numeric without IOU10
            var upperCaseAlphaNumericSansIOU10 = new HashSet<char>("23456789ABCDEFGHJKLMNPQRSTVWXYZ");

            var upperAlphaNumeric = new HashSet<char>(KeyAnalyzer.CharSetFor(KeyCharSetEnum.AlphabetUpper));
            foreach (var c in KeyAnalyzer.CharSetFor(KeyCharSetEnum.Numeric))
                upperAlphaNumeric.Add(c);
            
            // Bit warden time-based one-time password
            // Key Length: 32
            // Charset: Upper Alpha Numeric without 1890
            var upperCaseAlphaNumericSans1890 = new HashSet<char>(upperAlphaNumeric);
            upperCaseAlphaNumericSans1890.Remove('1');
            upperCaseAlphaNumericSans1890.Remove('8');
            upperCaseAlphaNumericSans1890.Remove('9');
            upperCaseAlphaNumericSans1890.Remove('0');
            
            var chars = RandomCharacter.GenerateCharacters("PLACE_KEY_HERE", 24 * 32, upperCaseAlphaNumericSans1890, 
                RandomSource.RandomDoubles.GetEnumerator());
            for (var index = 0; index < chars.Count; index+= 24)
            {
                var segment = chars.Skip(index).Take(24);
                _testOutputHelper.WriteLine(string.Join(null, segment));
            }      
        }
    }
}