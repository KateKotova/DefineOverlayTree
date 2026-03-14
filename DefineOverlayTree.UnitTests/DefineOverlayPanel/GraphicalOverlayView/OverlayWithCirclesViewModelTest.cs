using System.Collections.Generic;
using System.Linq;
using DefineOverlayTree.DefineOverlayPanel;
using DefineOverlayTree.DefineOverlayPanel.GraphicalOverlayView;
using DefineOverlayTree.Logic;
using Moq;
using NUnit.Framework;

namespace DefineOverlayTree.UnitTests.DefineOverlayPanel.GraphicalOverlayView
{
    [TestFixture]
    public class OverlayWithCirclesViewModelTest
    {
        private OverlayWithCirclesViewModel m_OverlayWithCirclesViewModel;
        private List<ILayer> m_Layers;
        private const string OverlayName = "Overlay 1";
        private IOverlay m_OverlayModel;
        private Overlay m_Overlay;

        [SetUp]
        public void Setup()
        {
            const int numberOfLayers = 3;
            var stackWrapper = GetMockStackWithGivenNumberOfLayers(numberOfLayers);
            var stack = stackWrapper.Object;
            m_Layers = stack.Layers.ToList();

            var overlayWrapper = new Mock<IOverlay>();
            overlayWrapper.SetupProperty(x => x.Name);
            overlayWrapper.SetupProperty(x => x.OverlayDefinition);
            m_OverlayModel = overlayWrapper.Object;
            m_OverlayModel.Name = OverlayName;
            m_OverlayModel.OverlayDefinition = GetMockOverlayDefinition(m_Layers[0], m_Layers[1], Direction.X);

            m_Overlay = new Overlay { OverlayModel = m_OverlayModel };
        }

        private static Mock<IStack> GetMockStackWithGivenNumberOfLayers(int nrOfLayers)
        {
            const string layerNameBase = "Layer ";
            var stackWrapper = new Mock<IStack>();
            var layers = new List<ILayer>();
            for (var i = 1; i <= nrOfLayers; ++i)
            {
                var layerWrapper = new Mock<ILayer>();
                layerWrapper.SetupProperty(o => o.Name);
                layerWrapper.Object.Name = layerNameBase + i;
                layers.Add(layerWrapper.Object);
            }
            stackWrapper.Setup(s => s.Layers).Returns(layers);
            return stackWrapper;
        }

        private static IOverlayDefinition GetMockOverlayDefinition(ILayer shiftedLayer, ILayer referenceLayer,
            Direction direction)
        {
            var overlayDefinitionWrapper = new Mock<IOverlayDefinition>();
            overlayDefinitionWrapper.Setup(o => o.ShiftedLayer).Returns(shiftedLayer);
            overlayDefinitionWrapper.Setup(o => o.ReferenceLayer).Returns(referenceLayer);
            overlayDefinitionWrapper.Setup(o => o.Direction).Returns(direction);
            return overlayDefinitionWrapper.Object;
        }

