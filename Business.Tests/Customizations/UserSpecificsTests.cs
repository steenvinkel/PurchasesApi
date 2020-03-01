using AutoFixture;
using Business.Customizations;
using NUnit.Framework;

namespace Business.Tests.Customizations
{
    public class UserSpecificsTests
    {
        private IFixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
        }

        [Test]
        public void ShouldReturnEmptyList_WhenItIsNotUser1()
        {
            var userId = _fixture.Create<int>() + 1;

            var ids = UserSpecifics.GetTaxSubcategoryIds(userId);

            CollectionAssert.IsEmpty(ids);
        }

        [Test]
        public void ShouldReturnThreeValues_WhenItIsUser1()
        {
            var userId = 1;

            var ids = UserSpecifics.GetTaxSubcategoryIds(userId);

            Assert.AreEqual(3, ids.Count);
        }
    }
}
