using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using DefineOverlayTree.HelperClasses;
using DefineOverlayTree.Logic;

namespace DefineOverlayTree.DefineOverlayPanel.GraphicalOverlayView
{
    public class OverlayWithCirclesViewModel : NotifyPropertyChanged
    {
        public Overlay Overlay { get; }

        public EnumWrapper<Direction> Direction { get; private set; }

        public void SetDirection(Direction? direction)
        {
            Direction = direction == null ? null
                : new EnumWrapper<Direction>(direction.Value, Enum.GetNames(typeof(Direction)));
            OnPropertyChanged(nameof(Direction));
        }

        public string Name => Overlay.Name;

        private bool m_IsEnabled = true;
        public bool IsEnabled
        {
            get => m_IsEnabled;
            set
            {
                if (value == m_IsEnabled) return;
                m_IsEnabled = value;
                OnPropertyChanged();
            }
        }

        public IList<ILayer> Layers { get; private set; }

        private IList<ILayer> m_SelectedLayers;
        public IList<ILayer> SelectedLayers
        {
            get => m_SelectedLayers;
            set
            {
                if (value == null) return;
                if (value.Equals(m_SelectedLayers)) return;
                var distinctValues = value.Where(layer => layer != null).ToList();
                var distinctValuesNotMoreThenMax = distinctValues
                    .Take(Math.Min(distinctValues.Count, SelectedLayersMaxCount)).ToList();
                m_SelectedLayers = distinctValuesNotMoreThenMax;
                SetTopAndBottomLayers();
                UpdateLayersSelections();
                UpdateCirclesAndArrows();
            }
        }

        public void UpdateLayersSelections()
        {
            OnPropertyChanged(nameof(SelectedLayers));
            OnPropertyChanged(nameof(IsFilled));
            OnPropertyChanged(nameof(HasReversedLayers));
            OnPropertyChanged(nameof(IsInEditMode));
            OnPropertyChanged(nameof(HasNoSelectedLayers));
        }

        public void UpdateCircles()
        {
            OnPropertyChanged(nameof(Circle0Y));
            OnPropertyChanged(nameof(Circle1Y));
            OnPropertyChanged(nameof(Circle0IsVisible));
            OnPropertyChanged(nameof(Circle1IsVisible));
        }

        public void UpdateArrows()
        {
            OnPropertyChanged(nameof(ArrowPointY));
            OnPropertyChanged(nameof(ArrowLineStartY));
            OnPropertyChanged(nameof(ArrowLineLength));
        }

        public void UpdateCirclesAndArrows()
        {
            UpdateCircles();
            UpdateArrows();
        }

        public const int SelectedLayersMaxCount = 2;

        public bool IsFilled => SelectedLayers?.Count == SelectedLayersMaxCount
            && SelectedLayers?.Count == SelectedLayers?.Distinct().Count();
        public bool IsInEditMode => SelectedLayers?.Count == 1 && Overlay.IsInEditMode;
        public bool HasNoSelectedLayers => !SelectedLayers?.Any() ?? true;
        public bool HasReversedLayers => SelectedLayers?.Count > 1 && Layers.IndexOf(SelectedLayers[1])
            >= Layers.IndexOf(SelectedLayers[0]);

        public ILayer TopLayer { get; private set; }

        public ILayer BottomLayer { get; private set; }

        public ICommand CircleMouseDownCommand { get; private set; }
        public ICommand Layer0CircleMouseDownCommand { get; private set; }
        public ICommand Layer1CircleMouseDownCommand { get; private set; }

        public bool Circle0IsVisible => SelectedLayers?.Any() ?? false;
        public bool Circle1IsVisible => IsFilled;

        public int Circle0Y
        {
            get
            {
                if (!Circle0IsVisible) return 0;
                var layerIndex = Layers.IndexOf(SelectedLayers[0]);
                return layerIndex < 0 ? 0 : GetCircleY(layerIndex);
            }
        }

        public int Circle1Y
        {
            get
            {
                if (SelectedLayers == null || SelectedLayers.Count < 2) return 0;
                var layerIndex = Layers.IndexOf(SelectedLayers[1]);
                return layerIndex < 0 ? 0 : GetCircleY(layerIndex);
            }
        }

        private int GetCircleY(int layerIndex) => GetCellY(layerIndex) + GraphicalOverlaySizes.CircleYRelativeToCell;

        private int GetCellY(int layerIndex) => (Layers.Count - layerIndex - 1)
            * GraphicalOverlaySizes.OuterLayerRowWithMarginHeight;

        public int ArrowPointY
        {
            get
            {
                if (!IsFilled || BottomLayer == null) return 0;
                var bottomLayerIndex = Layers.IndexOf(BottomLayer);
                return GetCellY(bottomLayerIndex) + GraphicalOverlaySizes.ArrowPointYRelativeToCell;
            }
        }

        public int ArrowLineStartY
        {
            get
            {
                if (!IsFilled || TopLayer == null) return 0;
                var topLayerIndex = Layers.IndexOf(TopLayer);
                return GetCellY(topLayerIndex) + GraphicalOverlaySizes.ArrowLineStartYRelativeToCell;
            }
        }

