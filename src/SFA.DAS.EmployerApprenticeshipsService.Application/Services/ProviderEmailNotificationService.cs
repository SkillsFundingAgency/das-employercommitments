﻿using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.Notification;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerCommitments.Application.Services
{
    public class ProviderEmailNotificationService : EmailNotificationService, IProviderEmailNotificationService
    {
        private readonly IProviderEmailService _providerEmailService;
        private readonly ILog _logger;

        public ProviderEmailNotificationService(IProviderEmailService providerEmailService, ILog logger)
        {
            _providerEmailService = providerEmailService;
            _logger = logger;
        }

        public async Task SendProviderTransferRejectedCommitmentEditNotification(CommitmentView commitment)
        {
            var emailMessage = new EmailMessage
            {
                TemplateId = "ProviderTransferRejectedCommitmentEditNotification",
                Tokens = new Dictionary<string, string>
                {
                    {"EmployerName", commitment.LegalEntityName},
                    {"CohortRef", commitment.Reference}
                }
            };

            _logger.Info($"Sending email to all provider recipients for Provider {commitment.ProviderId}, template {emailMessage.TemplateId}");

            await _providerEmailService.SendEmailToAllProviderRecipients(
                commitment.ProviderId.GetValueOrDefault(),
                commitment.ProviderLastUpdateInfo?.EmailAddress ?? string.Empty,
                emailMessage);
        }

        public async Task SendSenderApprovedOrRejectedCommitmentNotification(CommitmentView commitment, Commitments.Api.Types.TransferApprovalStatus newTransferApprovalStatus, Dictionary<string, string> tokens)
        {
            _logger.Info($"Sending notification to provider {commitment.ProviderId} that sender has {newTransferApprovalStatus} cohort {commitment.Id}");

            await _providerEmailService.SendEmailToAllProviderRecipients(
                commitment.ProviderId.GetValueOrDefault(),
                commitment.ProviderLastUpdateInfo?.EmailAddress ?? string.Empty,
                new EmailMessage
                {
                    TemplateId = GenerateSenderApprovedOrRejectedTemplateId(newTransferApprovalStatus, RecipientType.Provider),
                    Tokens = tokens
                });
        }
    }
}
