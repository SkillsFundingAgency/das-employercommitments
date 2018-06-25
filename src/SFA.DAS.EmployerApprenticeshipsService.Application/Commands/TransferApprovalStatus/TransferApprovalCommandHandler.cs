using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.EmployerCommitments.Application.Commands.SendNotification;
using SFA.DAS.EmployerCommitments.Application.Exceptions;
using SFA.DAS.EmployerCommitments.Domain.Configuration;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
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
        private readonly IProviderEmailLookupService _providerEmailLookupService;
        private readonly IHashingService _hashingService;

        public TransferApprovalCommandHandler(
            IEmployerCommitmentApi commitmentsApi,
            IMediator mediator,
            EmployerCommitmentsServiceConfiguration configuration,
            ILog logger,
            IProviderEmailLookupService providerEmailLookupService,
            IHashingService hashingService)
        {
            _commitmentsService = commitmentsApi;
            _mediator = mediator;
            _configuration = configuration;
            _logger = logger;
            _providerEmailLookupService = providerEmailLookupService;
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

            await SendNotifications(commitment);
        }

        //todo: add logging to above

        private async Task SendNotifications(CommitmentView commitment)
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

            await SendProviderNotification(commitment, tokens);
            await SendEmployerNotification(commitment, tokens);
        }

        //todo:? put this code in a helper? use ProviderEmailService
        private async Task SendProviderNotification(CommitmentView commitment, Dictionary<string, string> tokens)
        {
            _logger.Info($"Sending notification to provider {commitment.ProviderId} that sender has {commitment.TransferSender.TransferApprovalStatus} cohort {commitment.Id}");
            var emails = await
                _providerEmailLookupService.GetEmailsAsync(
                    commitment.ProviderId.GetValueOrDefault(),
                    commitment.ProviderLastUpdateInfo?.EmailAddress ?? string.Empty);

            _logger.Info($"Found {emails.Count} provider email address/es");

            foreach (var email in emails)
            {
                _logger.Info($"Sending email to {email}");
                var notificationCommand = BuildNotificationCommand(email, commitment, RecipientType.Provider, tokens);
                await _mediator.SendAsync(notificationCommand);
            }
        }

        private async Task SendEmployerNotification(CommitmentView commitment, Dictionary<string, string> tokens)
        {
            _logger.Info($"Sending notification to employer {commitment.EmployerAccountId} that sender has {commitment.TransferSender.TransferApprovalStatus} cohort {commitment.Id}");

            _logger.Info($"Sending email to {commitment.EmployerLastUpdateInfo.EmailAddress}");
            var notificationCommand = BuildNotificationCommand(commitment.EmployerLastUpdateInfo.EmailAddress, commitment, RecipientType.Employer, tokens);
            await _mediator.SendAsync(notificationCommand);
        }

        private enum RecipientType
        {
            Employer,
            Provider
        }

        private SendNotificationCommand BuildNotificationCommand(string emailAddress, CommitmentView commitment, RecipientType recipientType, Dictionary<string, string> tokens)
        {
            // employer's url: https://localhost:44348/commitments/accounts/((employer_hashed_account))/apprentices/cohorts/transferFunded
            // provider's url: https://providers.apprenticeships.sfa.bis.gov.uk/((ukprn))/apprentices/cohorts/transferfunded

            return new SendNotificationCommand
            {
                Email = new Email
                {
                    RecipientsAddress = emailAddress,
                    //todo: case sensitive? right case anyway
                    TemplateId = $"Sender{commitment.TransferSender.TransferApprovalStatus}Commitment{recipientType}Notification",
                    ReplyToAddress = "noreply@sfa.gov.uk",
                    Subject = "",
                    SystemId = "x",
                    Tokens = tokens
                }
            };
        }
    }
}

