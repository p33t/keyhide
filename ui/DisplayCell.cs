namespace ui
{
    public record DisplayCell
    {
        public char? Content { get; set; }

        public bool IsAvailable { get; set; } = true;

        /// <summary>
        /// Some cells are on the path but will still have no content.  These are the 'subtracted' cells.
        /// </summary>
        public bool IsOnPath { get; set; } = false;
    }
}