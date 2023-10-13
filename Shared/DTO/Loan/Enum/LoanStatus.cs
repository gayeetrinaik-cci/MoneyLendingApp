using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTO.Loan.Enum
{
    public enum LoanStatus
    {
        NotProcessed=1,
        Approved,
        Rejected,
        ConfirmationCodeExpired,
        TermsAccepted,
        TermsRejected,
        LoanCleared,
        ApprovalNotified,
        Granted,
        PaymentScheduleGenerated
    }
}