        public int ArrowLineLength
        {
            get
            {
                if (!IsFilled || TopLayer == null || BottomLayer == null) return 0;
                var bottomLayerIndex = Layers.IndexOf(BottomLayer);
                var arrowLineEndY = GetCellY(bottomLayerIndex) + GraphicalOverlaySizes.ArrowLineEndYRelativeToCell;
                return arrowLineEndY - ArrowLineStartY;
            }
        }

        public void SelectLayer(ILayer layer)
        {
            if (IsFilled || SelectedLayers != null && SelectedLayers.Contains(layer)) return;
            if (SelectedLayers == null)
            {
                SelectedLayers = new List<ILayer>();
            }
            SelectedLayers.Add(layer);
            UpdateLayersSelections();
            UpdateCircles();
            SetTopAndBottomLayers();
            if (IsFilled)
            {
                Overlay.ShiftedLayer = TopLayer;
                Overlay.ReferenceLayer = BottomLayer;
            }
        }

        private void SetTopAndBottomLayers()
        {
            if (!IsFilled)
            {
                BottomLayer = null;
                TopLayer = null;
            }
            else
            {
                var layer0Index = Layers.IndexOf(SelectedLayers[0]);
                var layer1Index = Layers.IndexOf(SelectedLayers[1]);
                if (layer0Index < layer1Index)
                {
                    BottomLayer = SelectedLayers[0];
                    TopLayer = SelectedLayers[1];
                }
                else
                {
                    BottomLayer = SelectedLayers[1];
                    TopLayer = SelectedLayers[0];
                }
            }
            OnPropertyChanged(nameof(BottomLayer));
            OnPropertyChanged(nameof(TopLayer));
            UpdateArrows();
        }

        public event EventHandler<LayerEventArgs> DeselectLayerRequested;

        private void DeselectLayer(ILayer layer)
        {
            if (HasNoSelectedLayers || !SelectedLayers.Contains(layer)) return;

            if (IsFilled)
            {
                BottomLayer = null;
                TopLayer = null;
                OnPropertyChanged(nameof(BottomLayer));
                OnPropertyChanged(nameof(TopLayer));
                UpdateArrows();
            }

            SelectedLayers.Remove(layer);
            UpdateLayersSelections();
            UpdateCircles();
        }

        public bool LayerIsSelected(ILayer layer) => SelectedLayers?.Contains(layer) ?? false;

        public void RemoveLayer(ILayer layer)
        {
            if (LayerIsSelected(layer))
            {
                DeselectLayer(layer);
                Layers.Remove(layer);
                OnPropertyChanged(nameof(Layers));
            }
        }

        public void SetLayers(IEnumerable<ILayer> layers)
        {
            Layers = layers?.ToList() ?? new List<ILayer>();
            OnPropertyChanged(nameof(Layers));
            if (m_SelectedLayers == null) return;
            SelectedLayers = m_SelectedLayers.Intersect(Layers).ToList();
        }

        public void CircleMouseDownExecute(object layerIndexObject)
        {
            if (layerIndexObject == null) return;
            int layerIndex;
            try
            {
                layerIndex = Convert.ToInt32(layerIndexObject);
            }
            catch
            {
                return;
            }
            if (HasNoSelectedLayers || layerIndex < 0 || layerIndex >= SelectedLayers.Count) return;
            var layer = SelectedLayers[layerIndex];
            DeselectLayer(layer);
            DeselectLayerRequested?.Invoke(this, new LayerEventArgs(layer));
        }

        private void LayerCircleMouseDownExecute(int layerIndex)
        {
            if (HasNoSelectedLayers || layerIndex >= SelectedLayers.Count) return;
            var layer = SelectedLayers[layerIndex];
            DeselectLayer(layer);
            DeselectLayerRequested?.Invoke(this, new LayerEventArgs(layer));
        }

        public void Layer0CircleMouseDownExecute(object o)
        {
            LayerCircleMouseDownExecute(0);
        }

        public void Layer1CircleMouseDownExecute(object o)
        {
            LayerCircleMouseDownExecute(1);
        }

        private void CreateCommands()
        {
            CircleMouseDownCommand = new DelegateCommand<object>(CircleMouseDownExecute);
            Layer0CircleMouseDownCommand = new DelegateCommand<object>(Layer0CircleMouseDownExecute);
            Layer1CircleMouseDownCommand = new DelegateCommand<object>(Layer1CircleMouseDownExecute);
        }

        public OverlayWithCirclesViewModel(Overlay overlay, IEnumerable<ILayer> layers)
        {
            Overlay = overlay ?? throw new ArgumentNullException(nameof(Overlay));

            CreateCommands();
            SetLayers(layers);
            SetDirection(overlay.Direction);
            SelectedLayers = new List<ILayer>
            {
                Layers.FirstOrDefault(layer => layer?.Name == overlay.ShiftedLayer?.Name),
                Layers.FirstOrDefault(layer => layer?.Name == overlay.ReferenceLayer?.Name)
            };
        }
    }
}
