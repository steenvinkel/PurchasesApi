using Legacy.Models;
using NUnit.Framework;
using Purchases;
using Purchases.IntegrationTests;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
using AutoFixture;
using System.Text;
using System.Text.Json;

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
            _factory = new CustomWebApplicationFactory<Startup>();
            _client = _factory.CreateClient();
            _client.DefaultRequestHeaders.Add("auth_token", _factory.AuthToken);
            _fixture = new Fixture();
        }

        public static StringContent SerializeToStringContent(object obj)
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

            // Assert
            Assert.That(postResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.AreEqual(postingToSave.Amount, savedPosting.Amount);
            Assert.AreEqual(postingToSave.Date.Date, savedPosting.Date);
            Assert.AreEqual(postingToSave.Description, savedPosting.Description);
            Assert.AreNotEqual(0, savedPosting.Posting_id);

            // Act
            var result = await _client.GetAsync("/api/LegacyPosting");
            var postings = await result.Content.ReadAsAsync<List<LegacyPosting>>();

            // Assert
            Assert.AreEqual(NumSeededPostings + 1, postings.Count());
        }

        [Test]
        public async Task Save_Get_Update_Get_Posting()
        {
            // Act
            var result = await _client.GetAsync("/api/LegacyPosting");
            var postings = await result.Content.ReadAsAsync<List<LegacyPosting>>();

            // Assert
            Assert.AreEqual(NumSeededPostings, postings.Count());


            // Arrange
            var postingToSave = _fixture.Create<LegacyPosting>();
            postingToSave.Posting_id = 0;
            postingToSave.Description = _factory.SubCategoryName;

            // Act
            var postResponse = await _client.PostAsync("/api/LegacyPosting", SerializeToStringContent(postingToSave));
            var savedPosting = await postResponse.Content.ReadAsAsync<LegacyPosting>();

            // Assert
            Assert.That(postResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.AreEqual(postingToSave.Amount, savedPosting.Amount);
            Assert.AreEqual(postingToSave.Date.Date, savedPosting.Date);
            Assert.AreEqual(postingToSave.Description, savedPosting.Description);
            Assert.AreNotEqual(0, savedPosting.Posting_id);

            // Act
            result = await _client.GetAsync("/api/LegacyPosting");
            postings = await result.Content.ReadAsAsync<List<LegacyPosting>>();

            // Assert
            Assert.AreEqual(NumSeededPostings + 1, postings.Count());

            // Arrange
            var postingToUpdate = _fixture.Create<LegacyPosting>();
            postingToUpdate.Posting_id = savedPosting.Posting_id;
            postingToUpdate.Description = _factory.SubCategoryName;

            // Act
            var putResponse = await _client.PutAsync("/api/LegacyPosting", SerializeToStringContent(postingToUpdate));
            var updatedPosting = await putResponse.Content.ReadAsAsync<LegacyPosting>();

            // Assert
            Assert.That(putResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.AreEqual(postingToUpdate.Amount, updatedPosting.Amount);
            Assert.AreEqual(postingToUpdate.Date.Date, updatedPosting.Date);
            Assert.AreEqual(postingToUpdate.Description, updatedPosting.Description);
            Assert.AreEqual(postingToUpdate.Posting_id, updatedPosting.Posting_id);
        }
    }
}
