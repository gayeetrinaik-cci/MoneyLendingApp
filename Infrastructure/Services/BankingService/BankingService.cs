using Newtonsoft.Json;
using Shared.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Services.BankingService
{
    public class BankingService : IBankingService
    {
        private readonly IHttpClientAgent _httpClientAgent;
        public BankingService(IHttpClientAgent httpClientAgent)
        {
            _httpClientAgent = httpClientAgent;
        }
        public async Task<decimal> GetBankBalance(string bankName, string accountNumber)
        {
            var httpResponseMessage = await _httpClientAgent.GetAsync(new HttpRequestMessage(HttpMethod.Get, $"Account/account/balance?bankName={bankName}&accountNumber={accountNumber}"));
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();

                return await System.Text.Json.JsonSerializer.DeserializeAsync<decimal>(contentStream);
            }
            throw new Exception("Error with banking api");
        }

        public async Task<bool> MakePayment(PaymentRequest paymentRequest)
        {
            var httpContent = new StringContent(JsonConvert.SerializeObject(paymentRequest), Encoding.UTF8, "application/json");
            var httpResponseMessage = await _httpClientAgent.PostAsync(new HttpRequestMessage(HttpMethod.Post, $"Account/account/transaction"), httpContent);
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                return true;
            }
            throw new Exception("Error with banking api");
        }
    }
}
