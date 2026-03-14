using System;
using System.Collections.Generic;
using System.Linq;
using DefineOverlayTree.Logic;

namespace DefineOverlayTree.LogicStubs
{
    /// <inheritdoc />
    public class OverlayMapStub : IOverlayMap
    {
        /// <inheritdoc />
        public IList<IOverlay> Overlays { get; } = new List<IOverlay>();

        /// <inheritdoc />
        public int MaxNrOfOverlays { get; } = 4;

        /// <inheritdoc />
        public event Action OverlayMapChanged;

        /// <inheritdoc />
        public bool CanAddOverlay() => Overlays.Count < MaxNrOfOverlays;

        /// <inheritdoc />
        public bool IsValidOverlayName(string name) => !string.IsNullOrWhiteSpace(name) && !Overlays.Any(o => o.Name.Equals(name));

        private IOverlayCandidatesService m_OverlayCandidatesService;

        /// <inheritdoc />
        public IOverlay AddOverlay(string name, IOverlayDefinition overlayDefinition)
        {
            if (!IsValidOverlayName(name) || !CanAddOverlay()
                || !m_OverlayCandidatesService.GetOverlayCandidates().Any(
                    c => c.ReferenceLayer.Name.Equals(overlayDefinition.ReferenceLayer.Name)
                    && c.ShiftedLayer.Name.Equals(overlayDefinition.ShiftedLayer.Name)
                    && c.Direction == overlayDefinition.Direction))
            {
                throw new InvalidOperationException("Invalid overlay name or the maximum overlay count is already exist.");
            }
            var overlay = new OverlayStub{ Name = name, OverlayDefinition = overlayDefinition };
            Overlays.Add(overlay);
            OverlayMapChanged?.Invoke();
            return overlay;
        }

        /// <inheritdoc />
        public IOverlay InsertOverlay(int index, string name, IOverlayDefinition overlayDefinition)
        {
            if (!IsValidOverlayName(name) || !CanAddOverlay()
                || !m_OverlayCandidatesService.GetOverlayCandidates().Any(
                    c => c.ReferenceLayer.Name.Equals(overlayDefinition.ReferenceLayer.Name)
                         && c.ShiftedLayer.Name.Equals(overlayDefinition.ShiftedLayer.Name)
                         && c.Direction == overlayDefinition.Direction))
            {
                throw new InvalidOperationException("Invalid overlay name or the maximum overlay count is already exist.");
            }
            if (index < 0 || index > Overlays.Count)
            {
                throw new ArgumentOutOfRangeException($"Index is out of range (less then zero or greater then Overlays.Count).");
            }
            var overlay = new OverlayStub { Name = name, OverlayDefinition = overlayDefinition };
            Overlays.Insert(index, overlay);
            OverlayMapChanged?.Invoke();
            return overlay;
        }

        /// <inheritdoc />
        public bool CanRemoveOverlay(IOverlay overlay) => Overlays.Contains(overlay);

        /// <inheritdoc />
        public void RemoveOverlay(IOverlay overlay)
        {
            if (!Overlays.Contains(overlay))
            {
                throw new InvalidOperationException("Overlay does not exist");
            }
            Overlays.Remove(overlay);
            OverlayMapChanged?.Invoke();
        }

        public OverlayMapStub(IOverlayCandidatesService overlayCandidatesService)
        {
            m_OverlayCandidatesService = overlayCandidatesService;
        }
    }
}