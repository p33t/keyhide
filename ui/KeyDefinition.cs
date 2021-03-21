namespace ui
{
    public record KeyDefinition
    {
        public string KeyString { get; init; } = string.Empty;

        public KeyCharSetEnum? CharSet { get; init; }

        /// <summary>
        /// Comma-separated list of fragment sizes that will repeat unless '0' is supplied, indicating 'rest of string'.
        /// E.g.  Key '123-45-678' could have values '3,2', '3,2,3', '3,2,0'.  For security purposes, '3,2' provides the most obscurity.
        /// </summary>
        public string? FragmentCycle { get; init; }

        public char? Separator { get; init; }
        
        public string? CustomCharset { get; init; }
    }
}