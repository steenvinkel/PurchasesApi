using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace IntegrationTests
{
    public static class HttpContentExtensionMethods
    {
        public async static Task<T> ReadAsAsync<T>(this HttpContent content)
        {
            return await JsonSerializer.DeserializeAsync<T>(await content.ReadAsStreamAsync(), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
    }
}
