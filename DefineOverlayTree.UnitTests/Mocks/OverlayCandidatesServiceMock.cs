using System.Collections.Generic;
using System.Linq;
using DefineOverlayTree.Logic;
using Moq;

namespace DefineOverlayTree.UnitTests.Mocks
{
    public class OverlayCandidatesServiceMock : Mock<IOverlayCandidatesService>
    {
        public OverlayCandidatesServiceMock()
        {
            OverlayDefinitionMocks = new List<Mock<IOverlayDefinition>>();

            Setup(ocs => ocs.GetOverlayCandidates(It.IsAny<IOverlayDefinition>()))
                .Returns(() => OverlayDefinitionMocks.Select(odm => odm.Object).ToList());
            Setup(ocs => ocs.GetOverlayCandidates(null))
                .Returns(() => OverlayDefinitionMocks.Select(odm => odm.Object).ToList());
            Setup(ocs => ocs.GetOverlayCandidates())
                .Returns(() => OverlayDefinitionMocks.Select(odm => odm.Object).ToList());
        }

        public List<Mock<IOverlayDefinition>> OverlayDefinitionMocks { get; }

        public OverlayCandidatesServiceMock GenerateOverlayDefinitions(IList<ILayer> layers)
        {
            for (var referenceIndex = 0; referenceIndex < layers.Count - 1; ++referenceIndex)
            {
                for (var shiftedIndex = referenceIndex + 1; shiftedIndex < layers.Count; ++shiftedIndex)
                {
                    var referenceLayer = layers[referenceIndex];
                    var shiftedLayer = layers[shiftedIndex];
                    OverlayDefinitionMocks.Add(GetOverlayDefinition(referenceLayer, shiftedLayer, Direction.X));
                    OverlayDefinitionMocks.Add(GetOverlayDefinition(referenceLayer, shiftedLayer, Direction.Y));
                }
            }

            return this;
        }

        private static Mock<IOverlayDefinition> GetOverlayDefinition(ILayer referenceLayer, ILayer shiftedLayer,
            Direction direction)
        {
            var overlayDefinitionWrapper = new Mock<IOverlayDefinition>();
            overlayDefinitionWrapper.Setup(od => od.ReferenceLayer).Returns(referenceLayer);
            overlayDefinitionWrapper.Setup(od => od.ShiftedLayer).Returns(shiftedLayer);
            overlayDefinitionWrapper.Setup(od => od.Direction).Returns(direction);

            return overlayDefinitionWrapper;
        }
    }
}
