using System;

namespace ui
{
    public class FinalModel
    {
        public string[] Grid { get; set; } = Array.Empty<string>();

        public string Prefix { get; set; } = string.Empty;

        public string Suffix { get; set; } = string.Empty;

        public int Subtract { get; set; } = 0;

        public int[] FragmentCycles { get; set; } = Array.Empty<int>();

        public string FragmentSeparator { get; set; } = string.Empty;
    }
}