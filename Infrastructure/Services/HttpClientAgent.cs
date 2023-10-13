using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;

namespace Infrastructure.Services
{
    public class HttpClientAgent : IHttpClientAgent
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HttpClientAgent(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<HttpResponseMessage> GetAsync(HttpRequestMessage request)
        {
            var httpClient = _httpClientFactory.CreateClient("HttpWrapper");
            var response = await httpClient.GetAsync(request.RequestUri);
            return response;
        }

        public async Task<HttpResponseMessage> PostAsync(HttpRequestMessage request, StringContent? content)
        {
            var httpClient = _httpClientFactory.CreateClient("HttpWrapper");
            return await httpClient.PostAsync(request.RequestUri, content);
        }
    }
}
