using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EmployerCommitments.Domain.Configuration;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.Notification;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Notifications.Api.Client;
using SFA.DAS.Notifications.Api.Types;

namespace SFA.DAS.EmployerCommitments.Application.Services
{
    public class ProviderEmailService : IProviderEmailService
    {
        private readonly IProviderEmailLookupService _providerEmailLookupService;
        private readonly INotificationsApi _notificationsApi;
        private readonly ILog _logger;
        private readonly EmployerCommitmentsServiceConfiguration _configuration;

        public ProviderEmailService(IProviderEmailLookupService providerEmailLookupService,
            INotificationsApi notificationsApi, ILog logger, EmployerCommitmentsServiceConfiguration configuration)
        {
            _providerEmailLookupService = providerEmailLookupService;
            _notificationsApi = notificationsApi;
            _logger = logger;
            _configuration = configuration;
        }
        public async Task SendEmailToAllProviderRecipients(long providerId, string lastUpdateEmailAddress, EmailMessage emailMessage)
        {
            if (!_configuration.CommitmentNotification.SendEmail) return;

            var recipients = await _providerEmailLookupService.GetEmailsAsync(providerId,lastUpdateEmailAddress);

            foreach (var recipient in recipients)
            {
                _logger.Info($"Sending email to: {recipient}");
                var email = CreateEmailForRecipient(recipient, emailMessage);
                await _notificationsApi.SendEmail(email);
            }
        }

        private Email CreateEmailForRecipient(string recipient, EmailMessage source)
        {
            return new Email
            {
                RecipientsAddress = recipient,
                TemplateId = source.TemplateId,
                Tokens = new Dictionary<string, string>(source.Tokens),
                ReplyToAddress = "noreply@sfa.gov.uk",
                Subject = "x",
                SystemId ="x"
            };
        }
    }
}
