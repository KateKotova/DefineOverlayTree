using System.Collections.ObjectModel;
using System.Linq;
using Moq;
using NUnit.Framework;
using DefineOverlayTree.DefineOverlayPanel;
using DefineOverlayTree.DefineOverlayPanel.GraphicalOverlayView;
using DefineOverlayTree.UnitTests.Mocks;

namespace DefineOverlayTree.UnitTests.DefineOverlayPanel.GraphicalOverlayView
{
    [TestFixture]
    public class GraphicalOverlayViewModelTest
    {
        private GraphicalOverlayViewModel m_GraphicalOverlayViewModel;
        private ToolMock m_ToolMock;
        private OverlayService m_OverlayService;

        private const string OverlayName = "Overlay ";
        private const string LayerName = "Layer ";

        [SetUp]
        public void Setup()
        {
            const int numberOfLayers = 3;
            const int numberOfOverlays = 1;
            var overlayCandidatesServiceMock = new OverlayCandidatesServiceMock();

            m_ToolMock = new ToolMock().WithStackMock(numberOfLayers).WithOverlayMapMock();
            m_ToolMock.OverlayMapMock.GenerateOverlays(numberOfOverlays, m_ToolMock.StackMock.Object.Layers);

            m_OverlayService = new OverlayService(m_ToolMock.StackMock.Object, m_ToolMock.OverlayMapMock.Object,
                overlayCandidatesServiceMock.Object);

            m_GraphicalOverlayViewModel = new GraphicalOverlayViewModel(m_ToolMock.Object, m_OverlayService);
        }

        [Test]
        public void InitializeTest()
        {
            var stack = m_ToolMock.StackMock.Object;

            // Assert
            Assert.IsNotNull(m_GraphicalOverlayViewModel.Overlays);
            Assert.AreEqual(1, m_GraphicalOverlayViewModel.Overlays.Count);
            Assert.IsNotNull(m_GraphicalOverlayViewModel.NewLayerDefaultName);
            Assert.AreEqual(LayerName + "4", m_GraphicalOverlayViewModel.NewLayerDefaultName);
            Assert.IsNotNull(m_GraphicalOverlayViewModel.OverlayNames);
            Assert.AreEqual(4, m_GraphicalOverlayViewModel.OverlayNames.Count);
            Assert.IsTrue(m_GraphicalOverlayViewModel.OverlayNames.Any(on
                => on.Name.Equals(m_OverlayService.Overlays[0].Name)));
            Assert.AreEqual(3, m_GraphicalOverlayViewModel.Layers.Count);
            Assert.IsTrue(m_GraphicalOverlayViewModel.Layers.Any(l => l.Name.Equals(stack.Layers[0].Name)));
            Assert.IsTrue(m_GraphicalOverlayViewModel.Layers.Any(l => l.Name.Equals(stack.Layers[1].Name)));
            Assert.IsTrue(m_GraphicalOverlayViewModel.Layers.Any(l => l.Name.Equals(stack.Layers[2].Name)));
            Assert.IsTrue(m_GraphicalOverlayViewModel.Layers[0].IsFirst);
            Assert.IsFalse(m_GraphicalOverlayViewModel.Layers[1].IsFirst);
            Assert.IsFalse(m_GraphicalOverlayViewModel.Layers[2].IsFirst);
            Assert.AreEqual(3, m_GraphicalOverlayViewModel.ReversedLayers.Count);
            Assert.IsTrue(m_GraphicalOverlayViewModel.ReversedLayers.Any(l => l.Name.Equals(stack.Layers[0].Name)));
            Assert.IsTrue(m_GraphicalOverlayViewModel.ReversedLayers.Any(l => l.Name.Equals(stack.Layers[1].Name)));
            Assert.IsTrue(m_GraphicalOverlayViewModel.ReversedLayers.Any(l => l.Name.Equals(stack.Layers[2].Name)));
            Assert.IsNull(m_GraphicalOverlayViewModel.NewLayerName);
            Assert.AreEqual($"* Up to {stack.MaxNrOfLayers} layers",
                m_GraphicalOverlayViewModel.MaximumLayersCountMessage);
            Assert.IsNotNull(m_GraphicalOverlayViewModel.NewLayerNotEmptyName);
            Assert.AreEqual(LayerName + "4", m_GraphicalOverlayViewModel.NewLayerNotEmptyName);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase(" LayerNewName  ")]
        public void NewLayerNameTest(string layerName)
        {
            // Arrange
            var newLayerNotEmptyName = string.IsNullOrWhiteSpace(layerName) ? LayerName + "4" : layerName.Trim();

            // Act
            m_GraphicalOverlayViewModel.NewLayerName = layerName;

            // Assert
            Assert.AreEqual(layerName, m_GraphicalOverlayViewModel.NewLayerName);
            Assert.AreEqual(newLayerNotEmptyName, m_GraphicalOverlayViewModel.NewLayerNotEmptyName);
        }

