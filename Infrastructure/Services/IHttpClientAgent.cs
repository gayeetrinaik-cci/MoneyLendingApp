using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public interface IHttpClientAgent
    {
        Task<HttpResponseMessage> PostAsync(HttpRequestMessage request, StringContent? content);
        Task<HttpResponseMessage> GetAsync(HttpRequestMessage request);

    }
}
