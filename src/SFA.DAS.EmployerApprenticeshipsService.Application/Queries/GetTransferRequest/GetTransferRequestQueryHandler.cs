using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Provider.Events.Api.Client;

namespace SFA.DAS.EmployerCommitments.Application.Queries.GetTransferRequest
{
    public class GetTransferRequestQueryHandler : IAsyncRequestHandler<GetTransferRequestQueryRequest, GetTransferRequestQueryResponse>
    {
        private readonly IEmployerCommitmentApi _commitmentsApi;

        public GetTransferRequestQueryHandler(IEmployerCommitmentApi commitmentsApi)
        {
            _commitmentsApi = commitmentsApi;
        }

        public async Task<GetTransferRequestQueryResponse> Handle(GetTransferRequestQueryRequest message)
        {
            TransferRequest transferRequest = null;

            switch (message.CallerType)
            {
                case CallerType.TransferReceiver:
                    transferRequest = await _commitmentsApi.GetTransferRequestForReceiver(message.AccountId, message.TransferRequestId);
                    break;
                case CallerType.TransferSender:
                    transferRequest = await _commitmentsApi.GetTransferRequestForSender(message.AccountId, message.TransferRequestId);
                    break;
                default:
                    throw new BadRequestException("Only Getting a Transfer Request as a Sender is supported", null);
            }

            return new GetTransferRequestQueryResponse
            {
                TransferRequest = transferRequest
            };
        }
    }
}