        [Test]
        public void AddLayerCanExecuteTest()
        {
            //Arrange
            var maxNumberOfLayers = m_ToolMock.StackMock.Object.MaxNrOfLayers;
            m_ToolMock.StackMock.Setup(s => s.CanAddLayer()).Returns(()
                => m_ToolMock.StackMock.Object.Layers.Count < maxNumberOfLayers);
            m_ToolMock.StackMock.Setup(s => s.IsValidLayerName(It.IsAny<string>())).Returns(true);

            for (var i = m_GraphicalOverlayViewModel.Layers.Count; i <= maxNumberOfLayers; i++)
            {
                // Arrange
                m_GraphicalOverlayViewModel.AddLayer.Execute(null);

                // Act
                var canAddLayer = m_GraphicalOverlayViewModel.AddLayer.CanExecute(null);

                // Assert
                Assert.AreEqual(canAddLayer, m_ToolMock.StackMock.Object.Layers.Count < maxNumberOfLayers);
            }
        }

        [Test]
        public void AddLayerTest()
        {
            //Arrange
            var maxNumberOfLayers = m_ToolMock.StackMock.Object.MaxNrOfLayers;

            for (var i = m_GraphicalOverlayViewModel.Layers.Count; i <= maxNumberOfLayers; i++)
            {
                // Act
                m_GraphicalOverlayViewModel.AddLayer.Execute(null);

                // Assert
                Assert.AreEqual(m_GraphicalOverlayViewModel.NewLayerDefaultName,
                    LayerName + (m_GraphicalOverlayViewModel.Layers.Count + 1));
                Assert.AreEqual(m_GraphicalOverlayViewModel.Layers.Count, i + 1);
                Assert.AreEqual(m_GraphicalOverlayViewModel.Layers.Last().Name,
                    m_ToolMock.StackMock.Object.Layers[i].Name);
                Assert.AreEqual(m_GraphicalOverlayViewModel.ReversedLayers.Count, i + 1);
                Assert.AreEqual(m_GraphicalOverlayViewModel.ReversedLayers.First().Name,
                    m_ToolMock.StackMock.Object.Layers[i].Name);
                Assert.AreEqual(m_GraphicalOverlayViewModel.NewLayerName, string.Empty);
            }
        }

