using System.Collections.Generic;
using System.Linq;
using DefineOverlayTree.Logic;

namespace DefineOverlayTree.LogicStubs
{
    /// <inheritdoc />
    public class OverlayCandidatesServiceStub : IOverlayCandidatesService
    {
        public IList<IOverlayDefinition> OverlayCandidates { get; } = new List<IOverlayDefinition>();

        /// <inheritdoc />
        public IList<IOverlayDefinition> GetOverlayCandidates() => OverlayCandidates;

        /// <inheritdoc />
        public IList<IOverlayDefinition> GetOverlayCandidates(IOverlayDefinition overlayDefinition)
            => OverlayCandidates.Concat(new List<IOverlayDefinition> { overlayDefinition }).ToList();
    }
}
