using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.EmployerCommitments.Application.Commands.SendNotification;
using SFA.DAS.EmployerCommitments.Application.Exceptions;
using SFA.DAS.EmployerCommitments.Domain.Configuration;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.Notification;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Notifications.Api.Types;

namespace SFA.DAS.EmployerCommitments.Application.Commands.TransferApprovalStatus
{
    public sealed class TransferApprovalCommandHandler : AsyncRequestHandler<TransferApprovalCommand>
    {
        private readonly IEmployerCommitmentApi _commitmentsService;
        private readonly IMediator _mediator;
        private readonly ILog _logger;
        private readonly EmployerCommitmentsServiceConfiguration _configuration;
        private readonly IProviderEmailService _providerEmailService;
        private readonly IHashingService _hashingService;

        public TransferApprovalCommandHandler(
            IEmployerCommitmentApi commitmentsApi,
            IMediator mediator,
            EmployerCommitmentsServiceConfiguration configuration,
            ILog logger,
            IProviderEmailService providerEmailService,
            IHashingService hashingService)
        {
            _commitmentsService = commitmentsApi;
            _mediator = mediator;
            _configuration = configuration;
            _logger = logger;
            _providerEmailService = providerEmailService;
            _hashingService = hashingService;
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
                {"cohort_reference", commitment.Reference}, //todo: already hashed?
                {"ukprn", commitment.ProviderId.ToString()},       // only provider email needs this
                {"employer_name", commitment.LegalEntityName },    // only employer email needs these...
                {"sender_name", commitment.TransferSender.Name },
                {"employer_hashed_account", _hashingService.HashValue(commitment.EmployerAccountId) },
            };

            await SendProviderNotification(commitment, newTransferApprovalStatus, tokens);
            await SendEmployerNotification(commitment, newTransferApprovalStatus, tokens);
        }

        private async Task SendProviderNotification(CommitmentView commitment, Commitments.Api.Types.TransferApprovalStatus newTransferApprovalStatus, Dictionary<string, string> tokens)
        {
            _logger.Info($"Sending notification to provider {commitment.ProviderId} that sender has {commitment.TransferSender.TransferApprovalStatus} cohort {commitment.Id}");

            await _providerEmailService.SendEmailToAllProviderRecipients(
                commitment.ProviderId.GetValueOrDefault(),
                commitment.ProviderLastUpdateInfo?.EmailAddress ?? string.Empty,
                new EmailMessage
                {
                    TemplateId = GenerateTemplateId(newTransferApprovalStatus, RecipientType.Provider),
                    Tokens = tokens
                });
        }

        private async Task SendEmployerNotification(CommitmentView commitment, Commitments.Api.Types.TransferApprovalStatus newTransferApprovalStatus, Dictionary<string, string> tokens)
        {
            _logger.Info($"Sending email notification to {commitment.EmployerLastUpdateInfo.EmailAddress} of employer id {commitment.EmployerAccountId} that sender has {commitment.TransferSender.TransferApprovalStatus} cohort id {commitment.Id}");

            var notificationCommand = BuildNotificationCommand(commitment.EmployerLastUpdateInfo.EmailAddress, newTransferApprovalStatus, RecipientType.Employer, tokens);
            await _mediator.SendAsync(notificationCommand);
        }

        private enum RecipientType
        {
            Employer,
            Provider
        }

        private SendNotificationCommand BuildNotificationCommand(string emailAddress, Commitments.Api.Types.TransferApprovalStatus transferApprovalStatus, RecipientType recipientType, Dictionary<string, string> tokens)
        {
            // employer's url: https://localhost:44348/commitments/accounts/((employer_hashed_account))/apprentices/cohorts/transferFunded
            // provider's url: https://providers.apprenticeships.sfa.bis.gov.uk/((ukprn))/apprentices/cohorts/transferfunded

            return new SendNotificationCommand
            {
                Email = new Email
                {
                    RecipientsAddress = emailAddress,
                    TemplateId = GenerateTemplateId(transferApprovalStatus, recipientType),
                    ReplyToAddress = "noreply@sfa.gov.uk",
                    Subject = "",
                    SystemId = "x",
                    Tokens = tokens
                }
            };
        }

        private string GenerateTemplateId(Commitments.Api.Types.TransferApprovalStatus transferApprovalStatus, RecipientType recipientType)
        {
            return $"Sender{transferApprovalStatus}Commitment{recipientType}Notification";
        }
    }
}

