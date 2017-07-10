using System.Collections.Generic;
using SFA.DAS.EmployerCommitments.Domain.Models.Payments;
using SFA.DAS.EmployerCommitments.Web.Models;

namespace SFA.DAS.EmployerCommitments.Web.ViewModels
{
    public class PaymentTransactionViewModel : TransactionLineViewModel<PaymentTransactionLine>
    {
        public string ProviderName { get; set; }

        public ICollection<ApprenticeshipPaymentGroup> CoursePaymentGroups { get; set; }
    }
}