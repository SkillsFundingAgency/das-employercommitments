using System.Collections.Generic;
using SFA.DAS.Commitments.Api.Types.ProviderPayment;

namespace SFA.DAS.EmployerCommitments.Application.Queries.GetProviderPaymentPriority
{
    public class GetProviderPaymentPriorityResponse
    {
        public IList<ProviderPaymentPriorityItem> Data { get; set; }
    }
}