using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EmployerCommitments.Domain.Models.Payments;

namespace SFA.DAS.EmployerCommitments.Domain.Interfaces
{
    public interface IPaymentService
    {
        Task<ICollection<PaymentDetails>> GetAccountPayments(string periodEnd, long employerAccountId);
    }
}
