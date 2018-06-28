using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.EmployerCommitments.Application.Commands.SendNotification;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Notifications.Api.Types;

namespace SFA.DAS.EmployerCommitments.Application.Services
{
    public class EmployerEmailNotificationService : EmailNotificationService, IEmployerEmailNotificationService
    {
        private readonly IMediator _mediator;
        private readonly ILog _logger;

        public EmployerEmailNotificationService(IMediator mediator, ILog logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task SendSenderApprovedOrRejectedCommitmentNotification(CommitmentView commitment, Commitments.Api.Types.TransferApprovalStatus newTransferApprovalStatus, Dictionary<string, string> tokens)
        {
            _logger.Info($"Sending email notification to {commitment.EmployerLastUpdateInfo.EmailAddress} of employer id {commitment.EmployerAccountId} that sender has {newTransferApprovalStatus} cohort id {commitment.Id}");

            var notificationCommand = BuildNotificationCommand(commitment.EmployerLastUpdateInfo.EmailAddress, newTransferApprovalStatus, RecipientType.Employer, tokens);
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
