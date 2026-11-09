using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace RaceDay.Web.Services
{
    public class ApiService : IApiService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiService(IHttpClientFactory clientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _clientFactory = clientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        private HttpClient GetClient(bool includeToken)
        {
            var client = _clientFactory.CreateClient("RaceDayApi");

            if (includeToken)
            {
                var token = _httpContextAccessor.HttpContext?.Session.GetString("JwtToken");
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
            }
            return client;
        }

        public async Task<HttpResponseMessage> PostAsync(string endpoint, object data, bool includeToken = false)
        {
            var client = GetClient(includeToken);
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            return await client.PostAsync(endpoint, content);
        }

        public async Task<T?> GetAsync<T>(string endpoint, bool includeToken = false)
        {
            var client = GetClient(includeToken);
            var response = await client.GetAsync(endpoint);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            return default;
        }

        public async Task<HttpResponseMessage> PostFileAsync(string endpoint, IFormFile file, bool includeToken = false)
        {
            var client = GetClient(includeToken);

            using var content = new MultipartFormDataContent();
            var fileContent = new StreamContent(file.OpenReadStream());
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

            content.Add(fileContent, "file", file.FileName);

            return await client.PostAsync(endpoint, content);
        }

        //  ADDED THE DELETE METHOD HERE
        public async Task<HttpResponseMessage> DeleteAsync(string endpoint, bool includeToken = false)
        {
            var client = GetClient(includeToken);
            return await client.DeleteAsync(endpoint);
        }
        public async Task<HttpResponseMessage> PutAsync(string endpoint, object data, bool includeToken = false)
        {
            var client = GetClient(includeToken);
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            return await client.PutAsync(endpoint, content);
        }
    }
}