using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Core
{
    public interface IHttpProvider
    {
        bool HasHeader(string name);
        void AddHeader(string name, string value, bool isOverried = false);
        Task<TResult> PostAsync<TResult, TRequest>(string uri, TRequest body);
        Task<TResult> GetAsync<TResult>(string uri);
    }

    public class HttpProvider : IHttpProvider
    {
        protected static readonly HttpClient _httpClient = new HttpClient();

        public HttpProvider()
        {
            if (_httpClient.BaseAddress == null)
            {
                _httpClient.BaseAddress = new Uri("https://localhost:5001/");
                _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            }
        }

        public bool HasHeader(string name)
        {
            return _httpClient.DefaultRequestHeaders.Contains(name);
        }

        public void AddHeader(string name, string value, bool isOverried = false)
        {
            if(isOverried)
            {
                if (_httpClient.DefaultRequestHeaders.Contains(name))
                    _httpClient.DefaultRequestHeaders.Remove(name);

                _httpClient.DefaultRequestHeaders.Add(name, value);
            }
            else
            {
                if (_httpClient.DefaultRequestHeaders.Contains(name))
                    return;

                _httpClient.DefaultRequestHeaders.Add(name, value);
            }
        }

        public async Task<TResult> PostAsync<TResult, TRequest>(string uri, TRequest body)
        {
            return await DoPostBodyAsync<TResult>(async () =>
            {
                var debug = JsonSerializer.Serialize(body);

                return await _httpClient.PostAsJsonAsync($"{uri}", body);
            });
        }

        public async Task<TResult> GetAsync<TResult>(string uri)
        {
            var result = await _httpClient.GetAsync(uri);

            var debug = await result.Content.ReadAsStringAsync();

            return await result.Content.ReadFromJsonAsync<TResult>();
        }

        private async Task<TResult> DoPostBodyAsync<TResult>(Func<Task<HttpResponseMessage>> postAsync)
        {
            try
            {
                var response = await postAsync();

                var debug = await response.Content.ReadAsStringAsync();

                if(response.IsSuccessStatusCode == false)
                    return default(TResult);

                return await response.Content.ReadFromJsonAsync<TResult>();
            }
            catch (NotSupportedException) // When content type is not valid
            {
                return default(TResult);
            }
            catch (JsonException ex) // Invalid JSON
            {
                return default(TResult);
            }
        }
    }
}
