using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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

        [Fact]
        public async Task CanLoadAssemblyResource()
        {
            var assembly = Assembly.GetAssembly(typeof(UiState));
            var fileName = $"{assembly!.GetName().Name}.etc.deliverable.html";
            await using var resFilestream = assembly.GetManifestResourceStream(fileName);
            Assert.NotNull(resFilestream);
            var ms = new MemoryStream();
            await resFilestream!.CopyToAsync(ms);
            var str = Encoding.UTF8.GetString(ms.ToArray());
            await ms.DisposeAsync();
            // _testOutputHelper.WriteLine(str);
        }

        [Fact] //(Skip = "Not generating ATM")]
        public void ScratchMethod()
        {
            foreach (var charSet in Enum.GetValues<KeyCharSetEnum>())
            {
                _testOutputHelper.WriteLine($"{charSet}:\t{KeyAnalyzer.CharSetFor(charSet).AsString()}");
            }

            // 1Password Secret Key
            // Prefix: A3-
            // FragmentCycles: 5
            // Key length: 30
            // Charset: Upper Alpha Numeric without IOU10

            // Bit warden time-based one-time password
            // Key Length: 32
            // Charset: Upper Alpha Numeric without 1890
            
            // var chars = RandomCharacter.GenerateCharacters("PLACE_KEY_HERE", 24 * 32, upperCaseAlphaNumericSans1890, 
            //     RandomSource.RandomDoubles.GetEnumerator());
            // for (var index = 0; index < chars.Count; index+= 24)
            // {
            //     var segment = chars.Skip(index).Take(24);
            //     _testOutputHelper.WriteLine(string.Join(null, segment));
            // }      
        }
    }
}