        [Test]
        public void InitializeWithOverlayTest()
        {
            // Arrange.
            m_OverlayWithCirclesViewModel = new OverlayWithCirclesViewModel(m_Overlay, m_Layers);
            // Assert.
            Assert.IsNotNull(m_OverlayWithCirclesViewModel);
            Assert.IsNotNull(m_OverlayWithCirclesViewModel.Overlay);
            Assert.AreEqual(m_OverlayWithCirclesViewModel.Name, m_Overlay.Name);
            Assert.AreEqual(m_OverlayWithCirclesViewModel.Overlay.Direction,
                m_OverlayModel.OverlayDefinition.Direction);
            Assert.AreEqual(m_OverlayWithCirclesViewModel.Overlay.ReferenceLayer.Name,
                m_OverlayModel.OverlayDefinition.ReferenceLayer.Name);
            Assert.AreEqual(m_OverlayWithCirclesViewModel.Overlay.ShiftedLayer.Name,
                m_OverlayModel.OverlayDefinition.ShiftedLayer.Name);
            Assert.IsNotNull(m_OverlayWithCirclesViewModel.Direction);
            Assert.AreEqual(m_OverlayWithCirclesViewModel.Direction.Value, m_OverlayModel.OverlayDefinition.Direction);
            Assert.IsNotNull(m_OverlayWithCirclesViewModel.Name);
            Assert.AreEqual(m_OverlayWithCirclesViewModel.Name, m_OverlayModel.Name);
            Assert.IsNotNull(m_OverlayWithCirclesViewModel.IsEnabled);
            Assert.IsTrue(m_OverlayWithCirclesViewModel.IsEnabled);
            Assert.IsNotNull(m_OverlayWithCirclesViewModel.Layers);
            Assert.AreEqual(m_OverlayWithCirclesViewModel.Layers.Count, m_Layers.Count);
            Assert.IsNotNull(m_OverlayWithCirclesViewModel.SelectedLayers);
            Assert.AreEqual(m_OverlayWithCirclesViewModel.SelectedLayers.Count, 2);
            Assert.IsTrue(m_OverlayWithCirclesViewModel.SelectedLayers.Contains(m_Layers[0]));
            Assert.IsTrue(m_OverlayWithCirclesViewModel.SelectedLayers.Contains(m_Layers[1]));
            Assert.IsNotNull(m_OverlayWithCirclesViewModel.IsFilled);
            Assert.IsTrue(m_OverlayWithCirclesViewModel.IsFilled);
            Assert.IsFalse(m_OverlayWithCirclesViewModel.IsInEditMode);
            Assert.IsNotNull(m_OverlayWithCirclesViewModel.HasNoSelectedLayers);
            Assert.IsFalse(m_OverlayWithCirclesViewModel.HasNoSelectedLayers);
            Assert.IsNotNull(m_OverlayWithCirclesViewModel.TopLayer);
            Assert.AreEqual(m_OverlayWithCirclesViewModel.TopLayer.Name, m_Layers[1].Name);
            Assert.IsNotNull(m_OverlayWithCirclesViewModel.BottomLayer);
            Assert.AreEqual(m_OverlayWithCirclesViewModel.BottomLayer.Name, m_Layers[0].Name);
            Assert.IsNotNull(m_OverlayWithCirclesViewModel.CircleMouseDownCommand);
            Assert.IsNotNull(m_OverlayWithCirclesViewModel.Circle0IsVisible);
            Assert.IsTrue(m_OverlayWithCirclesViewModel.Circle0IsVisible);
            Assert.IsNotNull(m_OverlayWithCirclesViewModel.Circle1IsVisible);
            Assert.IsTrue(m_OverlayWithCirclesViewModel.Circle1IsVisible);
            Assert.IsNotNull(m_OverlayWithCirclesViewModel.Circle0Y);
            Assert.AreEqual(m_OverlayWithCirclesViewModel.Circle0Y,
                m_OverlayWithCirclesViewModel.SelectedLayers[0].Name == m_Layers[0].Name
                    ? 2 * GraphicalOverlaySizes.OuterLayerRowWithMarginHeight
                      + GraphicalOverlaySizes.CircleYRelativeToCell
                    : GraphicalOverlaySizes.OuterLayerRowWithMarginHeight
                      + GraphicalOverlaySizes.CircleYRelativeToCell);
            Assert.IsNotNull(m_OverlayWithCirclesViewModel.Circle1Y);
            Assert.AreEqual(m_OverlayWithCirclesViewModel.Circle1Y,
                m_OverlayWithCirclesViewModel.SelectedLayers[1].Name == m_Layers[0].Name
                    ? 2 * GraphicalOverlaySizes.OuterLayerRowWithMarginHeight
                      + GraphicalOverlaySizes.CircleYRelativeToCell
                    : GraphicalOverlaySizes.OuterLayerRowWithMarginHeight
                      + GraphicalOverlaySizes.CircleYRelativeToCell);
            Assert.IsNotNull(m_OverlayWithCirclesViewModel.ArrowPointY);
            Assert.AreEqual(m_OverlayWithCirclesViewModel.ArrowPointY,
                2 * GraphicalOverlaySizes.OuterLayerRowWithMarginHeight
                + GraphicalOverlaySizes.ArrowPointYRelativeToCell);
            Assert.IsNotNull(m_OverlayWithCirclesViewModel.ArrowLineStartY);
            Assert.AreEqual(m_OverlayWithCirclesViewModel.ArrowLineStartY,
                GraphicalOverlaySizes.OuterLayerRowWithMarginHeight
                + GraphicalOverlaySizes.ArrowLineStartYRelativeToCell);
            Assert.IsNotNull(m_OverlayWithCirclesViewModel.ArrowLineLength);
            Assert.AreEqual(m_OverlayWithCirclesViewModel.ArrowLineLength,
                GraphicalOverlaySizes.OuterLayerRowWithMarginHeight
                + GraphicalOverlaySizes.ArrowLineEndYRelativeToCell
                - GraphicalOverlaySizes.ArrowLineStartYRelativeToCell);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void IsEnabledTest(bool isEnabled)
        {
            m_OverlayWithCirclesViewModel = new OverlayWithCirclesViewModel(m_Overlay, m_Layers);
            m_OverlayWithCirclesViewModel.IsEnabled = isEnabled;
            Assert.AreEqual(m_OverlayWithCirclesViewModel.IsEnabled, isEnabled);
        }

        [Test]
        [TestCase(null)]
        [TestCase(new int[] { })]
        [TestCase(new[] { 0 })]
        [TestCase(new[] { 1 })]
        [TestCase(new[] { 2 })]
        [TestCase(new[] { 0, 0 })]
        [TestCase(new[] { 0, 1 })]
        [TestCase(new[] { 0, 2 })]
        [TestCase(new[] { 1, 0 })]
        [TestCase(new[] { 1, 1 })]
        [TestCase(new[] { 1, 2 })]
        [TestCase(new[] { 2, 0 })]
        [TestCase(new[] { 2, 1 })]
        [TestCase(new[] { 2, 2 })]
        [TestCase(new[] { 0, 0, 0 })]
        [TestCase(new[] { 0, 0, 1 })]
        [TestCase(new[] { 0, 0, 2 })]
        [TestCase(new[] { 0, 1, 0 })]
        [TestCase(new[] { 0, 1, 1 })]
        [TestCase(new[] { 0, 1, 2 })]
        [TestCase(new[] { 0, 2, 0 })]
        [TestCase(new[] { 0, 2, 1 })]
        [TestCase(new[] { 0, 2, 2 })]
        [TestCase(new[] { 1, 0, 0 })]
        [TestCase(new[] { 1, 0, 1 })]
        [TestCase(new[] { 1, 0, 2 })]
        [TestCase(new[] { 1, 1, 0 })]
        [TestCase(new[] { 1, 1, 1 })]
        [TestCase(new[] { 1, 1, 2 })]
        [TestCase(new[] { 1, 2, 0 })]
        [TestCase(new[] { 1, 2, 1 })]
        [TestCase(new[] { 1, 2, 2 })]
        [TestCase(new[] { 2, 0, 0 })]
        [TestCase(new[] { 2, 0, 1 })]
        [TestCase(new[] { 2, 0, 2 })]
        [TestCase(new[] { 2, 1, 0 })]
        [TestCase(new[] { 2, 1, 1 })]
        [TestCase(new[] { 2, 1, 2 })]
        [TestCase(new[] { 2, 2, 0 })]
        [TestCase(new[] { 2, 2, 1 })]
        [TestCase(new[] { 2, 2, 2 })]
        public void SelectedLayersTest(int[] selectedLayersIndices)
        {
            // Arrange.
            var uniqueSelectedLayersIndices = selectedLayersIndices?.Distinct().ToList();
            if (uniqueSelectedLayersIndices?.Count > 2)
            {
                uniqueSelectedLayersIndices = uniqueSelectedLayersIndices.Take(2).ToList();
            }
            var bottomLayerIndex = selectedLayersIndices != null && selectedLayersIndices.Any()
                ? uniqueSelectedLayersIndices.Min()
                : 0;
            var topLayerIndex = selectedLayersIndices != null && selectedLayersIndices.Any()
                ? uniqueSelectedLayersIndices.Max()
                : 0;
            m_OverlayWithCirclesViewModel = new OverlayWithCirclesViewModel(m_Overlay, m_Layers);
            var selectedLayers = new List<ILayer>();
            if (uniqueSelectedLayersIndices != null && uniqueSelectedLayersIndices.Any())
            {
                selectedLayers.AddRange(uniqueSelectedLayersIndices.Select(t => m_Layers[t]));
            }
            // Act.
            m_OverlayWithCirclesViewModel.SelectedLayers = selectedLayersIndices == null ? null : selectedLayers;
            // Assert.
            if (selectedLayersIndices == null)
            {
                Assert.IsNotNull(m_OverlayWithCirclesViewModel.SelectedLayers);
                Assert.AreEqual(m_OverlayWithCirclesViewModel.SelectedLayers.Count, 2);
                Assert.IsTrue(m_OverlayWithCirclesViewModel.SelectedLayers.Contains(m_Layers[0]));
                Assert.IsTrue(m_OverlayWithCirclesViewModel.SelectedLayers.Contains(m_Layers[1]));
                Assert.IsTrue(m_OverlayWithCirclesViewModel.IsFilled);
                Assert.IsFalse(m_OverlayWithCirclesViewModel.HasNoSelectedLayers);
                Assert.IsNotNull(m_OverlayWithCirclesViewModel.TopLayer);
                Assert.AreEqual(m_OverlayWithCirclesViewModel.TopLayer.Name, m_Layers[1].Name);
                Assert.IsNotNull(m_OverlayWithCirclesViewModel.BottomLayer);
                Assert.AreEqual(m_OverlayWithCirclesViewModel.BottomLayer.Name, m_Layers[0].Name);
                return;
            }

            Assert.IsNotNull(m_OverlayWithCirclesViewModel.SelectedLayers);
            Assert.AreEqual(m_OverlayWithCirclesViewModel.SelectedLayers.Count, selectedLayers.Count);
            foreach (var selectedLayer in selectedLayers)
            {
                Assert.IsTrue(m_OverlayWithCirclesViewModel.SelectedLayers.Contains(selectedLayer));
            }
            Assert.AreEqual(m_OverlayWithCirclesViewModel.IsFilled, selectedLayers.Count == 2);
            Assert.AreEqual(m_OverlayWithCirclesViewModel.HasNoSelectedLayers, selectedLayers.Count == 0);
            if (selectedLayers.Count < 2)
            {
                Assert.IsNull(m_OverlayWithCirclesViewModel.TopLayer);
                Assert.IsNull(m_OverlayWithCirclesViewModel.BottomLayer);
            }
            else
            {
                Assert.IsNotNull(m_OverlayWithCirclesViewModel.TopLayer);
                Assert.AreEqual(m_OverlayWithCirclesViewModel.TopLayer.Name, m_Layers[topLayerIndex].Name);
                Assert.IsNotNull(m_OverlayWithCirclesViewModel.BottomLayer);
                Assert.AreEqual(m_OverlayWithCirclesViewModel.BottomLayer.Name, m_Layers[bottomLayerIndex].Name);
            }
        }

        [Test]
        public void SelectTheSameLayerTest()
        {
            // Arrange.
            m_OverlayWithCirclesViewModel = new OverlayWithCirclesViewModel(m_Overlay, m_Layers);

            m_OverlayWithCirclesViewModel.CircleMouseDownExecute(1);
            // Act.
            m_OverlayWithCirclesViewModel.SelectLayer(m_Layers[0]);
            // Assert.
            Assert.IsNotNull(m_OverlayWithCirclesViewModel.SelectedLayers);
            Assert.AreEqual(m_OverlayWithCirclesViewModel.SelectedLayers.Count, 1);
            Assert.IsTrue(m_OverlayWithCirclesViewModel.SelectedLayers.Contains(m_Layers[0]));
            Assert.IsFalse(m_OverlayWithCirclesViewModel.IsFilled);
            Assert.IsFalse(m_OverlayWithCirclesViewModel.HasNoSelectedLayers);
            Assert.IsNull(m_OverlayWithCirclesViewModel.TopLayer);
            Assert.IsNull(m_OverlayWithCirclesViewModel.BottomLayer);
        }

        [Test]
        public void SelectSecondLayerTest()
        {
            // Arrange.
            m_OverlayWithCirclesViewModel = new OverlayWithCirclesViewModel(m_Overlay, m_Layers);
            // Act.
            m_OverlayWithCirclesViewModel.SelectLayer(m_Layers[0]);
            m_OverlayWithCirclesViewModel.SelectLayer(m_Layers[1]);
            // Assert.
            Assert.IsNotNull(m_OverlayWithCirclesViewModel.Layers);
            Assert.AreEqual(m_OverlayWithCirclesViewModel.Layers.Count, m_Layers.Count);
            Assert.IsNotNull(m_OverlayWithCirclesViewModel.SelectedLayers);
            Assert.AreEqual(m_OverlayWithCirclesViewModel.SelectedLayers.Count, 2);
            Assert.IsTrue(m_OverlayWithCirclesViewModel.SelectedLayers.Contains(m_Layers[0]));
            Assert.IsTrue(m_OverlayWithCirclesViewModel.SelectedLayers.Contains(m_Layers[1]));
            Assert.IsTrue(m_OverlayWithCirclesViewModel.IsFilled);
            Assert.IsFalse(m_OverlayWithCirclesViewModel.HasNoSelectedLayers);
            Assert.IsNotNull(m_OverlayWithCirclesViewModel.TopLayer);
            Assert.AreEqual(m_OverlayWithCirclesViewModel.TopLayer.Name, m_Layers[1].Name);
            Assert.IsNotNull(m_OverlayWithCirclesViewModel.BottomLayer);
            Assert.AreEqual(m_OverlayWithCirclesViewModel.BottomLayer.Name, m_Layers[0].Name);
        }

        [Test]
        public void CanNotSelectTheThirdLayerTest()
        {
            // Arrange.
            m_OverlayWithCirclesViewModel = new OverlayWithCirclesViewModel(m_Overlay, m_Layers);
            // Act.
            m_OverlayWithCirclesViewModel.SelectLayer(m_Layers[0]);
            m_OverlayWithCirclesViewModel.SelectLayer(m_Layers[1]);
            m_OverlayWithCirclesViewModel.SelectLayer(m_Layers[2]);
            // Assert.
            Assert.IsNotNull(m_OverlayWithCirclesViewModel.SelectedLayers);
            Assert.AreEqual(m_OverlayWithCirclesViewModel.SelectedLayers.Count, 2);
            Assert.IsTrue(m_OverlayWithCirclesViewModel.SelectedLayers.Contains(m_Layers[0]));
            Assert.IsTrue(m_OverlayWithCirclesViewModel.SelectedLayers.Contains(m_Layers[1]));
            Assert.IsFalse(m_OverlayWithCirclesViewModel.SelectedLayers.Contains(m_Layers[2]));
            Assert.IsTrue(m_OverlayWithCirclesViewModel.IsFilled);
            Assert.IsFalse(m_OverlayWithCirclesViewModel.HasNoSelectedLayers);
            Assert.IsNotNull(m_OverlayWithCirclesViewModel.TopLayer);
            Assert.AreEqual(m_OverlayWithCirclesViewModel.TopLayer.Name, m_Layers[1].Name);
            Assert.IsNotNull(m_OverlayWithCirclesViewModel.BottomLayer);
            Assert.AreEqual(m_OverlayWithCirclesViewModel.BottomLayer.Name, m_Layers[0].Name);
        }

        [Test]
        public void LayerIsSelectedTest()
        {
            // Act.
            m_OverlayWithCirclesViewModel = new OverlayWithCirclesViewModel(m_Overlay, m_Layers);
            // Assert.
            Assert.IsTrue(m_OverlayWithCirclesViewModel.LayerIsSelected(m_Layers[0]));
            Assert.IsTrue(m_OverlayWithCirclesViewModel.LayerIsSelected(m_Layers[1]));
            Assert.IsFalse(m_OverlayWithCirclesViewModel.LayerIsSelected(m_Layers[2]));
        }

        [TestCase(null, 0)]
        [TestCase(0, 0)]
        [TestCase(1, 0)]
        [TestCase(1, 1)]
        [TestCase(2, 0)]
        [TestCase(2, 1)]
        [TestCase(2, 2)]
        public void RemoveLayerTest(int? selectedLayersCount, int remoingLayerIndex)
        {
            // Arrange.
            m_OverlayWithCirclesViewModel = new OverlayWithCirclesViewModel(m_Overlay, m_Layers);

            var selectedLayers = new List<ILayer>();
            if (selectedLayersCount.HasValue)
            {
                for (var i = 0; i < selectedLayersCount.Value; i++)
                {
                    selectedLayers.Add(m_Layers[i]);
                }
            }
            m_OverlayWithCirclesViewModel.SelectedLayers = selectedLayers;

            var layerToBeRemoved = m_Layers[remoingLayerIndex];
            var layerIsSelected = selectedLayers.Contains(layerToBeRemoved);
            // Act.
            m_OverlayWithCirclesViewModel.RemoveLayer(layerToBeRemoved);
            // Assert.
            if (!selectedLayersCount.HasValue)
            {
                Assert.AreEqual(0, m_OverlayWithCirclesViewModel.SelectedLayers.Count);
                Assert.IsFalse(m_OverlayWithCirclesViewModel.IsFilled);
                Assert.IsTrue(m_OverlayWithCirclesViewModel.HasNoSelectedLayers);
                Assert.IsNull(m_OverlayWithCirclesViewModel.TopLayer);
                Assert.IsNull(m_OverlayWithCirclesViewModel.BottomLayer);
                return;
            }
            Assert.IsNotNull(m_OverlayWithCirclesViewModel.SelectedLayers);
            Assert.AreEqual(m_OverlayWithCirclesViewModel.SelectedLayers.Count,
                layerIsSelected ? selectedLayersCount.Value - 1 : selectedLayersCount.Value);
            Assert.IsFalse(m_OverlayWithCirclesViewModel.SelectedLayers.Contains(layerToBeRemoved));
        }

        [Test]
        public void SetLayersTest()
        {
            // Arrange.
            m_OverlayWithCirclesViewModel = new OverlayWithCirclesViewModel(m_Overlay, m_Layers);
            var layers = new List<ILayer> { m_Layers[0], m_Layers[2] };
            // Act.
            m_OverlayWithCirclesViewModel.SetLayers(layers);
            // Assert.
            Assert.IsNotNull(m_OverlayWithCirclesViewModel.Layers);
            Assert.AreEqual(m_OverlayWithCirclesViewModel.Layers.Count, layers.Count);
            Assert.IsNotNull(m_OverlayWithCirclesViewModel.SelectedLayers);
            Assert.AreEqual(m_OverlayWithCirclesViewModel.SelectedLayers.Count, 1);
            Assert.IsTrue(m_OverlayWithCirclesViewModel.SelectedLayers.Contains(m_Layers[0]));
            Assert.IsFalse(m_OverlayWithCirclesViewModel.SelectedLayers.Contains(m_Layers[1]));
            Assert.IsFalse(m_OverlayWithCirclesViewModel.IsFilled);
            Assert.IsFalse(m_OverlayWithCirclesViewModel.HasNoSelectedLayers);
            Assert.IsNull(m_OverlayWithCirclesViewModel.TopLayer);
            Assert.IsNull(m_OverlayWithCirclesViewModel.BottomLayer);
        }

        [TestCase(null)]
        [TestCase("a")]
        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void CircleMouseDownExecuteTest(object layerIndexObject)
        {
            // Arrange.
            m_OverlayWithCirclesViewModel = new OverlayWithCirclesViewModel(m_Overlay, m_Layers);
            var layerIndexIsInt = layerIndexObject is int;
            var layerIndex = -1;
            if (layerIndexIsInt)
            {
                layerIndex = (int)layerIndexObject;
            }
            // Assert.
            Assert.AreEqual(m_OverlayWithCirclesViewModel.SelectedLayers.Count, 2);
            Assert.IsTrue(m_OverlayWithCirclesViewModel.SelectedLayers.Contains(m_Layers[0]));
            Assert.IsTrue(m_OverlayWithCirclesViewModel.SelectedLayers.Contains(m_Layers[1]));
            // Act.
            m_OverlayWithCirclesViewModel.CircleMouseDownExecute(layerIndexObject);
            // Assert.
            if (layerIndexIsInt && (layerIndex == 0 || layerIndex == 1))
            {
                Assert.AreEqual(m_OverlayWithCirclesViewModel.SelectedLayers.Count, 1);
                Assert.IsFalse(m_OverlayWithCirclesViewModel.SelectedLayers.Contains(m_Layers[layerIndex]));
                Assert.IsFalse(m_OverlayWithCirclesViewModel.IsFilled);
                Assert.IsFalse(m_OverlayWithCirclesViewModel.HasNoSelectedLayers);
                Assert.IsNull(m_OverlayWithCirclesViewModel.TopLayer);
                Assert.IsNull(m_OverlayWithCirclesViewModel.BottomLayer);
            }
            else
            {
                Assert.AreEqual(m_OverlayWithCirclesViewModel.SelectedLayers.Count, 2);
                Assert.IsTrue(m_OverlayWithCirclesViewModel.SelectedLayers.Contains(m_Layers[0]));
                Assert.IsTrue(m_OverlayWithCirclesViewModel.SelectedLayers.Contains(m_Layers[1]));
                Assert.IsTrue(m_OverlayWithCirclesViewModel.IsFilled);
                Assert.IsFalse(m_OverlayWithCirclesViewModel.HasNoSelectedLayers);
                Assert.IsNotNull(m_OverlayWithCirclesViewModel.TopLayer);
                Assert.AreEqual(m_OverlayWithCirclesViewModel.TopLayer.Name, m_Layers[1].Name);
                Assert.IsNotNull(m_OverlayWithCirclesViewModel.BottomLayer);
                Assert.AreEqual(m_OverlayWithCirclesViewModel.BottomLayer.Name, m_Layers[0].Name);
            }
        }
    }
}
