namespace DefineOverlayTree.Logic
{
    /// <summary>
    ///     Interface for an Overlay.
    /// </summary>
    public interface IOverlay
    {
        /// <summary>
        ///     Name of the Overlay.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        ///     The definition of the Overlay.
        /// </summary>
        IOverlayDefinition OverlayDefinition { get; set; }
    }
}