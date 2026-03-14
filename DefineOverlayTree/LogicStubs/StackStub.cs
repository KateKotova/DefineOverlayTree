using System;
using System.Collections.Generic;
using System.Linq;
using DefineOverlayTree.Logic;

namespace DefineOverlayTree.LogicStubs
{
    /// <inheritdoc />
    public class StackStub : IStack
    {
        /// <inheritdoc />
        public IList<ILayer> Layers { get; } = new List<ILayer>();

        /// <inheritdoc />
        public int MaxNrOfLayers { get; } = 10;

        /// <inheritdoc />
        public event Action StackChanged;

        /// <inheritdoc />
        public bool CanAddLayer() => Layers.Count < MaxNrOfLayers;

        /// <inheritdoc />
        public bool IsValidLayerName(string name) => !string.IsNullOrWhiteSpace(name) && !Layers.Any(l => l.Name.Equals(name));

        /// <inheritdoc />
        public ILayer AddLayer(string name)
        {
            if (!IsValidLayerName(name) || !CanAddLayer())
            {
                throw new InvalidOperationException("Invalid layer name or the maximum layer count is already exist.");
            }
            var layer = new LayerStub(name);
            Layers.Add(layer);
            StackChanged?.Invoke();
            return layer;
        }

        /// <inheritdoc />
        public void RemoveLayer(ILayer layer)
        {
            if (!Layers.Contains(layer))
            {
                throw new InvalidOperationException("Layer does not exist.");
            }
            Layers.Remove(layer);
            StackChanged?.Invoke();
        }
    }
}
