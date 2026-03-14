using System;
using System.Collections.Generic;

namespace DefineOverlayTree.Logic
{
    /// <summary>
    ///     Interface for an OverlayMap.
    /// </summary>
    public interface IOverlayMap
    {
        /// <summary>
        ///     List that contains all Overlays in the order as they are added.
        /// </summary>
        IList<IOverlay> Overlays { get; }

        /// <summary>
        ///     The maximum number of overlays.
        /// </summary>
        int MaxNrOfOverlays { get; }

        /// <summary>
        ///     Event that notifies when the OverlayMap has changed (Overlays are added/removed).
        ///     When this event is fired, the previously retrieved list of Overlays contains outdated information.
        /// </summary>
        event Action OverlayMapChanged;

        /// <summary>
        ///     Indicates if a new Overlay can be added or not.
        /// </summary>
        bool CanAddOverlay();

        /// <summary>
        ///     Indicates if 'name' for a new Overlay is a valid name.
        ///     Names are considered invalid if they are empty, are duplicates of existing Overlays or have incorrect format.
        /// </summary>
        /// <param name="name"></param>
        bool IsValidOverlayName(string name);

        /// <summary>
        ///     <para>
        ///         Adds a new Overlay to the list of Overlays.
        ///     </para>
        ///     <para>
        ///         Triggers the <see cref="OverlayMapChanged" />
        ///     </para>
        ///     <exception cref="InvalidOperationException">
        ///         - IsValidOverlayName(name) returns false.
        ///         - CanAddOverlay() returns false.
        ///         - overlayDefinition is not available in the CandidateOverlays list.
        ///     </exception>
        /// </summary>
        /// <param name="name">Name of the Overlay.</param>
        /// <param name="overlayDefinition">Definition of the Overlay.</param>
        /// <returns>The newly created Overlay.</returns>
        IOverlay AddOverlay(string name, IOverlayDefinition overlayDefinition);

        /// <summary>
        ///     <para>
        ///         Inserts a new Overlay to the list of Overlays.
        ///     </para>
        ///     <para>
        ///         Triggers the <see cref="OverlayMapChanged" />
        ///     </para>
        ///     <exception cref="InvalidOperationException">
        ///         - IsValidOverlayName(name) returns false.
        ///         - CanAddOverlay() returns false.
        ///         - overlayDefinition is not available in the CandidateOverlays list.
        ///     </exception>
        ///     <exception cref="ArgumentOutOfRangeException">
        ///         - index out of range (less then zero or greater then Overlays.Count).
        ///     </exception>
        /// </summary>
        /// <param name="index">Index in the Overlays list.</param>
        /// <param name="name">Name of the Overlay.</param>
        /// <param name="overlayDefinition">Definition of the Overlay.</param>
        /// <returns>The newly created Overlay.</returns>
        IOverlay InsertOverlay(int index, string name, IOverlayDefinition overlayDefinition);

        /// <summary>
        ///     Indicates if an existing Overlay can be removed or not.
        /// </summary>
        bool CanRemoveOverlay(IOverlay overlay);

        /// <summary>
        ///     <para>
        ///         Removes Overlay from list of Overlays.
        ///     </para>
        ///     <para>
        ///         Triggers the <see cref="OverlayMapChanged" />
        ///     </para>
        ///     <exception cref="InvalidOperationException">
        ///         - Overlay does not exist.
        ///     </exception>
        /// </summary>
        /// <param name="overlay">Overlay to remove.</param>
        void RemoveOverlay(IOverlay overlay);
    }
}
