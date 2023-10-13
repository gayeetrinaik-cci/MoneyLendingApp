using Application.Interface;
using Hangfire;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace MoneyLendingApp.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class BackgroundJobsController : Controller
    {
        private readonly ICompanyService _companyService;
        private readonly ILoanService _loanService;
        private readonly IPaymentScheduleService _paymentScheduleService;
        private readonly IPaymentService _paymentService;

        public BackgroundJobsController(ICompanyService companyService, ILoanService loanService, IPaymentScheduleService paymentScheduleService, IPaymentService paymentService)
        {
            _companyService = companyService;
            _loanService = loanService;
            _paymentScheduleService = paymentScheduleService;
            _paymentService = paymentService;
        }

        [HttpPost]
        public void NotifyCompanyRegistration()
        {
            RecurringJob.AddOrUpdate(() => _companyService.NotifyCompanyRegistration(), Cron.Minutely);
        }

        [HttpPost]
        public void NotifyLoanApprovalToCompany()
        {
            RecurringJob.AddOrUpdate(() => _loanService.NotifyCompanyOnLoanTerms(), Cron.Minutely);
        }

        [HttpPost]
        public void GeneratePaymentScheduleJob()
        {
            RecurringJob.AddOrUpdate(() => _paymentScheduleService.GeneratePaymentSchedule(), Cron.Minutely);
        }

        [HttpPost]
        public void GenaretePayment()
        {
            RecurringJob.AddOrUpdate(() => _paymentService.GeneratePayment(), Cron.Monthly);
        }
    }
}
