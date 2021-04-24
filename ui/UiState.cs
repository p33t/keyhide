namespace ui
{
    public class UiState
    {
        public KeyDefinition KeyDefinition { get; set; } = new KeyDefinition();
        
        /// The definition that was validated successfully
        public KeyDefinition? ValidKeyDefinition { get; set; }

        public GridSetupModel GridSetup { get; set; } = new GridSetupModel();

        public GridSetupModel? ValidGridSetup { get; set; }

        public PathDefinition PathDefinition { get; set; } = new PathDefinition();

        public PathDefinition? ValidPathDefinition { get; set; }
    }
}