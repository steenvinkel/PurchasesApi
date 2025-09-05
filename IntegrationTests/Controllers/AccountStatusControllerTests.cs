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
    public class AccountStatusControllerTests
    {
        private CustomWebApplicationFactory _factory;
        private HttpClient _client;
        private Fixture _fixture;

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
        public async Task Save_Get_Update_Get_Posting()
        {
            var userResponse = await _client.GetAsync("/api/User");
            var user = await userResponse.Content.ReadAsAsync<User>();
            var account = user.Accounts.First();

            // Assert
            Assert.That(account.Statuses, Has.Count.EqualTo(2));

            // Arrange
            var accountStatusToSave = new AccountStatus(0, account.Statuses.Last().Date.AddMonths(1), _fixture.Create<decimal>());

            // Act
            var postResponse = await _client.PostAsync($"/api/Account/{account.AccountId}/AccountStatus", SerializeToStringContent(accountStatusToSave));

            // Assert
            Assert.That(postResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            userResponse = await _client.GetAsync("/api/User");
            user = await userResponse.Content.ReadAsAsync<User>();
            account = user.Accounts.First();
            var savedAccountStatus = account.Statuses.Last();

            Assert.Multiple(() =>
            {
                Assert.That(savedAccountStatus.Amount, Is.EqualTo(accountStatusToSave.Amount));
                Assert.That(savedAccountStatus.Date, Is.EqualTo(accountStatusToSave.Date.Date));
                Assert.That(account.Statuses, Has.Count.EqualTo(3));
            });

            // Arrange
            var statusToUpdate = new AccountStatus(savedAccountStatus.AccountStatusId, savedAccountStatus.Date.AddDays(1), savedAccountStatus.Amount + 100);

            // Act
            var putResponse = await _client.PutAsync($"/api/Account/{account.AccountId}/AccountStatus", SerializeToStringContent(statusToUpdate));

            // Assert
            Assert.That(putResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            userResponse = await _client.GetAsync("/api/User");
            user = await userResponse.Content.ReadAsAsync<User>();
            account = user.Accounts.First();
            var updatedAccountStatus = account.Statuses.Last();

            Assert.Multiple(() =>
            {
                Assert.That(savedAccountStatus.Amount, Is.EqualTo(accountStatusToSave.Amount));
                Assert.That(savedAccountStatus.Date, Is.EqualTo(accountStatusToSave.Date.Date));
                Assert.That(account.Statuses, Has.Count.EqualTo(3));
            });
        }
    }
}
