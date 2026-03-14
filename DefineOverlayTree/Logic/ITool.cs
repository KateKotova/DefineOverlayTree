namespace DefineOverlayTree.Logic
{
    /// <summary>
    ///     Interface for the tool.
    /// </summary>
    public interface ITool
    {
        /// <summary>
        ///     The Stack of the tool.
        /// </summary>
        IStack Stack { get; }

        /// <summary>
        ///     The OverlayMap of the tool.
        /// </summary>
        IOverlayMap OverlayMap { get; }

        /// <summary>
        ///     The OverlayCandidatesService of the tool.
        /// </summary>
        IOverlayCandidatesService OverlayCandidatesService { get; }
    }
}
