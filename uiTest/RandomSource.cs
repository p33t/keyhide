using System;
using System.Collections.Generic;

namespace uiTest
{
    public static class RandomSource
    {
        private static readonly Random RandomInst = new Random();

        // public static IEnumerator<double> PseudoRandomSource = () => 1.0;
        
        public static readonly IEnumerable<double> RandomDoubles = new[]
        {
            // Get these from https://www.random.org/decimal-fractions/?num=768&dec=15&col=1&format=html&rnd=new
            0.610684740481072
        };
    }
}