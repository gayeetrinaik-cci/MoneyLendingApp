using Shared.DTO;
using System.Text.Json;

namespace MoneyLendingApp.Extensions
{
    public static class ResponseErrorExtensions
    {
        public static string ToJsonString(this ResponseError error)
        {
            return JsonSerializer.Serialize(error);
        }
    }
}
