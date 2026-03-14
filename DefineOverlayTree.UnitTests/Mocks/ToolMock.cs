using System.Collections.Generic;
using DefineOverlayTree.Logic;
using Moq;

namespace DefineOverlayTree.UnitTests.Mocks
{
    public class ToolMock : Mock<ITool>
    {
        public StackMock StackMock { get; private set; }
        public OverlayMapMock OverlayMapMock { get; private set; }

        public ToolMock WithStackMock(int numberOfLayers = 0)
        {
            StackMock = new StackMock();
            StackMock.GenerateNumberOfLayers(numberOfLayers);
            Setup(t => t.Stack).Returns(StackMock.Object);

            return this;
        }

        public ToolMock WithOverlayMapMock(int numberOfOverlays = 0, IList<ILayer> layers = null)
        {
            OverlayMapMock = new OverlayMapMock();
            OverlayMapMock.GenerateOverlays(numberOfOverlays, layers);
            Setup(t => t.OverlayMap).Returns(OverlayMapMock.Object);

            return this;
        }
    }
}
