namespace ui
{
    public class UiState
    {
        public KeyDefinition KeyDefinition { get; set; } = new KeyDefinition();
        
        /// The definition that was validated successfully
        public KeyDefinition? ValidKeyDefinition { get; set; }

        public PathDefinition PathDefinition { get; set; } = new PathDefinition
        {
            Coords = new[] {CellCoord.Origin, CellCoord.Origin}
        };
    }
}