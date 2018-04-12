using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.EmployerCommitments.Application.Exceptions;

namespace SFA.DAS.EmployerCommitments.Application.Commands.TransferApprovalStatus
{
    public sealed class TransferApprovalCommandHandler : AsyncRequestHandler<TransferApprovalCommand>
    {
        private readonly IEmployerCommitmentApi _commitmentsService;

        public TransferApprovalCommandHandler(IEmployerCommitmentApi commitmentsApi)
        {
            _commitmentsService = commitmentsApi;
        }

        protected override async Task HandleCore(TransferApprovalCommand message)
        {
            var commitment =
                await _commitmentsService.GetTransferSenderCommitment(message.TransferSenderId, message.CommitmentId);

            if (commitment.TransferSender.Id != message.TransferSenderId)
                throw new InvalidRequestException(new Dictionary<string, string>
                {
                    {"Commitment", "This commitment does not belong to this Transfer Sender Account"}
                });

            if (commitment.TransferSender.TransferApprovalStatus !=
                Commitments.Api.Types.TransferApprovalStatus.Pending)
            {
                var status = commitment.TransferSender.TransferApprovalStatus ==
                             Commitments.Api.Types.TransferApprovalStatus.Approved
                    ? "approved"
                    : "rejected";

                throw new InvalidRequestException(new Dictionary<string, string>
                {
                    {"Commitment", $"This transfer request has already been {status}"}
                });
            }

            var request = new TransferApprovalRequest
            {
                TransferApprovalStatus = message.TransferStatus,
                TransferReceiverId = commitment.EmployerAccountId,
                UserEmail = message.UserEmail,
                UserName = message.UserName
            };

            if (message.TransferRequestId > 0)
            {
                await _commitmentsService.PatchTransferApprovalStatus(message.TransferSenderId, message.CommitmentId, message.TransferRequestId, request);
            }
            else
            {
                await _commitmentsService.PatchTransferApprovalStatus(message.TransferSenderId, message.CommitmentId, request);
            }
        }
    }
}

