using DefineOverlayTree.DefineOverlayPanel.GraphicalOverlayView;
using DefineOverlayTree.Logic;
using Moq;
using NUnit.Framework;

namespace DefineOverlayTree.UnitTests.DefineOverlayPanel.GraphicalOverlayView
{
    [TestFixture]
    public class LayerEventArgsTest
    {
        private LayerEventArgs m_LayerEventArgs;
        private const string LayerName = "TheName";

        [SetUp]
        public void Setup()
        {
            var layerWrapper = new Mock<ILayer>();
            layerWrapper.Setup(x => x.Name).Returns(LayerName);
            m_LayerEventArgs = new LayerEventArgs(layerWrapper.Object);
        }

        [Test]
        public void CellIsNotNull()
        {
            // Assert.
            Assert.IsNotNull(m_LayerEventArgs);
        }

        [Test]
        public void PropertiesAreCreated()
        {
            // Assert.
            Assert.IsNotNull(m_LayerEventArgs.Layer);
            Assert.AreEqual(m_LayerEventArgs.Layer.Name, LayerName);
        }
    }
}
