using System.Collections.Generic;
using System.Linq;
using DefineOverlayTree.Logic;
using Moq;

namespace DefineOverlayTree.UnitTests.Mocks
{
    public class OverlayMapMock : Mock<IOverlayMap>
    {
        public OverlayMapMock()
        {
            const int maxNrOfOverlays = 4;

            OverlayMocks = new List<Mock<IOverlay>>();
            Setup(om => om.MaxNrOfOverlays).Returns(maxNrOfOverlays);
            Setup(om => om.Overlays).Returns(() => OverlayMocks.Select(lm => lm.Object).ToList());
            Setup(om => om.CanAddOverlay()).Returns(OverlayMocks.Count <= maxNrOfOverlays);
            Setup(x => x.CanRemoveOverlay(It.IsAny<IOverlay>()))
                .Returns((IOverlay overlay) => OverlayMocks.Any(om => om.Object.Name.Equals(overlay.Name)));
            Setup(x => x.AddOverlay(It.IsAny<string>(), It.IsAny<IOverlayDefinition>()))
                .Returns((string name, IOverlayDefinition overlayDefinition) =>
                {
                    var overlay = CreateOverlayMock(name, overlayDefinition);
                    OverlayMocks.Add(overlay);
                    Raise(o => o.OverlayMapChanged += null);
                    return overlay.Object;
                });
            Setup(x => x.InsertOverlay(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<IOverlayDefinition>()))
                .Returns((int index, string name, IOverlayDefinition overlayDefinition) =>
                {
                    var overlay = CreateOverlayMock(name, overlayDefinition);
                    OverlayMocks.Insert(index, overlay);
                    Raise(o => o.OverlayMapChanged += null);
                    return overlay.Object;
                });
            Setup(x => x.RemoveOverlay(It.IsAny<IOverlay>()))
                .Callback((IOverlay overlay) =>
                {
                    OverlayMocks.RemoveAll(om => Equals(om.Object, overlay));
                    Raise(o => o.OverlayMapChanged += null);
                });
        }

        public List<Mock<IOverlay>> OverlayMocks { get; }

        public OverlayMapMock GenerateOverlays(int count, IList<ILayer> layers = null)
        {
            var overlayDefinitionMocks = new OverlayCandidatesServiceMock().GenerateOverlayDefinitions
                (layers ?? new List<ILayer>()).OverlayDefinitionMocks;

            for (var i = 1; i <= count; i++)
            {
                var overlayDefinitionWrapper = overlayDefinitionMocks.Count > i ? overlayDefinitionMocks[i] : new Mock<IOverlayDefinition>();
                OverlayMocks.Add(CreateOverlayMock("Overlay" + i, overlayDefinitionWrapper.Object));
            }
            Raise(o => o.OverlayMapChanged += null);

            return this;
        }

        private static Mock<IOverlay> CreateOverlayMock(string name, IOverlayDefinition overlayDefinition)
        {
            var overlayWrapper = new Mock<IOverlay>();
            overlayWrapper.Setup(o => o.Name).Returns(name);
            overlayWrapper.Setup(o => o.OverlayDefinition).Returns(overlayDefinition);

            return overlayWrapper;
        }
    }
}
