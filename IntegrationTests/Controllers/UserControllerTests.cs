using NUnit.Framework;
using Purchases.IntegrationTests;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
using AutoFixture;
using System.Text;
using System.Text.Json;
using System;
using Business.Models;
using System.Linq;

namespace IntegrationTests.Controllers
{
    public class UserControllerTests
    {
        private CustomWebApplicationFactory _factory;
        private HttpClient _client;
        private Fixture _fixture;
        private const int NumSeededPostings = 2;

        [SetUp]
        public void Setup()
        {
            Environment.SetEnvironmentVariable("sql_connection", "FakeConnection");
            _factory = new CustomWebApplicationFactory();
            _client = _factory.CreateClient();
            _client.DefaultRequestHeaders.Add("auth_token", _factory.AuthToken);
            _fixture = new Fixture();
        }

        [TearDown]
        public void TearDown()
        {
            _client.Dispose();
            _factory.Dispose();
        }

        private static StringContent SerializeToStringContent(object obj)
        {
            return new StringContent(JsonSerializer.Serialize(obj), Encoding.UTF8, "application/json");
        }

        [Test]
        public async Task GetUser_ReturnsUser()
        {
            // Act
            var userResponse = await _client.GetAsync("/api/User");
            var user = await userResponse.Content.ReadAsAsync<User>();

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(userResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                // Assert User
                Assert.That(user.UserId, Is.Not.Zero);
                Assert.That(user.Username, Is.Not.Empty);
                // Assert Category
                Assert.That(user.Categories, Is.Not.Empty);
                var category = user.Categories.First();
                Assert.That(category, Is.TypeOf<IncomeCategory>());
                Assert.That(category.Subcategories, Is.Not.Empty);
                var expenseSubCategories = user.Categories.Last().Subcategories;
                Assert.That(expenseSubCategories.First(), Is.TypeOf<VariableExpenseSubCategory>());
                // Asset Account
                Assert.That(user.Accounts, Is.Not.Empty);
                var account = user.Accounts.First();
                Assert.That(account.AccumulatedCategoryName, Is.Not.Empty);
                Assert.That(account.Statuses, Is.Not.Empty);
            });

        }
    }
}
