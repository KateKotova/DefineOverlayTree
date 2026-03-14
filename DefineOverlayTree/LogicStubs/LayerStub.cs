using DefineOverlayTree.Logic;

namespace DefineOverlayTree.LogicStubs
{
    /// <inheritdoc />
    public class LayerStub : ILayer
    {
        /// <inheritdoc />
        public string Name { get; set; }

        public LayerStub(string name)
        {
            Name = name;
        }
    }
}
