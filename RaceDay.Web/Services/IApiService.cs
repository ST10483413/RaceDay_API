using Microsoft.AspNetCore.Http;

namespace RaceDay.Web.Services
{
    public interface IApiService
    {
        Task<HttpResponseMessage> PostAsync(string endpoint, object data, bool includeToken = false);
        Task<T?> GetAsync<T>(string endpoint, bool includeToken = false);
        Task<HttpResponseMessage> PostFileAsync(string endpoint, IFormFile file, bool includeToken = false);

        Task<HttpResponseMessage> DeleteAsync(string endpoint, bool includeToken = false);

        Task<HttpResponseMessage> PutAsync(string endpoint, object data, bool includeToken = false);
    }
}