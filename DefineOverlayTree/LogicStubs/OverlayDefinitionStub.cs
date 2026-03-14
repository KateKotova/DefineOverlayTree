using DefineOverlayTree.Logic;

namespace DefineOverlayTree.LogicStubs
{
    /// <inheritdoc />
    public class OverlayDefinitionStub : IOverlayDefinition
    {
        /// <inheritdoc />
        public ILayer ReferenceLayer { get; }

        /// <inheritdoc />
        public ILayer ShiftedLayer { get; }

        /// <inheritdoc />
        public Direction Direction { get; }

        public OverlayDefinitionStub(ILayer referenceLayer, ILayer shiftedLayer, Direction direction)
        {
            ReferenceLayer = referenceLayer;
            ShiftedLayer = shiftedLayer;
            Direction = direction;
        }
    }
}
