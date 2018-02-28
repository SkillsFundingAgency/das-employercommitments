using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Commitment;

namespace SFA.DAS.EmployerCommitments.Application.Queries.GetCommitment
{
    public class GetCommitmentQueryHandler : IAsyncRequestHandler<GetCommitmentQueryRequest, GetCommitmentQueryResponse>
    {
        private readonly IEmployerCommitmentApi _commitmentsApi;

        public GetCommitmentQueryHandler(IEmployerCommitmentApi commitmentsApi)
        {
            _commitmentsApi = commitmentsApi;
        }

        public async Task<GetCommitmentQueryResponse> Handle(GetCommitmentQueryRequest message)
        {
            CommitmentView commitment = null;

            switch (message.CallType)
            {
                case CallType.Employer:
                    commitment = await _commitmentsApi.GetEmployerCommitment(message.AccountId, message.CommitmentId);
                    break;
                case CallType.TransferSender:
                    commitment = await _commitmentsApi.GetTransferSenderCommitment(message.AccountId, message.CommitmentId);
                    break;
            }

            return new GetCommitmentQueryResponse
            {
                Commitment = commitment
            };
        }
    }
}