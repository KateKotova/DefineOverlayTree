using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DefineOverlayTree.HelperClasses;
using DefineOverlayTree.Logic;

namespace DefineOverlayTree.DefineOverlayPanel.GraphicalOverlayView
{
    public class GraphicalOverlayViewModel : NotifyPropertyChanged
    {
        private ITool m_ToolModel;
        private readonly OverlayService m_OverlayService;

        public GraphicalOverlayViewModel(ITool toolModel, OverlayService overlayService)
        {
            m_OverlayService = overlayService ?? throw new ArgumentNullException(nameof(OverlayService));
            m_OverlayService.OverlaysChanged += OnOverlaysChanged;

            Overlays = new ObservableCollection<OverlayWithCirclesViewModel>();
            OverlayNames = new ObservableCollection<OverlayName>();
            for (var i = 0; i < m_OverlayService.MaxOverlaysCount;)
            {
                OverlayNames.Add(new OverlayName { Index = ++i });
            }

            CreateCommands();
            Init(toolModel);
        }

        public ObservableCollection<OverlayName> OverlayNames { get; }

        public ObservableCollection<OverlayWithCirclesViewModel> Overlays { get; }

        public string NewLayerDefaultName => NamingService.LayerDefaultNameBuilder.GetNextDefaultName
            (Layers?.Select(layer => layer.Name).ToList());

        private ObservableCollection<LayerViewModel> m_Layers;

        private void CreateCommands()
        {
            AddLayer = new DelegateCommand<object>(AddLayerExecute, AddLayerCanExecute);
        }

        public void SetSubject(ITool toolModel)
        {
            Init(toolModel);
        }

        private void Init(ITool toolModel)
        {
            UnsubscribeToolModel(m_ToolModel);
            m_ToolModel = toolModel;
            FillLayers(m_ToolModel?.Stack?.Layers);
            FillOverlayGraphical();
            SubscribeToolModel(m_ToolModel);
        }

        public ObservableCollection<LayerViewModel> Layers
        {
            get => m_Layers;
            set
            {
                if (value == m_Layers) return;
                if (m_Layers != null)
                {
                    foreach (var layer in m_Layers)
                    {
                        UnsubscribeLayer(layer);
                    }
                }
                m_Layers = value;
                if (m_Layers != null)
                {
                    ReversedLayers = new ObservableCollection<LayerViewModel>(m_Layers.Reverse());
                    for (var layerIndex = 0; layerIndex < m_Layers.Count; layerIndex++)
                    {
                        var layer = m_Layers[layerIndex];
                        layer.IsFirst = layerIndex == 0;
                        SubscribeLayer(layer);
                    }
                }
                else
                {
                    ReversedLayers = null;
                }
                OnLayersChanged();
            }
        }

        private void SubscribeOverlay(OverlayWithCirclesViewModel overlay)
        {
            overlay.DeselectLayerRequested += OnDeselectLayerPointRequested;
        }

        private void UnsubscribeOverlay(OverlayWithCirclesViewModel overlay)
        {
            overlay.DeselectLayerRequested -= OnDeselectLayerPointRequested;
        }

        private void SubscribeLayer(LayerViewModel layer)
        {
            layer.DeleteRequested += OnLayerDeleteRequested;
            layer.PropertyChanged += OnLayerPropertyChanged;
            layer.CellSelected += OnSelectLayerPointRequested;
        }

        private void UnsubscribeLayer(LayerViewModel layer)
        {
            layer.DeleteRequested -= OnLayerDeleteRequested;
            layer.PropertyChanged -= OnLayerPropertyChanged;
            layer.CellSelected -= OnSelectLayerPointRequested;
        }

        private void SubscribeToolModel(ITool toolModel)
        {
            if (toolModel?.Stack != null)
            {
                toolModel.Stack.StackChanged += OnStackChanged;
            }
        }

        private void UnsubscribeToolModel(ITool toolModel)
        {
            if (toolModel?.Stack != null)
            {
                toolModel.Stack.StackChanged -= OnStackChanged;
            }
        }

        private void OnOverlaysChanged(object sender, EventArgs e)
        {
            FillOverlayGraphical();
        }

