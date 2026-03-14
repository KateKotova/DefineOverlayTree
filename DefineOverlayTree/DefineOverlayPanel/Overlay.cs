using DefineOverlayTree.HelperClasses;
using DefineOverlayTree.Logic;

namespace DefineOverlayTree.DefineOverlayPanel
{
    public class Overlay : NotifyPropertyChanged
    {
        public Overlay()
        {
        }

        public Overlay(IOverlay overlayModel)
        {
            OverlayModel = overlayModel;
            IsOverlayDefinitionCompleted = true;
        }

        private string m_Name;
        public string Name
        {
            get => m_Name;
            set
            {
                if (Equals(m_Name, value)) return;
                m_Name = value;
                if (OverlayModel != null)
                {
                    OverlayModel.Name = m_Name;
                }
                OnPropertyChanged();
            }
        }

        private ILayer m_ShiftedLayer;
        public ILayer ShiftedLayer
        {
            get => m_ShiftedLayer;
            set
            {
                if (Equals(m_ShiftedLayer, value)) return;
                m_ShiftedLayer = value;
                SetIsOverlayCompleted();
                OnPropertyChanged();
            }
        }

        private ILayer m_ReferenceLayer;
        public ILayer ReferenceLayer
        {
            get => m_ReferenceLayer;
            set
            {
                if (Equals(m_ReferenceLayer, value)) return;
                m_ReferenceLayer = value;
                SetIsOverlayCompleted();
                OnPropertyChanged();
            }
        }

        private Direction? m_Direction;
        public Direction? Direction
        {
            get => m_Direction;
            set
            {
                if (m_Direction == value) return;
                m_Direction = value;
                SetIsOverlayCompleted();
                OnPropertyChanged();
            }
        }

        private IOverlay m_OverlayModel;
        public IOverlay OverlayModel
        {
            get => m_OverlayModel;
            set
            {
                m_OverlayModel = value;
                SetOverlayProperties(OverlayModel);
            }
        }

        private bool m_IsInEditMode;
        public bool IsInEditMode
        {
            get => m_IsInEditMode;
            set
            {
                if (m_IsInEditMode == value) return;
                m_IsInEditMode = value;
                IsOverlayDefinitionCompleted = !m_IsInEditMode;
                OnPropertyChanged();
            }
        }

        private bool m_IsOverlayDefinitionCompleted;
        public bool IsOverlayDefinitionCompleted
        {
            get => m_IsOverlayDefinitionCompleted;
            set
            {
                if (m_IsOverlayDefinitionCompleted || !value) return;
                m_IsOverlayDefinitionCompleted = true;
                OnPropertyChanged();
            }
        }

        private void SetIsOverlayCompleted()
        {
            IsOverlayDefinitionCompleted = ShiftedLayer != null && ReferenceLayer != null && Direction != null;
        }

        private void SetOverlayProperties(IOverlay overlayModel)
        {
            if (m_OverlayModel == null) return;

            m_Name = overlayModel.Name;
            m_ShiftedLayer = overlayModel.OverlayDefinition.ShiftedLayer;
            m_ReferenceLayer = overlayModel.OverlayDefinition.ReferenceLayer;
            m_Direction = overlayModel.OverlayDefinition.Direction;
        }
    }
}
