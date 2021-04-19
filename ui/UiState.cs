namespace ui
{
    public class UiState
    {
        public KeyDefinition KeyDefinition { get; set; } = new KeyDefinition();
        
        /// The definition that was validated successfully
        public KeyDefinition? ValidKeyDefinition { get; set; }
    }
}