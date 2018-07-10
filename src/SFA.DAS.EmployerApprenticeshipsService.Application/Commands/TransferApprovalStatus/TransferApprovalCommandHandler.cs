using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.EmployerCommitments.Application.Exceptions;
using SFA.DAS.EmployerCommitments.Domain.Configuration;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerCommitments.Application.Commands.TransferApprovalStatus
{
    public sealed class TransferApprovalCommandHandler : AsyncRequestHandler<TransferApprovalCommand>
    {
        private readonly IEmployerCommitmentApi _commitmentsService;
        private readonly ILog _logger;
        private readonly EmployerCommitmentsServiceConfiguration _configuration;
        private readonly IProviderEmailNotificationService _providerEmailNotificationService;
        private readonly IEmployerEmailNotificationService _employerEmailNotificationService;

        public TransferApprovalCommandHandler(
            IEmployerCommitmentApi commitmentsApi,
            EmployerCommitmentsServiceConfiguration configuration,
            ILog logger,
            IProviderEmailNotificationService providerEmailNotificationService,
            IEmployerEmailNotificationService employerEmailNotificationService)
        {
            _commitmentsService = commitmentsApi;
            _configuration = configuration;
            _logger = logger;
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
            //todo: we should probably also check this in EmployerEmailNotificationService
            // (ProviderEmailNotificationService uses ProviderEmailService, which checks it)
            // or in defaultregistry we could supply no-op implementations for XxxEmailNotificationService when SendEmail is disabled
            if (!_configuration.CommitmentNotification.SendEmail)
            {
                _logger.Info("Sending email notifications disabled by config.");
                return;
            }

            var providerNotifyTask = _providerEmailNotificationService.SendSenderApprovedOrRejectedCommitmentNotification(commitment, newTransferApprovalStatus);
            var employerNotifyTask = _employerEmailNotificationService.SendSenderApprovedOrRejectedCommitmentNotification(commitment, newTransferApprovalStatus);

            await Task.WhenAll(providerNotifyTask, employerNotifyTask);
        }
    }
}

