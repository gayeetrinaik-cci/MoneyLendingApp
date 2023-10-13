using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.EmailService
{
    public class EmailOptions
    {
        public string SMTP {  get; set; }
        public int Port {  get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public string Sender {  get; set; }
    }
}
