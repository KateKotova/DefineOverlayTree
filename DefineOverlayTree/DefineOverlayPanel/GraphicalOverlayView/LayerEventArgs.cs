using System;
using DefineOverlayTree.Logic;

namespace DefineOverlayTree.DefineOverlayPanel.GraphicalOverlayView
{
    public class LayerEventArgs : EventArgs
    {
        public ILayer Layer { get; set; }

        public LayerEventArgs(ILayer layer)
        {
            Layer = layer;
        }
    }
}
