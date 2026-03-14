using System;
using System.Collections.Generic;
using System.Linq;
using DefineOverlayTree.HelperClasses;
using DefineOverlayTree.Logic;

namespace DefineOverlayTree.DefineOverlayPanel
{
    public class OverlayService
    {
        private const string TopLayerEmptyValidationMessage = "Top layer can't be empty";
        private const string BottomLayerEmptyValidationMessage = "Bottom layer can't be empty";
        private const string TopLayerLowerValidationMessage
            = "Top layer should always be higher in the hierarchy than the bottom layer";
        private const string TopLayerAlreadyExistValidationMessage = "Only one X or Y overlay can be present from a top layer";

        public event EventHandler OverlaysChanged;

        private IOverlayMap m_OverlayMap;
        private IStack m_Stack;
        private IOverlayCandidatesService m_OverlayCandidatesService;
        private readonly ObservableCollectionEx<Overlay> m_OverlayList;

        public OverlayService(IStack stack, IOverlayMap overlayMap, IOverlayCandidatesService overlayCandidatesService)
        {
            m_OverlayList = new ObservableCollectionEx<Overlay>();
            m_OverlayList.ItemPropertyChanged += OnOverlaysItemPropertyChanged;

            SetSubject(stack, overlayMap, overlayCandidatesService);
        }

        public IReadOnlyList<Overlay> Overlays => m_OverlayList;

        public string NewOverlayDefaultName => NamingService.OverlayDefaultNameBuilder.GetNextDefaultName
            (Overlays?.Select(overlay => overlay.Name).ToList());

        public int MaxOverlaysCount => m_OverlayMap?.MaxNrOfOverlays ?? 0;

        public void SetSubject(IStack stack, IOverlayMap overlayMap, IOverlayCandidatesService overlayCandidatesService)
        {
            Unsubscribe();

            m_Stack = stack;
            m_OverlayMap = overlayMap;
            m_OverlayCandidatesService = overlayCandidatesService;
            LoadOverlayMap();

            Subscribe();
            OverlaysChanged?.Invoke(this, EventArgs.Empty);
        }

        private void LoadOverlayMap()
        {
            m_OverlayList.Clear();

            if (m_OverlayMap?.Overlays != null)
            {
                foreach (var overlayModel in m_OverlayMap.Overlays)
                {
                    m_OverlayList.Add(new Overlay(overlayModel));
                }
            }
        }

        public bool AreOverlaysValid()
        {
            if (m_OverlayMap?.Overlays == null || m_OverlayMap.Overlays.Count == 0
                || m_OverlayMap.Overlays.Count != m_OverlayList.Count) return false;

            return !m_OverlayList.Where((t, i) => !Equals(m_OverlayMap.Overlays[i], t.OverlayModel)).Any();
        }

        public bool CanAddOverlay()
        {
            return m_OverlayMap != null && m_Stack != null && m_Stack.Layers.Count > 1 && m_OverlayMap.CanAddOverlay()
                && m_OverlayList.Count < MaxOverlaysCount;
        }

