using DefineOverlayTree.DefineOverlayPanel.GraphicalOverlayView;
using NUnit.Framework;

namespace DefineOverlayTree.UnitTests.DefineOverlayPanel.GraphicalOverlayView
{
    [TestFixture]
    public class OverlayNameTest
    {
        private OverlayName m_OverlayName;
        private const string Name = "Overlay";
        private const int Index = 3;

        [SetUp]
        public void Setup()
        {
            m_OverlayName = new OverlayName
            {
                Name = Name,
                Index = Index
            };
        }

        [Test]
        public void OverlayNameIsNotNull()
        {
            // Assert.
            Assert.IsNotNull(m_OverlayName);
        }

        [Test]
        public void PropertiesAreCreated()
        {
            // Assert.
            Assert.IsNotNull(m_OverlayName.Name);
            Assert.AreEqual(m_OverlayName.Name, Name);
            Assert.IsNotNull(m_OverlayName.Index);
            Assert.AreEqual(m_OverlayName.Index, Index);
            Assert.IsNotNull(m_OverlayName.DefaultName);
            Assert.AreEqual(m_OverlayName.DefaultName, OverlayName.OverlayDefaultNameBase + Index);
        }

        [TestCase(Name)]
        [TestCase("Overlay 1")]
        public void NameTest(string name)
        {
            m_OverlayName.Name = name;
            Assert.AreEqual(m_OverlayName.Name, name);
        }

        [TestCase(Index)]
        [TestCase(5)]
        public void IndexTest(int index)
        {
            m_OverlayName.Index = index;
            Assert.AreEqual(m_OverlayName.Index, index);
            Assert.AreEqual(m_OverlayName.DefaultName, OverlayName.OverlayDefaultNameBase + index);
        }
    }
}