        [Test]
        public void ReversedLayersTest()
        {
            //Arrange
            var maxNumberOfLayers = m_ToolMock.StackMock.Object.MaxNrOfLayers;

            for (var i = m_GraphicalOverlayViewModel.Layers.Count; i <= maxNumberOfLayers; i++)
            {
                // Act
                m_GraphicalOverlayViewModel.AddLayer.Execute(null);

                // Assert
                Assert.AreEqual(m_GraphicalOverlayViewModel.Layers.Count, i + 1);
                Assert.IsNotNull(m_GraphicalOverlayViewModel.ReversedLayers);
                Assert.AreEqual(m_GraphicalOverlayViewModel.ReversedLayers.Count, i + 1);

                for (int index = 0, reversedIndex = m_GraphicalOverlayViewModel.Layers.Count - 1;
                     index < m_GraphicalOverlayViewModel.Layers.Count;
                     index++, reversedIndex--)
                {
                    Assert.AreEqual(m_GraphicalOverlayViewModel.Layers[index].Name,
                        m_GraphicalOverlayViewModel.ReversedLayers[reversedIndex].Name);
                }
            }

            // Act
            m_GraphicalOverlayViewModel.Layers = null;

            // Assert
            Assert.IsNull(m_GraphicalOverlayViewModel.Layers);
            Assert.IsNull(m_GraphicalOverlayViewModel.ReversedLayers);
        }

        [Test]
        public void SetNullLayersTest()
        {
            // Act.
            m_GraphicalOverlayViewModel.Layers = null;
            // Assert.
            Assert.AreEqual(m_GraphicalOverlayViewModel.NewLayerDefaultName, LayerName + "1");
            Assert.IsNull(m_GraphicalOverlayViewModel.Layers);
            Assert.IsNull(m_GraphicalOverlayViewModel.ReversedLayers);
            Assert.IsNull(m_GraphicalOverlayViewModel.NewLayerName);
            Assert.IsNotNull(m_GraphicalOverlayViewModel.NewLayerNotEmptyName);
            Assert.AreEqual(m_GraphicalOverlayViewModel.NewLayerNotEmptyName, LayerName + "1");
        }

        [Test]
        public void SetZeroLayersTest()
        {
            // Act.
            m_GraphicalOverlayViewModel.Layers = new ObservableCollection<LayerViewModel>();
            // Assert.
            Assert.AreEqual(m_GraphicalOverlayViewModel.NewLayerDefaultName, LayerName + "1");
            Assert.AreEqual(m_GraphicalOverlayViewModel.Layers.Count, 0);
            Assert.AreEqual(m_GraphicalOverlayViewModel.ReversedLayers.Count, 0);
            Assert.IsNull(m_GraphicalOverlayViewModel.NewLayerName);
            Assert.IsNotNull(m_GraphicalOverlayViewModel.NewLayerNotEmptyName);
            Assert.AreEqual(m_GraphicalOverlayViewModel.NewLayerNotEmptyName, LayerName + "1");
        }

        [Test]
        public void RenameOverlayTest()
        {
            // Arrange
            const string newName = OverlayName + "11";

            // Act
            m_OverlayService.Overlays[0].Name = newName;

            // Assert
            Assert.AreEqual(m_GraphicalOverlayViewModel.Overlays[0].Name, newName);
        }

