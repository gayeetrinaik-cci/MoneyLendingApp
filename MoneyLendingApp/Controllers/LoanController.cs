using Application.Interface;
using Microsoft.AspNetCore.Mvc;
using MoneyLendingApp.Extensions;
using Shared.DTO.Loan;

namespace MoneyLendingApp.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class LoanController : Controller
    {
        #region Private fields
        private readonly ILoanService _loanService;
        #endregion Private fiels

        #region Constructor
        public LoanController(ILoanService loanService)
        {
            _loanService = loanService;
        }
        #endregion Constructor

        [HttpPost(Name = "Apply")]
        public async Task<IActionResult> Apply(LoanDTO loan)
        {
            var response = await _loanService.LoanApplication(loan);
            return response.ToHttpResponse();
        }

        [HttpPut(Name ="UpdateStatus")]
        public async Task<IActionResult> UpdateStatus(LoanStatusUpdateRequestDTO loanStatusUpdateRequest)
        {
            var response = await _loanService.LoanStatusUpdate(loanStatusUpdateRequest);
            return response.ToHttpResponse();
        }

        [HttpPut(Name ="Confirm")]
        public async Task<IActionResult> Confirm([FromQuery] string confirmationCode)
        {
            var response = await _loanService.ConfirmWithLoanToken(confirmationCode);
            return response.ToHttpResponse();
        }
    }
}
