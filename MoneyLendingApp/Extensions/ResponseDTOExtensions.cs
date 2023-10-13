using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Shared.DTO;
using System.Net;

namespace MoneyLendingApp.Extensions
{
    public static class ResponseDTOExtensions
    {
        public static ContentResult ToHttpResponse<T>(this ResponseDTO<T> response)
        {
            var status = HttpStatusCode.OK;

            if(response.Error != null)
            {
                switch(response.Error.ErrorCode)
                {
                    case (int)HttpStatusCode.BadRequest: status = HttpStatusCode.BadRequest; 
                        break;
                    case (int)HttpStatusCode.NotFound:
                        status = HttpStatusCode.NotFound;
                        break;
                }
            }
            return new ContentResult
            {
                StatusCode = (int)status,
                ContentType = "application/json",
                Content = JsonConvert.SerializeObject(response)
            };
        }

        //public static ResponseViewModel<T>
    }
}
