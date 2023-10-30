using NUnit.Framework;

namespace Legacy.Tests
{
    public class StringExtensionMethodsTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase("AccumulatedCategoryId", "accumulated_category_id")]
        [TestCase("Account", "account")]
        [TestCase("accumulated_category_id", "accumulated_category_id")]
        public void StringShouldBeConverted(string input, string expected)
        {
            var result = input.ToUnderscoreCase();

            Assert.AreEqual(expected, result);
        }
    }
}