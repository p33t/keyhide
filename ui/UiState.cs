using System;
using System.Linq;

namespace ui
{
    public class UiState
    {
        private KeyDefinition? _validKeyDefinition;
        private PathDefinition? _validPathDefinition;
        private DateTime? _finalModelTimestamp;
        private FinalModel? _finalModel;
        public KeyDefinition KeyDefinition { get; set; } = new KeyDefinition();

        /// The definition that was validated successfully
        public KeyDefinition? ValidKeyDefinition
        {
            get => _validKeyDefinition;
            set
            {
                if (Equals(ValidKeyDefinition, value))
                    return;

                _validKeyDefinition = value;
                PathDefinition.EffectiveKeyString = ValidKeyDefinition?.EffectiveKeyString;
                PathDefinition.Coords = Enumerable.Empty<CellCoord>();
                ValidPathDefinition = null;
            }
        }

        public PathDefinition PathDefinition { get; set; } = new PathDefinition();

        public PathDefinition? ValidPathDefinition
        {
            get => _validPathDefinition;
            set
            {
                if (Equals(ValidPathDefinition, value))
                    return;

                _validPathDefinition = value;
                FinalModel = null;
            }
        }

        public FinalModel? FinalModel
        {
            get => _finalModel;
            set
            {
                if (Equals(_finalModel, value))
                    return;
                
                _finalModel = value;
                if (value != null)
                {
                    _finalModelTimestamp = DateTime.Now;
                }
            }
        }

        public DateTime? FinalModelTimestamp => _finalModelTimestamp;
    }
}