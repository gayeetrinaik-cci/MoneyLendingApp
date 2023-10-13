using Application.Interface;
using Microsoft.AspNetCore.Mvc;
using MoneyLendingApp.Extensions;

namespace MoneyLendingApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;
        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPut]
        [Route("MakePayment")]
        public async Task<IActionResult> MakePayment(long paymentId)
        {
            var response = await _paymentService.MakePayment(paymentId);
            return response.ToHttpResponse();
        }
    }
}
