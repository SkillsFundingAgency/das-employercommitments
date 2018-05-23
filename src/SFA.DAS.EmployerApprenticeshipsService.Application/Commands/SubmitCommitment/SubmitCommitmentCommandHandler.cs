using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.EmployerCommitments.Application.Commands.SendNotification;
using SFA.DAS.EmployerCommitments.Application.Exceptions;
using SFA.DAS.EmployerCommitments.Domain.Configuration;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Notifications.Api.Types;

namespace SFA.DAS.EmployerCommitments.Application.Commands.SubmitCommitment
{
    public sealed class SubmitCommitmentCommandHandler : AsyncRequestHandler<SubmitCommitmentCommand>
    {
        private readonly IEmployerCommitmentApi _commitmentApi;
        private readonly IMediator _mediator;
        private readonly EmployerCommitmentsServiceConfiguration _configuration;
        private readonly IProviderEmailLookupService _providerEmailLookupService;
        private readonly ILog _logger;
        private readonly SubmitCommitmentCommandValidator _validator;

        public SubmitCommitmentCommandHandler(
            IEmployerCommitmentApi commitmentApi,
            IMediator mediator,
            EmployerCommitmentsServiceConfiguration configuration,
            IProviderEmailLookupService providerEmailLookupService,
            ILog logger)
        {
            _commitmentApi = commitmentApi;
            _mediator = mediator;
            _configuration = configuration;
            _providerEmailLookupService = providerEmailLookupService;
            _logger = logger;

            _validator = new SubmitCommitmentCommandValidator();
        }

        protected override async Task HandleCore(SubmitCommitmentCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
                throw new InvalidRequestException(validationResult.ValidationDictionary);

            var commitment = await _commitmentApi.GetEmployerCommitment(message.EmployerAccountId, message.CommitmentId);

            if (commitment.EmployerAccountId != message.EmployerAccountId)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Commitment", "This commitment does not belong to this Employer Account " } });

            var submission = new CommitmentSubmission
            {
                Action = message.LastAction,
                LastUpdatedByInfo = new LastUpdateInfo { Name = message.UserDisplayName, EmailAddress = message.UserEmailAddress },
                UserId = message.UserId,
                Message = message.Message
            };

            if (message.LastAction != LastAction.Approve)
            {
                await _commitmentApi.PatchEmployerCommitment(message.EmployerAccountId, message.CommitmentId, submission);
            }
            else
            {
                await _commitmentApi.ApproveCohort(message.EmployerAccountId, message.CommitmentId, submission);
            }

            if (_configuration.CommitmentNotification.SendEmail
                && message.LastAction != LastAction.None)
            {
                await SendNotification(commitment, message);
            }
            _logger.Info("Submit commitment");
        }

        private async Task SendNotification(CommitmentView commitment, SubmitCommitmentCommand message)
        {
            _logger.Info($"Sending notification for commitment {commitment.Id} to providers with ukprn {commitment.ProviderId}");
            var emails = await
                _providerEmailLookupService.GetEmailsAsync(
                    commitment.ProviderId.GetValueOrDefault(),
                    commitment.ProviderLastUpdateInfo?.EmailAddress ?? string.Empty);

            _logger.Info($"Found {emails.Count} provider email address/es");

            var tokens = new Dictionary<string, string> {
                { "cohort_reference", commitment.Reference }
            };

            string templateId;
            switch (commitment.AgreementStatus)
            {
                case AgreementStatus.NotAgreed when commitment.TransferSender != null:
                    templateId = "TransferProviderCommitmentNotification";
                    tokens["receiving_employer"] = commitment.LegalEntityName;
                    break;
                case AgreementStatus.NotAgreed:
                    templateId = "ProviderCommitmentNotification";
                    tokens["type"] = message.LastAction == LastAction.Approve ? "approval" : "review";
                    break;
                case AgreementStatus.ProviderAgreed when commitment.TransferSender != null:
                    templateId = "TransferPendingFinalApproval";
                    break;
                default:
                    templateId = "ProviderCohortApproved";
                    break;
            }

            foreach (var email in emails)
            {
                _logger.Info($"Sending email to {email}");
                await _mediator.SendAsync(BuildNotificationCommand(email, templateId, tokens));
            }
        }

        private SendNotificationCommand BuildNotificationCommand(string email, string templateId, Dictionary<string, string> tokens)
        {
            return new SendNotificationCommand
            {
                Email = new Email
                {
                    RecipientsAddress = email,
                    TemplateId = templateId,
                    ReplyToAddress = "noreply@sfa.gov.uk",
                    Subject = "x",
                    SystemId = "x",
                    Tokens = tokens
                }
            };
        }
    }
}