        private void OnLayerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(LayerViewModel.Name))
            {
                OnPropertyChanged(nameof(NewLayerDefaultName));
                OnPropertyChanged(nameof(NewLayerNotEmptyName));
            }
        }

        public ObservableCollection<LayerViewModel> ReversedLayers { get; private set; }

        private void OnLayersChanged()
        {
            OnPropertyChanged(nameof(Layers));
            OnPropertyChanged(nameof(ReversedLayers));
            OnPropertyChanged(nameof(MaximumLayersCountMessage));
            OnPropertyChanged(nameof(NewLayerDefaultName));
            OnPropertyChanged(nameof(NewLayerNotEmptyName));

            foreach (var overlay in Overlays)
            {
                overlay.SetLayers(m_ToolModel?.Stack?.Layers);
            }
        }

        private string m_NewLayerName;
        public string NewLayerName
        {
            get => m_NewLayerName;
            set
            {
                if (value == m_NewLayerName) return;
                m_NewLayerName = value;
                OnPropertyChanged(nameof(NewLayerName));
                OnPropertyChanged(nameof(NewLayerNotEmptyName));
            }
        }

        public string MaximumLayersCountMessage => m_ToolModel?.Stack == null ? string.Empty
            : $"* Up to {m_ToolModel.Stack.MaxNrOfLayers} layers";

        public string NewLayerNotEmptyName => string.IsNullOrWhiteSpace(NewLayerName) ? NewLayerDefaultName : NewLayerName.Trim();

        public ICommand AddLayer { get; private set; }

        private bool AddLayerCanExecute(object obj)
        {
            return m_ToolModel?.Stack != null && m_ToolModel.Stack.CanAddLayer()
                && m_ToolModel.Stack.IsValidLayerName(NewLayerNotEmptyName);
        }

        private void AddLayerExecute(object obj)
        {
            try
            {
                m_ToolModel.Stack.AddLayer(NewLayerNotEmptyName);
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("An exception occured while adding the layer", "Could not add layer");
            }
            NewLayerName = string.Empty;
        }

        private void AddNewLayerViewModel(ILayer layer, int index)
        {
            var layerViewModel = new LayerViewModel(layer, m_ToolModel, m_OverlayService, m_ToolModel.Stack.IsValidLayerName);
            if (index >= Layers.Count)
            {
                Layers.Add(layerViewModel);
                ReversedLayers.Insert(0, layerViewModel);
                layerViewModel.IsFirst = Layers.Count == 1;
            }
            else
            {
                Layers.Insert(index, layerViewModel);
                layerViewModel.IsFirst = index == 0;
                if (layerViewModel.IsFirst)
                {
                    Layers[1].IsFirst = false;
                    ReversedLayers.Add(layerViewModel);
                }
                else
                {
                    ReversedLayers.Insert(Layers.Count - index, layerViewModel);
                }
            }
            SubscribeLayer(layerViewModel);
        }

        private void OnLayerDeleteRequested(object sender, EventArgs e)
        {
            if (!(sender is LayerViewModel oldLayer)) return;

            var layerIsShiftedOrReferencedInOverlay = m_OverlayService.Overlays.Any
                (o => o.ReferenceLayer != null && o.ReferenceLayer.Name.Equals(oldLayer.LayerModel.Name)
                || o.ShiftedLayer != null && o.ShiftedLayer.Name.Equals(oldLayer.LayerModel.Name));
            if (layerIsShiftedOrReferencedInOverlay)
            {
                MessageBox.Show("This layer is active in one or more overlays, "
                    + "removing the layer will cause these overlays to become invalid.", "Remove layer");
            }

            try
            {
                m_ToolModel.Stack.RemoveLayer(oldLayer.LayerModel);
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("An exception occured during removing the layer", "Could not remove layer");
            }
        }

        private void DeleteLayerViewModel(LayerViewModel oldLayer)
        {
            if (oldLayer == null) return;

            UnsubscribeLayer(oldLayer);
            Layers.Remove(oldLayer);
            ReversedLayers.Remove(oldLayer);
        }

        private void OnStackChanged()
        {
            FillLayers(m_ToolModel.Stack.Layers);
        }

        private void FillLayers(IList<ILayer> layers)
        {
            var index = 0;
            if (Layers == null)
            {
                Layers = new ObservableCollection<LayerViewModel>();
            }
            if (layers != null)
            {
                foreach (var currentLayer in layers)
                {
                    var layerViewModel = Layers.FirstOrDefault(layer => layer.LayerModel.Name == currentLayer.Name);
                    if (layerViewModel != null)
                    {
                        var oldIndex = Layers.IndexOf(layerViewModel);
                        if (oldIndex != index)
                        {
                            Layers.Move(oldIndex, index);
                            var oldReversedIndex = ReversedLayers.IndexOf(layerViewModel);
                            ReversedLayers.Move(oldReversedIndex, Layers.Count - index - 1);
                        }
                        layerViewModel.Init(currentLayer, m_ToolModel);
                    }
                    else
                    {
                        AddNewLayerViewModel(currentLayer, index);
                    }
                    index++;
                }
            }

            if (index < Layers.Count)
            {
                foreach (var currentLayer in Layers.ToList().GetRange(index, Layers.Count - index))
                {
                    DeleteLayerViewModel(currentLayer);
                }
            }
            if (m_ToolModel != null)
            {
                OnLayersChanged();
            }
            UpdateIsActiveAndIsValidForEachCell();
        }

        private void EnableOnlyThisOverlay(OverlayWithCirclesViewModel overlay)
        {
            foreach (var currentOverlay in Overlays)
            {
                currentOverlay.Overlay.IsInEditMode = currentOverlay.Equals(overlay);
                currentOverlay.UpdateLayersSelections();
            }
        }

        private int GetCurrentFillingOverlayIndex()
        {
            if (Overlays == null)
            {
                return -1;
            }

            var firstUnfilledOverlay = Overlays.FirstOrDefault(overlay => !overlay.IsFilled && !overlay.HasReversedLayers);
            if (firstUnfilledOverlay != null)
            {
                return Overlays.IndexOf(firstUnfilledOverlay);
            }
            return Overlays.Count == m_OverlayService.MaxOverlaysCount ? -1 : Overlays.Count;
        }

        private void OnSelectLayerPointRequested(object sender, EventArgs e)
        {
            if (!(sender is Cell cell) ||
                !m_ToolModel.Stack.Layers.Contains(cell.Layer) ||
                m_ToolModel.Stack.Layers.Count < 2)
            {
                return;
            }

            var selectingLayerModel = cell.Layer;
            var overlayIndex = GetCurrentFillingOverlayIndex();

            if (overlayIndex < 0) return;

            if (overlayIndex == Overlays.Count)
            {
                m_OverlayService.AddOverlay(selectingLayerModel);
            }
            else
            {
                var overlayWithCircle = Overlays[overlayIndex];
                if (overlayWithCircle?.Overlay == null || Equals(overlayWithCircle.SelectedLayers[0], selectingLayerModel)) return;
                overlayWithCircle.SelectLayer(selectingLayerModel);
            }
        }

        private void OnDeselectLayerPointRequested(object sender, LayerEventArgs e)
        {
            var overlayWithCircles = sender as OverlayWithCirclesViewModel;
            var deselectingLayerModel = e?.Layer;
            if (overlayWithCircles == null || deselectingLayerModel == null) return;

            var overlay = overlayWithCircles.Overlay;

            if (!Layers.Any(l => Equals(l.LayerModel, deselectingLayerModel))) return;

            if (!overlayWithCircles.SelectedLayers.Any())
            {
                if (m_OverlayService.CanRemoveOverlay(overlay))
                {
                    m_OverlayService.RemoveOverlay(overlay);
                }
            }
            else
            {
                EnableOnlyThisOverlay(overlayWithCircles);
                m_OverlayService.RemoveLayerFromOverlay(overlay, deselectingLayerModel);
            }
        }

        private void FillOverlayGraphical()
        {
            foreach (var overlayWithCircles in Overlays)
            {
                UnsubscribeOverlay(overlayWithCircles);
            }
            Overlays.Clear();

            foreach (var overlayName in OverlayNames)
            {
                overlayName.Name = null;
            }

            if (m_ToolModel?.Stack?.Layers == null) return;

            for (var i = 0; i < m_OverlayService.Overlays.Count; i++)
            {
                var overlay = m_OverlayService.Overlays[i];
                if (i < OverlayNames.Count)
                {
                    OverlayNames[i].Name = overlay.Name;
                }
                var overlayWithCircles = new OverlayWithCirclesViewModel(overlay, m_ToolModel.Stack.Layers);
                SubscribeOverlay(overlayWithCircles);
                Overlays.Add(overlayWithCircles);
            }
            UpdateIsActiveAndIsValidForEachCell();
        }

        private void DeactivateAllCells()
        {
            foreach (var layer in Layers)
            {
                foreach (var cell in layer.Cells)
                {
                    cell.IsActive = false;
                    cell.IsValid = true;
                }
            }
        }

        private void UpdateIsActiveAndIsValidForEachCell()
        {
            if (Layers == null || !Layers.Any()) return;

            if (Layers.Count == 1)
            {
                DeactivateAllCells();
                return;
            }

            var currentFillingOverlayIndex = GetCurrentFillingOverlayIndex();
            if (currentFillingOverlayIndex < 0)
            {
                DeactivateAllCells();
                return;
            }

            ILayer currentSelectedLayer = null;
            var overlayWithCirclesViewModel = Overlays.ElementAtOrDefault(currentFillingOverlayIndex);
            if (overlayWithCirclesViewModel != null)
            {
                EnableOnlyThisOverlay(overlayWithCirclesViewModel);
                if (currentFillingOverlayIndex < Overlays.Count - 1)
                {
                    overlayWithCirclesViewModel.Overlay.IsOverlayDefinitionCompleted = true;
                }

                currentSelectedLayer = overlayWithCirclesViewModel.Overlay.ShiftedLayer
                    ?? overlayWithCirclesViewModel.Overlay.ReferenceLayer;
            }
            var overlayCandidates = m_OverlayService.GetOverlayCandidates(currentSelectedLayer,
                overlayWithCirclesViewModel?.Overlay.Direction);

            foreach (var layer in Layers)
            {
                var layerIsValid = overlayCandidates.Any(oc => oc.ShiftedLayer.Name == layer.Name
                    || oc.ReferenceLayer.Name == layer.Name);

                for (var overlayIndex = 0; overlayIndex < layer.Cells.Count; overlayIndex++)
                {
                    var cell = layer.Cells[overlayIndex];

                    if (overlayIndex == currentFillingOverlayIndex)
                    {
                        var isSelected = (overlayWithCirclesViewModel?.IsInEditMode ?? false)
                            && Equals(overlayWithCirclesViewModel.SelectedLayers[0], layer.LayerModel);

                        cell.IsActive = !isSelected;
                        cell.IsValid = !isSelected && layerIsValid;
                    }
                    else
                    {
                        cell.IsActive = false;
                        cell.IsValid = true;
                    }
                }
            }
        }
    }
}
