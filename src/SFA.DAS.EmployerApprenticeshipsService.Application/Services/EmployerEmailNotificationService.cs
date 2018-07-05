using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.EmployerCommitments.Application.Commands.SendNotification;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Notifications.Api.Types;

namespace SFA.DAS.EmployerCommitments.Application.Services
{
    public class EmployerEmailNotificationService : EmailNotificationService, IEmployerEmailNotificationService
    {
        private readonly IMediator _mediator;

        public EmployerEmailNotificationService(IMediator mediator, ILog logger, IHashingService hashingService)
            : base(logger, hashingService)
        {
            _mediator = mediator;
        }

        public async Task SendSenderApprovedOrRejectedCommitmentNotification(CommitmentView commitment, Commitments.Api.Types.TransferApprovalStatus newTransferApprovalStatus)
        {
            var email = commitment.EmployerLastUpdateInfo?.EmailAddress;
            if (string.IsNullOrWhiteSpace(email))
            {
                Logger.Info($"No email associated with employer, skipping notification of employer id {commitment.EmployerAccountId} that sender has {newTransferApprovalStatus} cohort id {commitment.Id}");
                return;
            }

            Logger.Info($"Sending email notification to {email} of employer id {commitment.EmployerAccountId} that sender has {newTransferApprovalStatus} cohort id {commitment.Id}");

            var tokens = new Dictionary<string, string>
            {
                {"cohort_reference", commitment.Reference},
                {"employer_name", commitment.LegalEntityName },
                {"sender_name", commitment.TransferSender.Name },
                {"employer_hashed_account", HashingService.HashValue(commitment.EmployerAccountId) }
            };

            var notificationCommand = BuildNotificationCommand(email, newTransferApprovalStatus, RecipientType.Employer, tokens);
            await _mediator.SendAsync(notificationCommand);
        }

        private SendNotificationCommand BuildNotificationCommand(string emailAddress, Commitments.Api.Types.TransferApprovalStatus transferApprovalStatus, RecipientType recipientType, Dictionary<string, string> tokens)
        {
            return new SendNotificationCommand
            {
                Email = new Email
                {
                    RecipientsAddress = emailAddress,
                    TemplateId = GenerateSenderApprovedOrRejectedTemplateId(transferApprovalStatus, recipientType),
                    ReplyToAddress = "noreply@sfa.gov.uk",
                    Subject = "",
                    SystemId = "x",
                    Tokens = tokens
                }
            };
        }
    }
}
