using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Purchases.IntegrationTests;
using NUnit.Framework;
using System.Net;
using System.Dynamic;

namespace Purchases.IntegrationTests
{
    public class LegacyGraphControllerTests
    {
        private CustomWebApplicationFactory<Startup> _factory;
        private HttpClient _client;

        [SetUp]
        public void Setup()
        {
            _factory = new CustomWebApplicationFactory<Startup>();
            _client = _factory.CreateClient();
            _client.DefaultRequestHeaders.Add("auth_token", _factory.AuthToken);
        }

        [Test]
        [Ignore("This might not be needed with the full test is available")]
        public async Task Sumup_ShouldReturnOk()
        {
            var result = await _client.GetAsync("/api/LegacyGraph/Sumup");
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        [Ignore("This might not be needed with the full test is available")]
        public async Task Sumup_ShouldEmptyList()
        {
            var result = await _client.GetAsync("/api/LegacyGraph/Sumup");
            var content = await result.Content.ReadAsAsync<List<ExpandoObject>>();
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            CollectionAssert.IsNotEmpty(content);
            var element = (IDictionary<String, object>)content.First();
            Assert.IsTrue(element.ContainsKey("in"));
            Assert.AreEqual(10000, element["in"]);
        }

        [TearDown]
        public void TearDown()
        {
            _client.Dispose();
            _factory.Dispose();
        }
    }
}
