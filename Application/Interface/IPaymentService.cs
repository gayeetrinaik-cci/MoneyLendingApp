using Infrastructure.Services;
using Shared.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface
{
    public interface IPaymentService
    {
        Task GeneratePayment();

        Task<ResponseDTO<bool>> MakePayment(long paymentId);
    }
}
