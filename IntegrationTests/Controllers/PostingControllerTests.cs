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
using Business.Models;
using System.Linq;

namespace IntegrationTests.Controllers
{
    public class PostingControllerTests
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
        public async Task SavePostingWithNoAmount_Fail()
        {
            // Arrange
            var postingToSave = _fixture.Create<Posting>();
            postingToSave.Amount = 0;

            // Act
            var postResponse = await _client.PostAsync("/api/Posting", SerializeToStringContent(postingToSave));

            // Assert
            Assert.That(postResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task SavePostingWithNoSubcategory_Fail()
        {
            // Arrange
            var postingToSave = _fixture.Create<Posting>();
            postingToSave.SubcategoryId = 0;

            // Act
            var postResponse = await _client.PostAsync("/api/Posting", SerializeToStringContent(postingToSave));

            // Assert
            Assert.That(postResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task Save_Get_Update_Get_Posting()
        {
            // Act
            var result = await _client.GetAsync("/api/Posting");
            var postings = await result.Content.ReadAsAsync<List<Posting>>();

            // Assert
            Assert.That(postings, Has.Count.EqualTo(NumSeededPostings));

            // Arrange
            var postingToSave = _fixture.Create<Posting>();
            postingToSave.PostingId = 0;
            postingToSave.SubcategoryId = _factory.SubCategoryId;

            // Act
            var postResponse = await _client.PostAsync("/api/Posting", SerializeToStringContent(postingToSave));
            var savedPosting = await postResponse.Content.ReadAsAsync<Posting>();

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(postResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(savedPosting.Amount, Is.EqualTo(postingToSave.Amount));
                Assert.That(savedPosting.Date, Is.EqualTo(postingToSave.Date.Date));
                Assert.That(savedPosting.SubcategoryId, Is.EqualTo(postingToSave.SubcategoryId));
                Assert.That(savedPosting.PostingId, Is.Not.EqualTo(0));
            });

            // Act
            result = await _client.GetAsync("/api/Posting");
            postings = await result.Content.ReadAsAsync<List<Posting>>();

            // Assert
            Assert.That(postings, Has.Count.EqualTo(NumSeededPostings + 1));
            Assert.That(postings, Has.One.Matches<Posting>(p => p.PostingId == savedPosting.PostingId));

            // Arrange
            var postingToUpdate = _fixture.Create<Posting>();
            postingToUpdate.PostingId = savedPosting.PostingId;
            postingToUpdate.SubcategoryId = _factory.SubCategoryId;

            // Act
            var putResponse = await _client.PutAsync("/api/Posting", SerializeToStringContent(postingToUpdate));
            var updatedPosting = await putResponse.Content.ReadAsAsync<Posting>();

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(putResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(updatedPosting.Amount, Is.EqualTo(postingToUpdate.Amount));
                Assert.That(updatedPosting.Date, Is.EqualTo(postingToUpdate.Date.Date));
                Assert.That(updatedPosting.SubcategoryId, Is.EqualTo(postingToUpdate.SubcategoryId));
                Assert.That(updatedPosting.PostingId, Is.EqualTo(postingToUpdate.PostingId));
            });
        }

        [Test]
        public async Task DeletePosting_ThenGet_Fail()
        {
            // Arrange
            var result = await _client.GetAsync("/api/Posting");
            var postings = await result.Content.ReadAsAsync<List<Posting>>();
            var postingToDelete = postings.First();

            // Act
            var deleteResponse = await _client.DeleteAsync($"/api/Posting/{postingToDelete.PostingId}");

            // Assert
            Assert.That(deleteResponse.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

            // Act
            var getResponse = await _client.GetAsync("/api/Posting");
            var updatedPostings = await getResponse.Content.ReadAsAsync<List<Posting>>();

            // Assert
            Assert.That(updatedPostings, Has.Count.EqualTo(NumSeededPostings - 1));
            Assert.That(updatedPostings, Has.None.Matches<Posting>(p => p.PostingId == postingToDelete.PostingId));
        }
    }
}