        [Test]
        public void RenameLayerViewModelTest()
        {
            // Arrange
            m_ToolMock.StackMock.Setup(s => s.IsValidLayerName(It.IsAny<string>())).Returns(true);
            const string newName = LayerName + "11";

            // Act
            m_GraphicalOverlayViewModel.Layers[0].Name = newName;
            m_ToolMock.StackMock.Raise(s => s.StackChanged += null);

            // Assert
            m_ToolMock.StackMock.LayerMocks[0].VerifySet(l => l.Name = newName, Times.Once());
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void OnLayerDeleteRequestedTest(int layerIndex)
        {
            // Arrange
            var removingLayerName = m_GraphicalOverlayViewModel.Layers[layerIndex].Name;

            // Act
            m_GraphicalOverlayViewModel.Layers[layerIndex].DeleteCommand.Execute(null);

            // Assert
            Assert.AreEqual(m_GraphicalOverlayViewModel.Layers.Count, 2);
            Assert.IsFalse(m_GraphicalOverlayViewModel.Layers.Any(l => l.Name.Equals(removingLayerName)));
            Assert.IsNotNull(m_GraphicalOverlayViewModel.ReversedLayers);
            Assert.AreEqual(m_GraphicalOverlayViewModel.ReversedLayers.Count, 2);
            Assert.IsFalse(m_GraphicalOverlayViewModel.ReversedLayers.Any(l => l.Name.Equals(removingLayerName)));
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void AddFirstPointOfCreatingOverlayTest(int cellIndex)
        {
            // Act
            m_GraphicalOverlayViewModel.Layers[2].Cells[cellIndex].ClickedCommand.Execute(null);

            // Assert
            Assert.AreEqual(2, m_GraphicalOverlayViewModel.Overlays.Count);
            Assert.AreEqual(OverlayName + "2", m_GraphicalOverlayViewModel.Overlays[1].Name);

            Assert.AreEqual(1, m_GraphicalOverlayViewModel.Overlays[1].SelectedLayers.Count);
            Assert.IsTrue(m_GraphicalOverlayViewModel.Overlays[1].SelectedLayers.Contains
                (m_ToolMock.StackMock.Object.Layers[2]));
            Assert.IsNull(m_GraphicalOverlayViewModel.Overlays[1].TopLayer);
            Assert.IsNull(m_GraphicalOverlayViewModel.Overlays[1].BottomLayer);
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void TryToSelectSelectedPointTest(int cellIndex)
        {
            // Arrange
            m_GraphicalOverlayViewModel.Layers[2].Cells[cellIndex].ClickedCommand.Execute(null);

            // Act
            m_GraphicalOverlayViewModel.Layers[2].Cells[cellIndex].ClickedCommand.Execute(null);

            // Assert
            Assert.AreEqual(m_GraphicalOverlayViewModel.Overlays.Count, 2);
            Assert.AreEqual(m_GraphicalOverlayViewModel.Overlays[1].Name, OverlayName + "2");

            Assert.AreEqual(m_GraphicalOverlayViewModel.Overlays[1].SelectedLayers.Count, 1);
            Assert.IsTrue(m_GraphicalOverlayViewModel.Overlays[1].SelectedLayers.Contains(m_ToolMock.StackMock.Object.Layers[2]));
            Assert.IsNull(m_GraphicalOverlayViewModel.Overlays[1].TopLayer);
            Assert.IsNull(m_GraphicalOverlayViewModel.Overlays[1].BottomLayer);
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void AddSecondPointToCreatingOverlayAndCreateOverlayModelTest(int cellIndex)
        {
            // Arrange
            var overlays = m_OverlayService.Overlays;
            var layers = m_ToolMock.StackMock.Object.Layers;

            m_GraphicalOverlayViewModel.Layers[2].Cells[cellIndex].ClickedCommand.Execute(null);

            // Act
            m_GraphicalOverlayViewModel.Layers[0].Cells[cellIndex].ClickedCommand.Execute(null);

            // Assert
            Assert.AreEqual(m_GraphicalOverlayViewModel.Overlays.Count, 2);
            Assert.IsTrue(m_GraphicalOverlayViewModel.OverlayNames.Any(on => on.Name.Equals(overlays[1].Name)));

            Assert.AreEqual(m_GraphicalOverlayViewModel.Overlays[1].SelectedLayers.Count, 2);
            Assert.IsTrue(m_GraphicalOverlayViewModel.Overlays[1].SelectedLayers.Contains(layers[0]));
            Assert.IsTrue(m_GraphicalOverlayViewModel.Overlays[1].SelectedLayers.Contains(layers[2]));
            Assert.IsNotNull(m_GraphicalOverlayViewModel.Overlays[1].TopLayer);
            Assert.IsNotNull(m_GraphicalOverlayViewModel.Overlays[1].BottomLayer);
            Assert.AreEqual(m_GraphicalOverlayViewModel.Overlays[1].TopLayer.Name, layers[2].Name);
            Assert.AreEqual(m_GraphicalOverlayViewModel.Overlays[1].BottomLayer.Name, layers[0].Name);
        }

        [Test]
        public void DeleteSecondPointFromEditingOverlayTest()
        {
            // Arrange
            var layers = m_ToolMock.StackMock.Object.Layers;
            m_GraphicalOverlayViewModel.Layers[2].Cells[0].ClickedCommand.Execute(null);
            m_GraphicalOverlayViewModel.Layers[0].Cells[0].ClickedCommand.Execute(null);

            // Act
            m_GraphicalOverlayViewModel.Overlays[1].CircleMouseDownCommand.Execute(0);

            // Assert
            Assert.AreEqual(2, m_GraphicalOverlayViewModel.Overlays.Count);
            Assert.AreEqual(2, m_OverlayService.Overlays.Count);

            Assert.AreEqual(1, m_GraphicalOverlayViewModel.Overlays[1].SelectedLayers.Count);
            Assert.IsTrue(m_GraphicalOverlayViewModel.Overlays[1].SelectedLayers.Contains(layers[0]));
            Assert.IsFalse(m_GraphicalOverlayViewModel.Overlays[1].SelectedLayers.Contains(layers[2]));
            Assert.IsNull(m_GraphicalOverlayViewModel.Overlays[1].TopLayer);
            Assert.IsNull(m_GraphicalOverlayViewModel.Overlays[1].BottomLayer);
        }

        [Test]
        public void DeleteCreatingOverlayTest()
        {
            // Arrange
            m_GraphicalOverlayViewModel.Layers[2].Cells[0].ClickedCommand.Execute(null);

            // Act
            m_GraphicalOverlayViewModel.Overlays[1].CircleMouseDownCommand.Execute(0);

            // Assert
            Assert.AreEqual(1, m_GraphicalOverlayViewModel.Overlays.Count);
        }

        [Test]
        public void DeleteTheOnlyPointFromEditingOverlayTest()
        {
            // Arrange
            m_GraphicalOverlayViewModel.Overlays[0].CircleMouseDownCommand.Execute(1);

            // Act
            m_GraphicalOverlayViewModel.Overlays[0].CircleMouseDownCommand.Execute(0);

            // Assert
            Assert.IsNotNull(m_GraphicalOverlayViewModel.Overlays);
            Assert.AreEqual(0, m_GraphicalOverlayViewModel.Overlays.Count);
        }

        [Test]
        public void AddSecondPointToEditingOverlayTest()
        {
            // Arrange
            var layers = m_ToolMock.StackMock.Object.Layers;
            m_GraphicalOverlayViewModel.Overlays[0].CircleMouseDownCommand.Execute(1);

            // Act
            m_GraphicalOverlayViewModel.Layers[2].Cells[0].ClickedCommand.Execute(null);

            // Assert
            Assert.AreEqual(1, m_GraphicalOverlayViewModel.Overlays.Count);

            Assert.AreEqual(2, m_GraphicalOverlayViewModel.Overlays[0].SelectedLayers.Count);
            Assert.IsTrue(m_GraphicalOverlayViewModel.Overlays[0].SelectedLayers.Contains(layers[2]));
            Assert.IsNotNull(m_GraphicalOverlayViewModel.Overlays[0].TopLayer);
            Assert.AreEqual(layers[2].Name, m_GraphicalOverlayViewModel.Overlays[0].TopLayer.Name);
            Assert.IsNotNull(m_GraphicalOverlayViewModel.Overlays[0].BottomLayer);
        }

        [TestCase(0, true)]
        [TestCase(2, false)]
        public void CellsAreNotActive_WhenLayerCountIsLowerThenTwoTest(int removeLayerCount, bool expectedValue)
        {
            // Act
            for (var i = 0; i < removeLayerCount; i++)
            {
                m_ToolMock.StackMock.LayerMocks.RemoveAt(m_ToolMock.StackMock.LayerMocks.Count - 1);
            }
            m_ToolMock.StackMock.Raise(s => s.StackChanged += null);

            // Assert
            Assert.AreEqual(expectedValue, m_GraphicalOverlayViewModel.Layers[0].Cells[1].IsActive);
        }
    }
}