        public void AddOverlay()
        {
            if (!CanAddOverlay()) return;

            var overlayDefinition = m_OverlayCandidatesService?.GetOverlayCandidates().FirstOrDefault();
            if (overlayDefinition == null)
            {
                var newOverlay = new Overlay { Name = NewOverlayDefaultName };

                newOverlay.ShiftedLayer = GetShiftedLayers(newOverlay).FirstOrDefault()?.Item1;
                newOverlay.ReferenceLayer = GetReferenceLayers(newOverlay).FirstOrDefault()?.Item1;
                newOverlay.Direction = GetDirection(newOverlay).FirstOrDefault()?.Item1;

                m_OverlayList.Add(newOverlay);
                OverlaysChanged?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                m_OverlayMap?.AddOverlay(NewOverlayDefaultName, overlayDefinition);
                if (SyncOverlaysFromOverlayMap())
                {
                    OverlaysChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public void AddOverlay(ILayer layer)
        {
            var layerIndex = m_Stack?.Layers.ToList().IndexOf(layer);

            var overlay = new Overlay { Name = NewOverlayDefaultName };

            if (layerIndex == 0)
            {
                overlay.ReferenceLayer = layer;
            }
            else
            {
                var overlayCandidates = m_OverlayCandidatesService?.GetOverlayCandidates().
                    Where(oc => Equals(oc.ReferenceLayer, layer) || Equals(oc.ShiftedLayer, layer)).ToList();
                if (overlayCandidates?.Any(oc => Equals(oc.ShiftedLayer, layer)) ?? false)
                {
                    overlay.ShiftedLayer = layer;
                }
                else if (overlayCandidates?.Any(oc => Equals(oc.ReferenceLayer, layer)) ?? false)
                {
                    overlay.ReferenceLayer = layer;
                }
                else
                {
                    overlay.ShiftedLayer = layer;
                }
            }

            m_OverlayList.Add(overlay);
            OverlaysChanged?.Invoke(this, EventArgs.Empty);
        }

        public void RemoveLayerFromOverlay(Overlay overlay, ILayer layer)
        {
            if (Equals(overlay.ShiftedLayer, layer))
            {
                if (overlay.ReferenceLayer != null && m_Stack?.Layers.ToList().IndexOf(overlay.ReferenceLayer) == 0)
                {
                    overlay.ShiftedLayer = null;
                }
                else
                {
                    overlay.ShiftedLayer = overlay.ReferenceLayer;
                    overlay.ReferenceLayer = null;
                }
            }
            else if (Equals(overlay.ReferenceLayer, layer))
            {
                overlay.ReferenceLayer = null;
            }

            if (SyncOverlaysFromOverlayMap())
            {
                OverlaysChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public bool CanRemoveOverlay(Overlay overlay)
        {
            return m_OverlayList.Contains(overlay);
        }

        public void RemoveOverlay(Overlay overlay)
        {
            if (overlay.OverlayModel != null && m_OverlayMap.CanRemoveOverlay(overlay.OverlayModel))
            {
                m_OverlayMap?.RemoveOverlay(overlay.OverlayModel);
            }
            m_OverlayList.Remove(overlay);
            SyncOverlaysToOverlayMap();
            SyncOverlaysFromOverlayMap();
            OverlaysChanged?.Invoke(this, EventArgs.Empty);
        }

        private bool SyncOverlaysFromOverlayMap()
        {
            if (m_OverlayMap == null) return false;

            var overlaysChanged = RemoveDeletedOverlays();
            overlaysChanged = overlaysChanged || AddInsertedOverlays();

            return overlaysChanged;
        }

        private bool RemoveDeletedOverlays()
        {
            var overlaysChanged = false;
            for (var i = m_OverlayList.Count - 1; i >= 0; i--)
            {
                var overlay = m_OverlayList[i];
                if (overlay.OverlayModel != null && !m_OverlayMap.Overlays.Contains(overlay.OverlayModel))
                {
                    m_OverlayList.RemoveAt(i);
                    overlaysChanged = true;
                }
            }
            return overlaysChanged;
        }

        private bool AddInsertedOverlays()
        {
            var overlaysChanged = false;
            foreach (var overlay in m_OverlayMap.Overlays)
            {
                if (!m_OverlayList.Any(o => Equals(o.OverlayModel, overlay)))
                {
                    m_OverlayList.Add(new Overlay(overlay));
                    overlaysChanged = true;
                }
            }
            return overlaysChanged;
        }

        public List<IOverlayDefinition> GetOverlayCandidates(ILayer layer, Direction? direction)
        {
            var overlayCandidates = m_OverlayCandidatesService?.GetOverlayCandidates().Where(oc => (layer == null ||
                Equals(oc.ShiftedLayer, layer) ||
                Equals(oc.ReferenceLayer, layer)) &&
                (direction == null || oc.Direction == direction)).ToList();
            foreach (var overlay in m_OverlayList)
            {
                if (overlay.OverlayModel == null)
                {
                    overlayCandidates?.RemoveAll(oc => Equals(oc.ShiftedLayer, overlay.ShiftedLayer) &&
                        Equals(oc.ReferenceLayer, overlay.ReferenceLayer) &&
                        oc.Direction == overlay.Direction);
                }
            }

            return overlayCandidates;
        }

        public List<Tuple<ILayer, bool>> GetShiftedLayers(Overlay overlay)
        {
            var shiftedLayers = new List<Tuple<ILayer, bool>>();

            if (m_Stack?.Layers == null) return shiftedLayers;

            var overlayCandidates = m_OverlayCandidatesService?.GetOverlayCandidates(overlay.OverlayModel?.OverlayDefinition)
                .Where(oc => (overlay.ReferenceLayer == null || Equals(oc.ReferenceLayer, overlay.ReferenceLayer)) &&
                (overlay.Direction == null || oc.Direction == overlay.Direction)).ToList();

            foreach (var layer in m_Stack.Layers.Skip(1))
            {
                var isOverlayCandidate = overlayCandidates?.Any(oc => Equals(oc.ShiftedLayer, layer)) ?? false;
                shiftedLayers.Add(new Tuple<ILayer, bool>(layer, isOverlayCandidate));
            }

            return shiftedLayers;
        }

        public List<Tuple<ILayer, bool>> GetReferenceLayers(Overlay overlay)
        {
            var referenceLayers = new List<Tuple<ILayer, bool>>();

            if (m_Stack?.Layers == null) return referenceLayers;

            var overlayCandidates = m_OverlayCandidatesService?.GetOverlayCandidates(overlay.OverlayModel?.OverlayDefinition)
                .Where(oc => (overlay.ShiftedLayer == null || Equals(oc.ShiftedLayer, overlay.ShiftedLayer)) &&
                (overlay.Direction == null || oc.Direction == overlay.Direction)).ToList();

            foreach (var layer in m_Stack.Layers.Take(m_Stack.Layers.Count - 1))
            {
                var isOverlayCandidate = overlayCandidates?.Any(oc => Equals(oc.ReferenceLayer, layer)) ?? false;
                referenceLayers.Add(new Tuple<ILayer, bool>(layer, isOverlayCandidate));
            }

            return referenceLayers;
        }

        public List<Tuple<Direction, bool>> GetDirection(Overlay overlay)
        {
            var directions = new List<Tuple<Direction, bool>>();

            if (m_Stack?.Layers == null) return directions;

            var overlayCandidates = m_OverlayCandidatesService?.GetOverlayCandidates(overlay.OverlayModel?.OverlayDefinition)
                .Where(oc => (overlay.ShiftedLayer == null || Equals(oc.ShiftedLayer, overlay.ShiftedLayer)) &&
                (overlay.ReferenceLayer == null || Equals(oc.ReferenceLayer, overlay.ReferenceLayer))).ToList();

            foreach (var direction in (Direction[])Enum.GetValues(typeof(Direction)))
            {
                var isOverlayCandidate = overlayCandidates?.Any(oc => oc.Direction == direction) ?? false;
                directions.Add(new Tuple<Direction, bool>(direction, isOverlayCandidate));
            }

            return directions;
        }

        public bool IsValidOverlayDefinition(Overlay overlay, out string validationMessage)
        {
            var shiftedLayerIndex = m_Stack.Layers.ToList().IndexOf(overlay.ShiftedLayer);
            var referenceLayerIndex = m_Stack.Layers.ToList().IndexOf(overlay.ReferenceLayer);

            if (!overlay.IsOverlayDefinitionCompleted)
            {
                validationMessage = string.Empty;
            }
            else if (overlay.ShiftedLayer == null)
            {
                validationMessage = TopLayerEmptyValidationMessage;
            }
            else if (overlay.ReferenceLayer == null)
            {
                validationMessage = BottomLayerEmptyValidationMessage;
            }
            else if (shiftedLayerIndex <= referenceLayerIndex)
            {
                validationMessage = TopLayerLowerValidationMessage;
            }
            else if (!GetShiftedLayers(overlay).Find(l => Equals(l.Item1, overlay.ShiftedLayer)).Item2)
            {
                validationMessage = TopLayerAlreadyExistValidationMessage;
            }
            else
            {
                validationMessage = string.Empty;
            }

            return string.IsNullOrEmpty(validationMessage);
        }

        public bool IsValidOverlayName(string name)
        {
            return m_OverlayMap?.IsValidOverlayName(name) ?? false;
        }

        private IOverlayDefinition GetOverlayDefinition(Overlay overlay)
        {
            return m_OverlayCandidatesService?.GetOverlayCandidates()?.FirstOrDefault(oc => Equals(oc.ReferenceLayer,
                overlay.ReferenceLayer) &&
                Equals(oc.ShiftedLayer, overlay.ShiftedLayer) &&
                oc.Direction == overlay.Direction);
        }

        private void Subscribe()
        {
            if (m_Stack != null)
            {
                m_Stack.StackChanged += OnStackChanged;
            }
        }

        private void Unsubscribe()
        {
            if (m_Stack != null)
            {
                m_Stack.StackChanged -= OnStackChanged;
            }
        }

        private void OnStackChanged()
        {
            if (m_Stack?.Layers == null) return;

            for (var i = m_OverlayList.Count - 1; i >= 0; i--)
            {
                var overlay = m_OverlayList[i];

                if (overlay.ShiftedLayer != null && m_Stack.Layers.Skip(1).All(l => l.Name != overlay.ShiftedLayer.Name))
                {
                    overlay.OverlayModel = null;
                    overlay.ShiftedLayer = null;
                }

                if (overlay.ReferenceLayer != null && m_Stack.Layers.Take(m_Stack.Layers.Count - 1).All(l => l.Name != overlay.ReferenceLayer.Name))
                {
                    overlay.OverlayModel = null;
                    overlay.ShiftedLayer = m_Stack.Layers.Count > 1 && Equals(overlay.ReferenceLayer, m_Stack.Layers.Last()) ? overlay.ReferenceLayer : overlay.ShiftedLayer;
                    overlay.ReferenceLayer = null;
                }

                if (overlay.ShiftedLayer == null && overlay.ReferenceLayer == null)
                {
                    m_OverlayList.RemoveAt(i);
                }
            }
            OverlaysChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnOverlaysItemPropertyChanged(object sender, IPropertyChangedEventExArgs<Overlay> e)
        {
            if (e.PropertyName == nameof(Overlay.OverlayModel) ||
                e.PropertyName == nameof(Overlay.IsInEditMode) ||
                e.PropertyName == nameof(Overlay.IsOverlayDefinitionCompleted) ||
                m_OverlayMap == null ||
                m_OverlayCandidatesService == null)
            {
                return;
            }

            if (e.PropertyName == nameof(Overlay.ShiftedLayer) ||
                e.PropertyName == nameof(Overlay.ReferenceLayer) ||
                e.PropertyName == nameof(Overlay.Direction))
            {
                var uiOverlay = e.Item;
                if (uiOverlay.ShiftedLayer != null && uiOverlay.ReferenceLayer != null && uiOverlay.Direction == null)
                {
                    var getOverlayCandidates = m_OverlayCandidatesService?.
                        GetOverlayCandidates().
                        Where(oc => Equals(oc.ShiftedLayer, uiOverlay.ShiftedLayer) && Equals(oc.ReferenceLayer, uiOverlay.ReferenceLayer)).ToList();
                    uiOverlay.Direction = getOverlayCandidates.FirstOrDefault()?.Direction ?? Direction.X;
                    return;
                }
                if (uiOverlay.OverlayModel != null)
                {
                    var overlayDefinition = GetOverlayDefinition(uiOverlay);
                    if (overlayDefinition == null)
                    {
                        if (m_OverlayMap.CanRemoveOverlay(uiOverlay.OverlayModel))
                        {
                            m_OverlayMap.RemoveOverlay(uiOverlay.OverlayModel);
                            uiOverlay.OverlayModel = null;
                        }
                    }
                    else
                    {
                        uiOverlay.OverlayModel.OverlayDefinition = overlayDefinition;
                    }
                }

                SyncOverlaysToOverlayMap();
            }
            OverlaysChanged?.Invoke(this, EventArgs.Empty);
        }

        private void SyncOverlaysToOverlayMap()
        {
            if (m_OverlayMap == null) return;

            var index = 0;
            foreach (var uiOverlay in m_OverlayList)
            {
                var overlayDefinition = GetOverlayDefinition(uiOverlay);
                if (overlayDefinition != null)
                {
                    if (uiOverlay.OverlayModel == null)
                    {
                        if (m_OverlayMap.CanAddOverlay())
                        {
                            var overlay = m_OverlayMap.InsertOverlay(index, uiOverlay.Name, overlayDefinition);
                            uiOverlay.OverlayModel = overlay;
                        }
                    }
                    else
                    {
                        uiOverlay.OverlayModel.OverlayDefinition = overlayDefinition;
                    }
                }
                if (uiOverlay.OverlayModel != null)
                {
                    index++;
                }
            }
        }
    }
}
