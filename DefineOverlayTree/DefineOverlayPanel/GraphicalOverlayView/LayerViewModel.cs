using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;
using DefineOverlayTree.HelperClasses;
using DefineOverlayTree.Logic;

namespace DefineOverlayTree.DefineOverlayPanel.GraphicalOverlayView
{
    public class LayerViewModel : NotifyPropertyChanged
    {
        public ILayer LayerModel { get; private set; }

        private ITool m_ToolModel;
        private readonly OverlayService m_OverlayService;

        public ObservableCollection<Cell> Cells { get; }
        public event EventHandler CellSelected;

        public void SetOverlays()
        {
            ClearCells();
            if (m_OverlayService.Overlays != null)
            {
                foreach (var overlay in m_OverlayService.Overlays)
                {
                    Cells.Add(new Cell
                    {
                        Layer = LayerModel,
                        Overlay = overlay.OverlayModel
                    });
                }
            }

            SetOverlaysToMaxCount();
            OnPropertyChanged(nameof(Cells));
        }

        private void ClearCells()
        {
            // todo: cannot call Cells.Clear() because due to the way ObservableCollection is implemented doing Clear() raises CollectionChangedEvent but e.OldItems is null
            while (Cells?.Count > 0)
            {
                Cells.RemoveAt(0);
            }
        }

        private void OnCellSelected(object sender, EventArgs e)
        {
            CellSelected?.Invoke(sender, e);
        }

        private void SetOverlaysToMaxCount()
        {
            while (Cells.Count < m_OverlayService.MaxOverlaysCount)
            {
                Cells.Add(new Cell
                {
                    Layer = LayerModel,
                    Overlay = null
                });
            }
        }

        public ICommand DeleteCommand { get; }
        public event EventHandler DeleteRequested;

        private readonly Func<string, bool> m_IsValidLayerNameFunc;

        public bool CanRemoveLayer => m_ToolModel?.Stack != null && LayerModel != null;

        private void UpdateRemovingAbility()
        {
            OnPropertyChanged(nameof(CanRemoveLayer));
        }

        private void SubscribeToolModel(ITool toolModel)
        {
            if (toolModel?.Stack != null)
            {
                toolModel.Stack.StackChanged += UpdateRemovingAbility;
            }
        }

        private void UnsubscribeToolModel(ITool toolModel)
        {
            if (toolModel?.Stack != null)
            {
                toolModel.Stack.StackChanged -= UpdateRemovingAbility;
            }
        }

        public string Name
        {
            get => LayerModel?.Name;
            set
            {
                value = value?.Trim();
                if (LayerModel == null || value == LayerModel.Name ||
                    m_IsValidLayerNameFunc != null && !m_IsValidLayerNameFunc(value))
                {
                    return;
                }
                LayerModel.Name = value;
                OnPropertyChanged();
            }
        }

        private bool m_IsFirst;
        public bool IsFirst
        {
            get => m_IsFirst;
            set
            {
                if (value == m_IsFirst) return;
                m_IsFirst = value;
                OnPropertyChanged();
            }
        }

        public LayerViewModel(ILayer layerModel, ITool toolModel, OverlayService overlayService,
            Func<string, bool> isValidLayerNameFunc = null)
        {
            m_OverlayService = overlayService ?? throw new ArgumentNullException(nameof(OverlayService));

            Cells = new ObservableCollection<Cell>();
            Cells.CollectionChanged += OnCellsCollectionChanged;

            DeleteCommand = new DelegateCommand<object>(o => DeleteRequested?.Invoke(this, EventArgs.Empty));
            m_IsValidLayerNameFunc = isValidLayerNameFunc;
            Init(layerModel, toolModel);
        }

        public void Init(ILayer layerModel, ITool toolModel)
        {
            UnsubscribeToolModel(m_ToolModel);
            m_ToolModel = toolModel;
            SubscribeToolModel(m_ToolModel);
            LayerModel = layerModel;
            OnPropertyChanged(nameof(CanRemoveLayer));
            SetOverlays();
        }

        private void OnCellsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (Cell cells in e.OldItems)
                {
                    cells.PointToCellRequested -= OnCellSelected;
                }
            }

            if (e.NewItems != null)
            {
                foreach (Cell cells in e.NewItems)
                {
                    cells.PointToCellRequested += OnCellSelected;
                }
            }
        }

        public override string ToString() => Name;
    }
}
