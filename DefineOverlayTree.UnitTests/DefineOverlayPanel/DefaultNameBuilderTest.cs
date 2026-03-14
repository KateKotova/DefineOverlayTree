using System.Collections.Generic;
using DefineOverlayTree.DefineOverlayPanel;
using NUnit.Framework;

namespace DefineOverlayTree.UnitTests.DefineOverlayPanel
{
    [TestFixture]
    public class DefaultNameBuilderTest
    {
        private const string DefaultNameBase = "Layer ";
        private DefaultNameBuilder m_DefaultNameBuilder;

        [SetUp]
        public void Setup()
        {
            m_DefaultNameBuilder = new DefaultNameBuilder(DefaultNameBase);
        }

        [Test]
        public void DefaultNameBuilderIsNotNull()
        {
            // Assert.
            Assert.IsNotNull(m_DefaultNameBuilder);
        }

        [Test]
        public void PropertiesAreCreated()
        {
            // Assert.
            Assert.AreEqual(m_DefaultNameBuilder.DefaultNameBase, DefaultNameBase);
            Assert.AreEqual(m_DefaultNameBuilder.FirstDefaultName, DefaultNameBase + 1.ToString());
        }

        [Test]
        public void GetFirstDefaultNameIsExistingNamesAreEmptyOrDesNotContainDefaultNames()
        {
            // Arrange.
            var emptyNames = new List<string>();
            var namesWithoutDefaultNames = new List<string> { "1", "q", "laye", "l a y e r", string.Empty, null };
            // Act.
            var nextDefaultNameAfterNullNames = m_DefaultNameBuilder.GetNextDefaultName(null);
            var nextDefaultNameAfterEmptyNames = m_DefaultNameBuilder.GetNextDefaultName(emptyNames);
            var nextDefaultNameAfterNamesWithoutDefaultNames
                = m_DefaultNameBuilder.GetNextDefaultName(namesWithoutDefaultNames);
            // Assert.
            Assert.AreEqual(nextDefaultNameAfterNullNames, m_DefaultNameBuilder.FirstDefaultName);
            Assert.AreEqual(nextDefaultNameAfterEmptyNames, m_DefaultNameBuilder.FirstDefaultName);
            Assert.AreEqual(nextDefaultNameAfterNamesWithoutDefaultNames, m_DefaultNameBuilder.FirstDefaultName);
        }

        [Test]
        public void SpacesAfterDefaultNameBaseAtTheBegginingAreIgnored()
        {
            // Arrange.
            var names = new List<string> { "Layer", "Layer  2", "Layer   3", "Layer   4" };
            // Act.
            var nextDefaultName = m_DefaultNameBuilder.GetNextDefaultName(names);
            // Assert.
            Assert.AreEqual(nextDefaultName, DefaultNameBase + 5.ToString());
        }

        [Test]
        public void CaseOfDefaultNameBaseAtTheBegginingIsIgnored()
        {
            // Arrange.
            var names = new List<string> { "layer 1", "LAYER  2", "LaYeR   3", "Layer    4" };
            // Act.
            var nextDefaultName = m_DefaultNameBuilder.GetNextDefaultName(names);
            // Assert.
            Assert.AreEqual(nextDefaultName, DefaultNameBase + 5.ToString());
        }

        [Test]
        [TestCase("LAYER")]
        [TestCase("LAYER qwe")]
        [TestCase("LAYER 1qwe")]
        [TestCase("LAYER 1 qwe")]
        [TestCase("LAYER -2")]
        public void SetsOneAsMaximumIndexForDefaultNamesWithIncorrectIndices(string layerName)
        {
            // Arrange.
            var names = new List<string> { layerName };
            // Act.
            var nextDefaultName = m_DefaultNameBuilder.GetNextDefaultName(names);
            // Assert.
            Assert.AreEqual(nextDefaultName, m_DefaultNameBuilder.FirstDefaultName);
        }

        [Test]
        [TestCase("q", "LAYER", "Layer 1")]
        [TestCase("LAYER 0", "q", "Layer 1")]
        [TestCase("Layer 10", "Layer 1", "Layer 11")]
        [TestCase("Layer +1", "q", "Layer 2")]
        [TestCase("Layer +1E-0", "q", "Layer 1")]
        [TestCase("LAYER -3", "LAYER 1", "Layer 2")]
        public void FindsMaximumNextIndex(string layerName1, string layerName2, string result)
        {
            // Arrange.
            var names = new List<string> { layerName1, layerName2 };
            // Act.
            var nextDefaultName = m_DefaultNameBuilder.GetNextDefaultName(names);
            // Assert.
            Assert.AreEqual(nextDefaultName, result);
        }
    }
}
