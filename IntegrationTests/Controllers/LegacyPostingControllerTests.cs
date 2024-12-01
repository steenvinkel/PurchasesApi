using Legacy.Models;
using NUnit.Framework;
using Purchases;
using Purchases.IntegrationTests;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
using AutoFixture;
using System.Text;
using System.Text.Json;
using System;

namespace IntegrationTests.Controllers
{
    public class LegacyPostingControllerTests
    {
        private CustomWebApplicationFactory<Startup> _factory;
        private HttpClient _client;
        private Fixture _fixture;
        private const int NumSeededPostings = 2;

        [SetUp]
        public void Setup()
        {
            Environment.SetEnvironmentVariable("sql_connection", "FakeConnection");
            _factory = new CustomWebApplicationFactory<Startup>();
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
        public async Task SavePostingWithoutSubCategory_ListNewPostingOnGet()
        {
            // Arrange
            var postingToSave = _fixture.Create<LegacyPosting>();
            postingToSave.Posting_id = 0;
            postingToSave.Description = _factory.SubCategoryName;

            // Act
            var postResponse = await _client.PostAsync("/api/LegacyPosting", SerializeToStringContent(postingToSave));
            var savedPosting = await postResponse.Content.ReadAsAsync<LegacyPosting>();

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(postResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(savedPosting.Amount, Is.EqualTo(postingToSave.Amount));
                Assert.That(savedPosting.Date, Is.EqualTo(postingToSave.Date.Date));
                Assert.That(savedPosting.Description, Is.EqualTo(postingToSave.Description));
                Assert.That(savedPosting.Posting_id, Is.Not.EqualTo(0));
            });

            // Act
            var result = await _client.GetAsync("/api/LegacyPosting");
            var postings = await result.Content.ReadAsAsync<List<LegacyPosting>>();

            // Assert
            Assert.That(postings, Has.Count.EqualTo(NumSeededPostings + 1));
        }

        [Test]
        public async Task Save_Get_Update_Get_Posting()
        {
            // Act
            var result = await _client.GetAsync("/api/LegacyPosting");
            var postings = await result.Content.ReadAsAsync<List<LegacyPosting>>();

            // Assert
            Assert.That(postings, Has.Count.EqualTo(NumSeededPostings));

            // Arrange
            var postingToSave = _fixture.Create<LegacyPosting>();
            postingToSave.Posting_id = 0;
            postingToSave.Description = _factory.SubCategoryName;

            // Act
            var postResponse = await _client.PostAsync("/api/LegacyPosting", SerializeToStringContent(postingToSave));
            var savedPosting = await postResponse.Content.ReadAsAsync<LegacyPosting>();

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(postResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(savedPosting.Amount, Is.EqualTo(postingToSave.Amount));
                Assert.That(savedPosting.Date, Is.EqualTo(postingToSave.Date.Date));
                Assert.That(savedPosting.Description, Is.EqualTo(postingToSave.Description));
                Assert.That(savedPosting.Posting_id, Is.Not.EqualTo(0));
            });

            // Act
            result = await _client.GetAsync("/api/LegacyPosting");
            postings = await result.Content.ReadAsAsync<List<LegacyPosting>>();

            // Assert
            Assert.That(postings, Has.Count.EqualTo(NumSeededPostings + 1));

            // Arrange
            var postingToUpdate = _fixture.Create<LegacyPosting>();
            postingToUpdate.Posting_id = savedPosting.Posting_id;
            postingToUpdate.Description = _factory.SubCategoryName;

            // Act
            var putResponse = await _client.PutAsync("/api/LegacyPosting", SerializeToStringContent(postingToUpdate));
            var updatedPosting = await putResponse.Content.ReadAsAsync<LegacyPosting>();

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(putResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(updatedPosting.Amount, Is.EqualTo(postingToUpdate.Amount));
                Assert.That(updatedPosting.Date, Is.EqualTo(postingToUpdate.Date.Date));
                Assert.That(updatedPosting.Description, Is.EqualTo(postingToUpdate.Description));
                Assert.That(updatedPosting.Posting_id, Is.EqualTo(postingToUpdate.Posting_id));
            });
        }
    }
}
