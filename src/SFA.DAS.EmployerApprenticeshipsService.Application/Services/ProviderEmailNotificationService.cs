using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.Notification;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerCommitments.Application.Services
{
    public class ProviderEmailNotificationService : EmailNotificationService, IProviderEmailNotificationService
    {
        private readonly IProviderEmailService _providerEmailService;

        public ProviderEmailNotificationService(IProviderEmailService providerEmailService, ILog logger, IHashingService hashingService)
            :base(logger, hashingService)
        {
            _providerEmailService = providerEmailService;
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

            Logger.Info($"Sending email to all provider recipients for Provider {commitment.ProviderId}, template {emailMessage.TemplateId}");

            await _providerEmailService.SendEmailToAllProviderRecipients(
                commitment.ProviderId.GetValueOrDefault(),
                commitment.ProviderLastUpdateInfo?.EmailAddress ?? string.Empty,
                emailMessage);
        }

        public async Task SendProviderApprenticeshipStopNotification(Apprenticeship apprenticeship)
        {
            var emailMessage = new EmailMessage
            {
                TemplateId = "ProviderApprenticeshipStopNotification",
                Tokens = new Dictionary<string, string>
                {
                    {"EMPLOYER", apprenticeship.LegalEntityName},
                    {"APPRENTICE", apprenticeship.ApprenticeshipName },
                    {"DATE", apprenticeship.StopDate.Value.ToString("dd/MM/yyyy") },
                    {"URL", $"{apprenticeship.ProviderId}/apprentices/manage/{HashingService.HashValue(apprenticeship.Id)}/details" }
                }
            };

            Logger.Info($"Sending email to all provider recipients for Provider {apprenticeship.ProviderId}, template {emailMessage.TemplateId}");

            await _providerEmailService.SendEmailToAllProviderRecipients(
                apprenticeship.ProviderId,
                string.Empty,
                emailMessage);
        }

        public async Task SendProviderApprenticeshipStopEditNotification(Apprenticeship apprenticeship, DateTime newStopDate)
        {
            var emailMessage = new EmailMessage
            {
                TemplateId = "ProviderApprenticeshipStopEditNotification",
                Tokens = new Dictionary<string, string>
                {
                    {"EMPLOYER", apprenticeship.LegalEntityName},
                    {"APPRENTICE", apprenticeship.ApprenticeshipName },
                    {"OLDDATE", apprenticeship.StopDate.Value.ToString("dd/MM/yyyy") },
                    {"NEWDATE", newStopDate.ToString("dd/MM/yyyy") },
                    {"URL", $"{apprenticeship.ProviderId}/apprentices/manage/{HashingService.HashValue(apprenticeship.Id)}/details" }
                }
            };

            Logger.Info($"Sending email to all provider recipients for Provider {apprenticeship.ProviderId}, template {emailMessage.TemplateId}");

            await _providerEmailService.SendEmailToAllProviderRecipients(
                apprenticeship.ProviderId,
                string.Empty,
                emailMessage);
        }
        public async Task SendSenderApprovedOrRejectedCommitmentNotification(CommitmentView commitment, Commitments.Api.Types.TransferApprovalStatus newTransferApprovalStatus)
        {
            Logger.Info($"Sending notification to provider {commitment.ProviderId} that sender has {newTransferApprovalStatus} cohort {commitment.Id}");

            var tokens = new Dictionary<string, string>
            {
                {"cohort_reference", commitment.Reference},
                {"ukprn", commitment.ProviderId.ToString()}
            };

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
