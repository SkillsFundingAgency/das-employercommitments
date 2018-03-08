﻿using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Commitment;

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
            var commitment = await _commitmentsService.GetTransferSenderCommitment(message.TransferSenderId, message.CommitmentId);

            if (commitment.TransferSenderId != message.TransferSenderId)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Commitment", "This commitment does not belong to this Transfer Sender Account" } });

            if (commitment.TransferApprovalStatus != Commitments.Api.Types.TransferApprovalStatus.Pending)
            {
                var status = commitment.TransferApprovalStatus == Commitments.Api.Types.TransferApprovalStatus.Approved
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
                TransferReceiverId = message.TransferReceiverId,
                UserEmail = message.UserEmail,
                UserName = message.UserName
            };

            await _commitmentsService.PatchTransferApprovalStatus(message.TransferSenderId, message.CommitmentId, request);
        }
    }
}

