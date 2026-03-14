using System.Collections.Generic;

namespace DefineOverlayTree.Logic
{
    /// <summary>
    ///     Interface for the OverlayCandidatesService.
    /// </summary>
    public interface IOverlayCandidatesService
    {
        /// <summary>
        ///     Returns a list of candidate overlays that can be used to add new Overlays to the OverlapMap.
        /// </summary>
        /// <returns>The list of candidate overlays.</returns>
        IList<IOverlayDefinition> GetOverlayCandidates();

        /// <summary>
        ///     Returns a list of candidate overlays that can be used to update an existing Overlay
        ///     with a new OverlayDefinition in the OverlayMap.
        /// </summary>
        /// <returns>The list of candidate overlays.</returns>
        IList<IOverlayDefinition> GetOverlayCandidates(IOverlayDefinition overlayDefinition);
    }
}