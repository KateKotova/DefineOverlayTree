using DefineOverlayTree.HelperClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefineOverlayTree.DefineOverlayPanel.GraphicalOverlayView
{
    public class OverlayName : NotifyPropertyChanged
    {
        public const string OverlayDefaultNameBase = "OV";

        private int m_Index;
        public int Index
        {
            get => m_Index;
            set
            {
                if (value == m_Index) return;
                m_Index = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DefaultName));
            }
        }

        public string DefaultName => OverlayDefaultNameBase + Index;

        private string m_Name;
        public string Name
        {
            get => m_Name;
            set
            {
                if (value == m_Name) return;
                m_Name = value;
                OnPropertyChanged();
            }
        }
    }
}
