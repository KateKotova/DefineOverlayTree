using DefineOverlayTree.HelperClasses;
using System;
using System.Windows.Input;
using DefineOverlayTree.Logic;

namespace DefineOverlayTree.DefineOverlayPanel.GraphicalOverlayView
{
    public class Cell : NotifyPropertyChanged
    {
        public IOverlay Overlay { get; set; }
        public ILayer Layer { get; set; }

        private bool m_IsActive;
        public bool IsActive
        {
            get => m_IsActive;
            set
            {
                if (value == m_IsActive) return;
                m_IsActive = value;
                OnPropertyChanged();
            }
        }

        private bool m_IsValid = true;
        public bool IsValid
        {
            get => m_IsValid;
            set
            {
                if (value == m_IsValid) return;
                m_IsValid = value;
                OnPropertyChanged();
            }
        }

        public ICommand ClickedCommand { get; }
        public event EventHandler PointToCellRequested;

        public Cell()
        {
            ClickedCommand = new DelegateCommand<object>(o => PointToCellRequested?.Invoke(this, EventArgs.Empty));
        }
    }
}
