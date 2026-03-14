using DefineOverlayTree.DefineOverlayPanel.GraphicalOverlayView;
using DefineOverlayTree.Logic;
using Moq;
using NUnit.Framework;

namespace DefineOverlayTree.UnitTests.DefineOverlayPanel.GraphicalOverlayView
{
    [TestFixture]
    public class CellTest
    {
        private Cell m_Cell;

        [SetUp]
        public void Setup()
        {
            m_Cell = new Cell
            {
                Layer = new Mock<ILayer>().Object,
                Overlay = new Mock<IOverlay>().Object
            };
        }

        [Test]
        public void CellIsNotNull()
        {
            // Assert.
            Assert.IsNotNull(m_Cell);
        }

        [Test]
        public void CommandsAndPropertiesAreCreated()
        {
            // Assert.
            Assert.IsNotNull(m_Cell.Layer);
            Assert.IsNotNull(m_Cell.Overlay);
            Assert.IsNotNull(m_Cell.IsActive);
            Assert.IsFalse(m_Cell.IsActive);
            Assert.IsNotNull(m_Cell.IsValid);
            Assert.IsTrue(m_Cell.IsValid);
            Assert.IsNotNull(m_Cell.ClickedCommand);
        }

        [Test]
        public void ClickedCommandAndPointToCellRequestedWork()
        {
            // Arrange.
            var isSelected = false;
            m_Cell.PointToCellRequested += (sender, args) => isSelected = true;
            // Act.
            m_Cell.ClickedCommand.Execute(null);
            // Assert.
            Assert.IsTrue(isSelected);
        }

        [Test]
        public void PropertiesAreChanging()
        {
            // Act.
            m_Cell.IsActive = true;
            m_Cell.IsValid = false;
            // Assert.
            Assert.IsTrue(m_Cell.IsActive);
            Assert.IsFalse(m_Cell.IsValid);
        }

        [Test]
        public void PropertiesAreTheSame()
        {
            // Act.
            m_Cell.IsActive = false;
            m_Cell.IsValid = true;
            // Assert.
            Assert.IsFalse(m_Cell.IsActive);
            Assert.IsTrue(m_Cell.IsValid);
        }
    }
}
