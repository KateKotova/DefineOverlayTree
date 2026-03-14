using DefineOverlayTree.DefineOverlayPanel;
using DefineOverlayTree.HelperClasses;
using DefineOverlayTree.Logic;
using DefineOverlayTree.LogicStubs;

namespace DefineOverlayTree
{
    public class MainWindowViewModel : NotifyPropertyChanged
    {
        public const string ApplicationTitle = "Define Overlay Tree";

        public DefineOverlayPanelViewModel DefineOverlayPanel { get; }

        private ITool m_Tool = new ToolStub();

        public MainWindowViewModel()
        {
            DefineOverlayPanel = new DefineOverlayPanelViewModel(m_Tool);
        }

        public void SetSubject(ITool tool)
        {
            m_Tool = tool;
            DefineOverlayPanel.SetSubject(m_Tool);
        }
    }
}