using DefineOverlayTree.Logic;

namespace DefineOverlayTree.LogicStubs
{
    /// <inheritdoc />
    public class OverlayStub : IOverlay
    {
        /// <inheritdoc />
        public string Name { get; set; }

        /// <inheritdoc />
        public IOverlayDefinition OverlayDefinition { get; set; }
    }
}
