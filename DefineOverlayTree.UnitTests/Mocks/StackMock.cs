using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DefineOverlayTree.Logic;
using Moq;

namespace DefineOverlayTree.UnitTests.Mocks
{
    public class StackMock : Mock<IStack>
    {
        public StackMock()
        {
            const int maxNrOfLayers = 10;

            LayerMocks = new List<Mock<ILayer>>();
            Setup(s => s.Layers).Returns(() => LayerMocks.Select(lm => lm.Object).ToList());
            Setup(s => s.MaxNrOfLayers).Returns(maxNrOfLayers);
            Setup(s => s.CanAddLayer()).Returns(LayerMocks.Count <= maxNrOfLayers);
            Setup(s => s.AddLayer(It.IsAny<string>()))
                .Callback((string name) =>
                {
                    GenerateNumberOfLayers(1);
                });
            Setup(s => s.RemoveLayer(It.IsAny<ILayer>()))
                .Callback((ILayer layer) =>
                {
                    LayerMocks.RemoveAll(lm => Equals(lm.Object, layer));
                    Raise(s => s.StackChanged += null);
                });
        }

        public List<Mock<ILayer>> LayerMocks { get; }

        public StackMock GenerateNumberOfLayers(int count)
        {
            var lastLayerName = LayerMocks.LastOrDefault()?.Object.Name ?? "layer0";
            var lastIndexLayer = Regex.Match(lastLayerName, @"\d+$").Value;
            int.TryParse(lastIndexLayer, out var indexNumber);

            for (var i = ++indexNumber; i < indexNumber + count; i++)
            {
                var layerWrapper = new Mock<ILayer>();
                layerWrapper.Setup(l => l.Name).Returns("Layer " + i);
                LayerMocks.Add(layerWrapper);
            }
            Raise(s => s.StackChanged += null);

            return this;
        }
    }
}
