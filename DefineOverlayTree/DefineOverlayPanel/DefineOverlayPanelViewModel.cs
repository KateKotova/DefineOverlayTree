using DefineOverlayTree.DefineOverlayPanel.GraphicalOverlayView;
using DefineOverlayTree.Logic;

namespace DefineOverlayTree.DefineOverlayPanel
{
    public class DefineOverlayPanelViewModel
    {
        public GraphicalOverlayViewModel GraphicalOverlay { get; }

        private readonly OverlayService m_OverlayService;

        public DefineOverlayPanelViewModel(ITool tool)
        {
            m_OverlayService = new OverlayService(tool.Stack, tool.OverlayMap, tool.OverlayCandidatesService);
            GraphicalOverlay = new GraphicalOverlayViewModel(tool, m_OverlayService);
        }

        public void SetSubject(ITool tool)
        {
            GraphicalOverlay.SetSubject(tool);
            if (tool != null)
            {
                m_OverlayService.SetSubject(tool.Stack, tool.OverlayMap, tool.OverlayCandidatesService);
            }
        }
    }
}