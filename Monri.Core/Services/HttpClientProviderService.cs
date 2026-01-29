using Monri.Core.Models;
using System.Net.Http.Json;

namespace Monri.Core.Services
{
    public interface IHttpClientProviderService
    {
        Task<Result<TResult>> PostRequest<TResult, TRequest>(string endpoint, TRequest request);
        Task<Result<TResult>> GetRequest<TResult>(string endpoint);

    }
    public class HttpClientProviderService(IHttpClientFactory clientFactory) : IHttpClientProviderService
    {
        private readonly IHttpClientFactory _clientFactory = clientFactory;
        private HttpClient? _httpClient;

        public async Task<Result<TResult>> PostRequest<TResult, TRequest>(string endpoint, TRequest request)
        {
            _httpClient = await GetHttpClient();
            var response = await _httpClient.PostAsJsonAsync(endpoint, request);
            return await ResponseHandler<TResult>(response);
        }

        public async Task<Result<TResult>> GetRequest<TResult>(string endpoint)
        {
            _httpClient = await GetHttpClient();
            var response = await _httpClient.GetAsync(endpoint);
            return await ResponseHandler<TResult>(response);
        }

        #region PRIVATE
        private async Task<Result<TResult>> ResponseHandler<TResult>(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                if (content.Length > 0)
                {
                    var result = await response.Content.ReadFromJsonAsync<TResult>();
                    return Result.Success(result);
                }
                else
                {
                    return default(TResult);
                }
            }
            else
            {
                Error error = new((int)response.StatusCode, response.ReasonPhrase ?? Error.Fallback.Message);
                return Result.Failure<TResult>(error);
            }
        }

        private async Task<HttpClient> GetHttpClient()
        {            
            _httpClient ??= _clientFactory.CreateClient();
            return _httpClient;
        }
        #endregion
    }
}
