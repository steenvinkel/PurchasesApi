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
        public void ShouldReturnZero_WhenItIsNotUser1()
        {
            var userId = _fixture.Create<int>() + 1;

            var id = UserSpecifics.GetTaxCategoryId(userId);

            Assert.AreEqual(0, id);
        }

        [Test]
        public void ShouldReturn15_WhenItIsUser1()
        {
            var userId = 1;

            var id = UserSpecifics.GetTaxCategoryId(userId);

            Assert.AreEqual(15, id);
        }
    }
}
