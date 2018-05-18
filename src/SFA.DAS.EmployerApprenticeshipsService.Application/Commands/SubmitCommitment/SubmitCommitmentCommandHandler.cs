﻿using System.Collections.Generic;
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
using Task = System.Threading.Tasks.Task;

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
                throw new InvalidRequestException(new Dictionary<string, string> { { "Commitment", "This commiment does not belong to this Employer Account " } });

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

            if (message.LastAction != LastAction.None)
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

            _logger.Info($"{emails.Count} provider found email address/es");

            if (!_configuration.CommitmentNotification.SendEmail) return;

            foreach (var email in emails)
            {
                _logger.Info($"Sending email to {email}");
                var notificationCommand = BuildNotificationCommand(
                    email,
                    commitment,
                    message.LastAction, message.UserDisplayName);
                await _mediator.SendAsync(notificationCommand);
            }
        }

        private SendNotificationCommand BuildNotificationCommand(string email, CommitmentView commitment, LastAction action, string userDisplayName)
        {
            var template = commitment.TransferSender != null
                ? "ProviderTransferCommitmentNotification"
                    : commitment.AgreementStatus == AgreementStatus.NotAgreed
                        ? "ProviderCommitmentNotification"
                        : "ProviderCohortApproved";

            return new SendNotificationCommand
            {
                Email = new Email
                {
                    RecipientsAddress = email,
                    TemplateId = template,
                    ReplyToAddress = "noreply@sfa.gov.uk",
                    Subject = "x",
                    SystemId = "x",
                    Tokens = new Dictionary<string, string> {
                        { "type", action == LastAction.Approve ? "approval" : "review" },
                        { "cohort_reference", commitment.Reference },
                        { "employer_name",  commitment.LegalEntityName}
                    }
                }
            };
        }
    }
}
