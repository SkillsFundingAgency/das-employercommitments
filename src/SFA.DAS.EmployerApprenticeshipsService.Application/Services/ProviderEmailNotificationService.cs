﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.EmployerCommitments.Domain.Configuration;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.Notification;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerCommitments.Application.Services
{
    public class ProviderEmailNotificationService : EmailNotificationService, IProviderEmailNotificationService
    {
        private readonly IProviderEmailService _providerEmailService;
        private readonly EmployerCommitmentsServiceConfiguration _configuration;

        public ProviderEmailNotificationService(IProviderEmailService providerEmailService, ILog logger, IHashingService hashingService, EmployerCommitmentsServiceConfiguration configuration)
            :base(logger, hashingService)
        {
            _providerEmailService = providerEmailService;
            _configuration = configuration;
        }

        public async Task SendProviderTransferRejectedCommitmentEditNotification(CommitmentView commitment)
        {
            if (!_configuration.CommitmentNotification.SendEmail)
            {
                Logger.Info("Sending email notifications disabled by config.");
                return;
            }

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

        public async Task SendCreateCommitmentNotification(CommitmentView commitment)
        {
            if (!_configuration.CommitmentNotification.SendEmail)
            {
                Logger.Info("Sending email notifications disabled by config.");
                return;
            }

            var emailMessage = new EmailMessage
            {
                TemplateId = "CreateCommitmentNotification",
                Tokens = new Dictionary<string, string> {
                    { "cohort_reference", commitment.Reference },
                    { "employer_name", commitment.LegalEntityName },
                    { "ukprn", commitment.ProviderId.ToString() }
                }
            };

            Logger.Info($"Sending email to all provider recipients for Provider {commitment.ProviderId}, template {emailMessage.TemplateId}");

            await _providerEmailService.SendEmailToAllProviderRecipients(
                commitment.ProviderId.GetValueOrDefault(),
                commitment.ProviderLastUpdateInfo?.EmailAddress ?? string.Empty,
                emailMessage);
        }

        public async Task SendProviderApprenticeshipStopNotification(Apprenticeship apprenticeship, DateTime stopDate)
        {
            if (!_configuration.CommitmentNotification.SendEmail)
            {
                Logger.Info("Sending email notifications disabled by config.");
                return;
            }

            var emailMessage = new EmailMessage
            {
                TemplateId = "ProviderApprenticeshipStopNotification",
                Tokens = new Dictionary<string, string>
                {
                    {"EMPLOYER", apprenticeship.LegalEntityName},
                    {"APPRENTICE", apprenticeship.ApprenticeshipName },
                    {"DATE", stopDate.ToString("dd/MM/yyyy") },
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
            if (!_configuration.CommitmentNotification.SendEmail)
            {
                Logger.Info("Sending email notifications disabled by config.");
                return;
            }

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
            if (!_configuration.CommitmentNotification.SendEmail)
            {
                Logger.Info("Sending email notifications disabled by config.");
                return;
            }

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

        public async Task SendProviderApprenticeshipPauseNotification(Apprenticeship apprenticeship, DateTime pausedDate)
        {
            if (!_configuration.CommitmentNotification.SendEmail)
            {
                Logger.Info("Sending email notifications disabled by config.");
                return;
            }

            var emailMessage = new EmailMessage
            {
                TemplateId = "ProviderApprenticeshipPauseNotification",
                Tokens = new Dictionary<string, string>
                {
                    {"EMPLOYER", apprenticeship.LegalEntityName},
                    {"APPRENTICE", apprenticeship.ApprenticeshipName },
                    {"DATE", pausedDate.ToString("dd/MM/yyyy") },
                    {"URL", $"{apprenticeship.ProviderId}/apprentices/manage/{HashingService.HashValue(apprenticeship.Id)}/details" }
                }
            };

            Logger.Info($"Sending email to all provider recipients for Provider {apprenticeship.ProviderId}, template {emailMessage.TemplateId}");

            await _providerEmailService.SendEmailToAllProviderRecipients(
                apprenticeship.ProviderId,
                string.Empty,
                emailMessage);
        }

        public async Task SendProviderApprenticeshipResumeNotification(Apprenticeship apprenticeship, DateTime resumeDate)
        {
            if (!_configuration.CommitmentNotification.SendEmail)
            {
                Logger.Info("Sending email notifications disabled by config.");
                return;
            }

            var emailMessage = new EmailMessage
            {
                TemplateId = "ProviderApprenticeshipResumeNotification",
                Tokens = new Dictionary<string, string>
                {
                    {"EMPLOYER", apprenticeship.LegalEntityName},
                    {"APPRENTICE", apprenticeship.ApprenticeshipName },
                    {"DATE", resumeDate.ToString("dd/MM/yyyy") },
                    {"URL", $"{apprenticeship.ProviderId}/apprentices/manage/{HashingService.HashValue(apprenticeship.Id)}/details" }
                }
            };

            Logger.Info($"Sending email to all provider recipients for Provider {apprenticeship.ProviderId}, template {emailMessage.TemplateId}");

            await _providerEmailService.SendEmailToAllProviderRecipients(
                apprenticeship.ProviderId,
                string.Empty,
                emailMessage);
        }
    }
}
