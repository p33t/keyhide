namespace ui
{
    public record DisplayCell
    {
        public static DisplayCell Default = new DisplayCell();
        public char? Content { get; set; }

        public bool IsAvailable { get; set; } = true;
    }
}