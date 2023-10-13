using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.EmailService.Interface
{
    public interface IEmailService
    {
        Task SendEmailAsync(List<string> emailId, string subject, string message);
    }
}
