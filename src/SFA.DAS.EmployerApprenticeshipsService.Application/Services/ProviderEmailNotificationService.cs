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
    public class ProviderEmailNotificationService : IProviderEmailNotificationService
    {
        private readonly IProviderEmailService _providerEmailService;
        private readonly ILog _logger;
        private readonly IHashingService _hashingService;

        public ProviderEmailNotificationService(IProviderEmailService providerEmailService, ILog logger, IHashingService hashingService)
        {
            _providerEmailService = providerEmailService;
            _logger = logger;
            _hashingService = hashingService;
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
                    {"URL", $"{apprenticeship.ProviderId}/apprentices/manage/{_hashingService.HashValue(apprenticeship.Id)}/details" }
                }
            };

            _logger.Info($"Sending email to all provider recipients for Provider {apprenticeship.ProviderId}, template {emailMessage.TemplateId}");

            await _providerEmailService.SendEmailToAllProviderRecipients(
                apprenticeship.ProviderId,
                string.Empty,
                emailMessage);
        }

        public async Task SendProviderApprenticeshipStopEditNotification(Apprenticeship apprenticeship, DateTime newStopDate)
        {
            //throw new System.NotImplementedException();

            await _providerEmailService.SendEmailToAllProviderRecipients(1, "", "");
        }
    }
}
