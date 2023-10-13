using Application.Interface;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO.Company;

namespace MoneyLendingApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CompanyController : Controller
    {
        #region Private 
        private readonly ICompanyService _companyService;
        #endregion Private

        #region Constructor
        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
        }
        #endregion Constructor

        #region Public Methods
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(CompanyDTO request)
        {
            var response = await _companyService.RegisterCompany(request);
            if(response != null && response.Error != null)
            {
                return BadRequest(response.Error);
            }
            return Ok(response);
        }

        [HttpPut]
        [Route("UpdateStatus")]
        public async Task<IActionResult> UpdateStatus(ChangeCompanyStatusDTO updateStatusRequest)
        {
            var response = await _companyService.UpdateCompanyStatus(updateStatusRequest);

            if (response != null && response.Error != null)
            {
                return BadRequest(response.Error);
            }
            return Ok(response);
        }

        [HttpGet]
        [Route("Registrations")]
        public async Task<IActionResult> Registrations()
        {
            var response = await _companyService.GetAll();
            return Ok(response);
        }

        [HttpGet]
        [Route("Details")]
        public async Task<IActionResult> Details(long companyId)
        {
            var response = await _companyService.GetCompanyPaymentDetails(companyId);
            return Ok(response);
        }
        #endregion Public Methods
    }
}
