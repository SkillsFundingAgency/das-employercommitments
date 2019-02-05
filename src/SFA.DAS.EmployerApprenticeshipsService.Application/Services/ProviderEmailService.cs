using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.EmployerCommitments.Domain.Configuration;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.Notification;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Notifications.Api.Types;

namespace SFA.DAS.EmployerCommitments.Application.Services
{
    public class ProviderEmailService : IProviderEmailService
    {
        private readonly IProviderEmailLookupService _providerEmailLookupService;
        private readonly IBackgroundNotificationService _backgroundNotificationService;
        private readonly ILog _logger;
        private readonly CommitmentNotificationConfiguration _configuration;
        private readonly IdamsEmailServiceWrapper _idamsEmailServiceWrapper;

        public ProviderEmailService(IProviderEmailLookupService providerEmailLookupService,
            IBackgroundNotificationService backgroundNotificationService, ILog logger, CommitmentNotificationConfiguration configuration)
        {
            _providerEmailLookupService = providerEmailLookupService;
            _backgroundNotificationService = backgroundNotificationService;
            _logger = logger;
            _configuration = configuration;
        }
        public async Task SendEmailToAllProviderRecipients(long providerId, string lastUpdateEmailAddress, EmailMessage emailMessage)
        {
            IEnumerable<string> explicitAddresses;
            if (!_configuration.UseProviderEmail)
            {
                _logger.Info($"Using provider test email (${string.Join(", ", _configuration.ProviderTestEmails)})");
                explicitAddresses = _configuration.ProviderTestEmails;
            }

            if (!string.IsNullOrEmpty(lastUpdateEmailAddress))
            {
                _logger.Debug($"Using provider last updated email ({lastUpdateEmailAddress})");
                explicitAddresses = new List<string> { lastUpdateEmailAddress };
            }

            //pas call? will just be an internal object call when this lives in pas?
            

            var recipients = await _providerEmailLookupService.GetEmailsAsync(providerId, lastUpdateEmailAddress);
            //to be replaced by pas call
            //var tasks = recipients.Select(recipient =>
            //{
            //    _logger.Info($"Sending email to: {recipient}");
            //    return _backgroundNotificationService.SendEmail(CreateEmailForRecipient(recipient, emailMessage));
            //});

            //await Task.WhenAll(tasks);
            _logger.Info("Emails have been handed to the notification api.");
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
