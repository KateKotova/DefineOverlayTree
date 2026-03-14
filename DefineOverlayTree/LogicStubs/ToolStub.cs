using System;
using System.Linq;
using DefineOverlayTree.Logic;

namespace DefineOverlayTree.LogicStubs
{
    /// <inheritdoc />
    public class ToolStub : ITool
    {
        /// <inheritdoc />
        public IStack Stack { get; } = new StackStub();

        /// <inheritdoc />
        public IOverlayMap OverlayMap { get; }

        /// <inheritdoc />
        public IOverlayCandidatesService OverlayCandidatesService { get; } = new OverlayCandidatesServiceStub();

        public ToolStub()
        {
            OverlayMap = new OverlayMapStub(OverlayCandidatesService);
            Stack.StackChanged += OnStackOrOverlayMapChanged;
            OverlayMap.OverlayMapChanged += OnStackOrOverlayMapChanged;
        }

        private void OnStackOrOverlayMapChanged()
        {
            if (!(OverlayCandidatesService is OverlayCandidatesServiceStub overlayCandidatesService)) return;
            overlayCandidatesService.OverlayCandidates.Clear();

            for (var referencedLayerIndex = 0;
                referencedLayerIndex < Stack.Layers.Count - 1;
                referencedLayerIndex++)
            {
                for (var shiftedLayerIndex = referencedLayerIndex + 1;
                    shiftedLayerIndex < Stack.Layers.Count;
                    shiftedLayerIndex++)
                {
                    foreach (var direction in (Direction[]) Enum.GetValues(typeof(Direction)))
                    {
                        var referencedLayer = Stack.Layers[referencedLayerIndex];
                        var shiftedLayer = Stack.Layers[shiftedLayerIndex];

                        if (OverlayMap.Overlays.Any(
                            o => o.OverlayDefinition.ReferenceLayer.Name.Equals(referencedLayer.Name)
                                 && o.OverlayDefinition.ShiftedLayer.Name.Equals(shiftedLayer.Name)
                                 && o.OverlayDefinition.Direction == direction)) continue;

                        overlayCandidatesService.OverlayCandidates.Add(new OverlayDefinitionStub(
                            referencedLayer, shiftedLayer, direction));
                    }
                }
            }
        }
    }
}