using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Albums.Services
{
    public interface IHttpService
    {
        Task<T> Get<T>(string url);
    }

    public class HttpService : IHttpService
    {
        private readonly IHttpClientFactory _clientFactory;

        public HttpService(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<T> Get<T>(string url)
        {
            var client = _clientFactory.CreateClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var httpResponse = await client.GetAsync(url);

            httpResponse.EnsureSuccessStatusCode();

            var responseContent = await httpResponse.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(responseContent);
        }
    }
}
