using Moq;
using NUnit.Framework;
using System.Text.RegularExpressions;
using DefineOverlayTree.DefineOverlayPanel;
using DefineOverlayTree.DefineOverlayPanel.GraphicalOverlayView;
using DefineOverlayTree.Logic;
using DefineOverlayTree.UnitTests.Mocks;

namespace DefineOverlayTree.UnitTests.DefineOverlayPanel.GraphicalOverlayView
{
    [TestFixture]
    public class LayerViewModelTest
    {
        private ToolMock m_ToolMock;

        private LayerViewModel m_LayerViewModel;

        [SetUp]
        public void Setup()
        {
            const int numberOfLayers = 3;

            m_ToolMock = new ToolMock().WithStackMock(numberOfLayers)
                                             .WithOverlayMapMock();
            var layers = m_ToolMock.StackMock.Object.Layers;

            var overlayCandidatesService = new Mock<IOverlayCandidatesService>();
            var overlayService = new OverlayService(m_ToolMock.StackMock.Object,
                m_ToolMock.OverlayMapMock.Object, overlayCandidatesService.Object);

            m_LayerViewModel = new LayerViewModel(layers[0], m_ToolMock.Object, overlayService, HasValidNameFormat)
            {
                IsFirst = true
            };
        }

        [Test]
        public void LayerViewModelIsNotNull()
        {
            // Assert.
            Assert.IsNotNull(m_LayerViewModel);
        }

        [Test]
        public void CommandsAndPropertiesAreCreated()
        {
            // Arrange.
            var layer = m_ToolMock.Object.Stack.Layers[0];
            // Assert.
            Assert.IsNotNull(m_LayerViewModel.LayerModel);
            Assert.AreEqual(m_LayerViewModel.LayerModel.Name, layer.Name);
            Assert.IsNotNull(m_LayerViewModel.Cells);
            Assert.IsNotNull(m_LayerViewModel.DeleteCommand);
            Assert.IsNotNull(m_LayerViewModel.CanRemoveLayer);
            Assert.IsTrue(m_LayerViewModel.CanRemoveLayer);
            Assert.IsNotNull(m_LayerViewModel.Name);
            Assert.AreEqual(m_LayerViewModel.Name, layer.Name);
            Assert.IsNotNull(m_LayerViewModel.IsFirst);
            Assert.IsTrue(m_LayerViewModel.IsFirst);
        }

        [Test]
        public void CellsAreCreatedCorrectly()
        {
            // Arrange.
            var layer = m_ToolMock.Object.Stack.Layers[0];
            // Assert.
            CheckCellsAreCreatedCorrectlyForLayer(layer);
        }

        [Test]
        public void DeleteCommandAndDeleteRequestedWork()
        {
            // Arrange.
            var deleteIsRequested = false;
            m_LayerViewModel.DeleteRequested += (senger, args) => deleteIsRequested = true;
            // Act.
            m_LayerViewModel.DeleteCommand.Execute(null);
            // Assert.
            Assert.IsTrue(deleteIsRequested);
        }

        [Test]
        public void UpdateRemovingAbilityOnStackChangedWorks()
        {
            // Arrange.
            m_ToolMock.StackMock.Object.StackChanged += () => m_ToolMock.Setup(x => x.Stack).Returns((IStack)null);
            // Act.
            m_ToolMock.StackMock.Raise(m => m.StackChanged += null);
            // Assert.
            Assert.IsFalse(m_LayerViewModel.CanRemoveLayer);
        }

        [Test]
        public void UpdateRemovingAbilityOnOverlayMapChanged()
        {
            // Arrange.
            m_ToolMock.OverlayMapMock.Object.OverlayMapChanged += ()
                => m_ToolMock.Setup(x => x.Stack).Returns((IStack)null);
            // Act.
            m_ToolMock.OverlayMapMock.Raise(m => m.OverlayMapChanged += null);
            // Assert.
            Assert.IsFalse(m_LayerViewModel.CanRemoveLayer);
        }

        [Test]
        public void SetCorrectNameWorks()
        {
            // Arrange.
            const string newName = "NewName123";
            // Act.
            m_LayerViewModel.Name = newName;

            // Assert
            Assert.IsTrue(HasValidNameFormat(newName));
            m_ToolMock.StackMock.LayerMocks[0].VerifySet(l => l.Name = It.IsAny<string>(), Times.Once);
        }

        [Test]
        public void SetIncorrectNameDoesNotWork()
        {
            // Arrange.
            const string newName = "123NewName";
            // Act.
            m_LayerViewModel.Name = newName;
            // Assert.
            Assert.IsFalse(HasValidNameFormat(newName));
            m_ToolMock.StackMock.LayerMocks[0].VerifySet(l => l.Name = It.IsAny<string>(), Times.Never);
        }

        [Test]
        public void ToStringReturnsName()
        {
            // Assert.
            Assert.AreEqual(m_LayerViewModel.Name, m_LayerViewModel.ToString());
        }

        [Test]
        public void InitWorks()
        {
            // Arrange.
            var layer = m_ToolMock.StackMock.Object.Layers[1];
            // Act.
            m_LayerViewModel.Init(layer, m_ToolMock.Object);
            m_LayerViewModel.IsFirst = true;
            // Asset.
            Assert.AreEqual(m_LayerViewModel.LayerModel.Name, layer.Name);
            Assert.IsNotNull(m_LayerViewModel.Cells);
            Assert.IsNotNull(m_LayerViewModel.CanRemoveLayer);
            Assert.IsTrue(m_LayerViewModel.CanRemoveLayer);
            Assert.AreEqual(m_LayerViewModel.Name, layer.Name);

            CheckCellsAreCreatedCorrectlyForLayer(layer);
        }

        private static bool HasValidNameFormat(string name)
        {
            var pattern = new Regex("^[A-Za-z][A-Za-z0-9 ]*$");
            return pattern.IsMatch(name);
        }

        public void CheckCellsAreCreatedCorrectlyForLayer(ILayer layer)
        {
            // Assert.
            var overlayMap = m_ToolMock.Object.OverlayMap;

            Assert.AreEqual(m_LayerViewModel.Cells.Count, overlayMap.MaxNrOfOverlays);
            for (var overlayIndex = 0; overlayIndex < overlayMap.MaxNrOfOverlays; overlayIndex++)
            {
                var cell = m_LayerViewModel.Cells[overlayIndex];
                var overlay = overlayIndex >= overlayMap.Overlays.Count
                    ? null
                    : overlayMap.Overlays[overlayIndex];
                Assert.IsNotNull(cell);
                Assert.AreEqual(cell.Layer, layer);
                Assert.AreEqual(cell.Overlay, overlay);
                Assert.IsTrue(cell.IsValid);
                Assert.IsFalse(cell.IsActive);
            }
        }
    }
}
