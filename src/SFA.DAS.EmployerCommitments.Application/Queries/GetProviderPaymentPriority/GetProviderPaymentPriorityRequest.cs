using MediatR;

namespace SFA.DAS.EmployerCommitments.Application.Queries.GetProviderPaymentPriority
{
    public class GetProviderPaymentPriorityRequest : IAsyncRequest<GetProviderPaymentPriorityResponse>
    {
        public long AccountId { get; set; }
    }
}