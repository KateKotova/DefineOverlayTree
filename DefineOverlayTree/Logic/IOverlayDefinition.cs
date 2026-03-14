namespace DefineOverlayTree.Logic
{
    /// <summary>
    ///     Interface for an OverlayDefinition.
    /// </summary>
    public interface IOverlayDefinition
    {
        /// <summary>
        ///     The reference Layer of the Overlay.
        /// </summary>
        ILayer ReferenceLayer { get; }

        /// <summary>
        ///     The shifted Layer of the Overlay.
        /// </summary>
        ILayer ShiftedLayer { get; }

        /// <summary>
        ///     Direction of the Overlay.
        /// </summary>
        Direction Direction { get; }
    }
}