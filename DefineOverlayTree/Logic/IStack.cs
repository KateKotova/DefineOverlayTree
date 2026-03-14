using System;
using System.Collections.Generic;

namespace DefineOverlayTree.Logic
{
    /// <summary>
    ///     Interface for a Stack.
    /// </summary>
    public interface IStack
    {
        /// <summary>
        ///     List that contains all Layers in the order as they are added.
        /// </summary>
        IList<ILayer> Layers { get; }

        /// <summary>
        ///     The maximum number of Layers
        /// </summary>
        int MaxNrOfLayers { get; }

        /// <summary>
        ///     Event that notifies when the Stack has changed (Layers are added/removed).
        ///     When this event is fired, the previously retrieved list of Layers contains outdated information.
        /// </summary>
        event Action StackChanged;

        /// <summary>
        ///     Indicates if a new Layer can be added or not.
        /// </summary>
        bool CanAddLayer();

        /// <summary>
        ///     Indicates if 'name' for a new Layer is a valid name.
        ///     Names are considered invalid if they are empty, are duplicates of existing Layers or have incorrect format.
        /// </summary>
        /// <param name="name">Name of the Layer.</param>
        bool IsValidLayerName(string name);

        /// <summary>
        ///     <para>
        ///         Adds a new Layer to the list of Layers.
        ///     </para>
        ///     <para>
        ///         Triggers the <see cref="StackChanged" />
        ///     </para>
        ///     <exception cref="InvalidOperationException">
        ///         - IsValidLayerName(name) returns false.
        ///         - CanAddLayer() returns false;
        ///     </exception>
        /// </summary>
        /// <param name="name">Name of the Layer.</param>
        /// <returns>The newly created Layer.</returns>
        ILayer AddLayer(string name);

        /// <summary>
        ///     <para>
        ///         Removes Layer from list of Layers.
        ///     </para>
        ///     <para>
        ///         Triggers the <see cref="StackChanged" /> and/or <see cref="OverlayMapChangedEvent" />
        ///     </para>
        ///     <exception cref="InvalidOperationException">
        ///         - Layer does not exist.
        ///     </exception>
        /// </summary>
        /// <param name="layer">Layer to remove.</param>
        void RemoveLayer(ILayer layer);
    }
}
