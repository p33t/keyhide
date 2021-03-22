namespace ui
{
    public record KeyDefinition
    {
        public string KeyString { get; init; } = string.Empty;

        public KeyCharSetEnum? CharSet { get; init; }

        public char? Separator { get; init; }
        
        public string? CustomCharset { get; init; }
    }
}