using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.EmployerCommitments.Application.Exceptions;
using SFA.DAS.EmployerCommitments.Domain.Configuration;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerCommitments.Application.Commands.TransferApprovalStatus
{
    public sealed class TransferApprovalCommandHandler : AsyncRequestHandler<TransferApprovalCommand>
    {
        private readonly IEmployerCommitmentApi _commitmentsService;
        private readonly ILog _logger;
        private readonly EmployerCommitmentsServiceConfiguration _configuration;
        private readonly IHashingService _hashingService;
        private readonly IProviderEmailNotificationService _providerEmailNotificationService;
        private readonly IEmployerEmailNotificationService _employerEmailNotificationService;

        public TransferApprovalCommandHandler(
            IEmployerCommitmentApi commitmentsApi,
            EmployerCommitmentsServiceConfiguration configuration,
            ILog logger,
            IHashingService hashingService,
            IProviderEmailNotificationService providerEmailNotificationService,
            IEmployerEmailNotificationService employerEmailNotificationService)
        {
            _commitmentsService = commitmentsApi;
            _configuration = configuration;
            _logger = logger;
            _hashingService = hashingService;
            _providerEmailNotificationService = providerEmailNotificationService;
            _employerEmailNotificationService = employerEmailNotificationService;
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

            await SendNotifications(commitment, message.TransferStatus);
        }

        private async Task SendNotifications(CommitmentView commitment, Commitments.Api.Types.TransferApprovalStatus newTransferApprovalStatus)
        {
            if (!_configuration.CommitmentNotification.SendEmail)
            {
                _logger.Info("Sending email notifications disabled by config.");
                return;
            }

            var tokens = new Dictionary<string, string>
            {
                {"cohort_reference", commitment.Reference},
                {"ukprn", commitment.ProviderId.ToString()},       // only provider email needs this
                {"employer_name", commitment.LegalEntityName },    // only employer email needs these...
                {"sender_name", commitment.TransferSender.Name },
                {"employer_hashed_account", _hashingService.HashValue(commitment.EmployerAccountId) },
            };

            await _providerEmailNotificationService.SendSenderApprovedOrRejectedCommitmentNotification(commitment, newTransferApprovalStatus, tokens);
            await _employerEmailNotificationService.SendSenderApprovedOrRejectedCommitmentNotification(commitment, newTransferApprovalStatus, tokens);
        }
    }
}

