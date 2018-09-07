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
        private readonly EmployerCommitmentsServiceConfiguration _configuration;

        public ProviderEmailService(IProviderEmailLookupService providerEmailLookupService,
            IBackgroundNotificationService backgroundNotificationService, ILog logger, EmployerCommitmentsServiceConfiguration configuration)
        {
            _providerEmailLookupService = providerEmailLookupService;
            _backgroundNotificationService = backgroundNotificationService;
            _logger = logger;
            _configuration = configuration;
        }
        public async Task SendEmailToAllProviderRecipients(long providerId, string lastUpdateEmailAddress, EmailMessage emailMessage)
        {
            if (!_configuration.CommitmentNotification.SendEmail)
            {
                _logger.Info("Sending email notifications disabled by config.");
                return;
            }

            var recipients = await _providerEmailLookupService.GetEmailsAsync(providerId,lastUpdateEmailAddress);

            var tasks = recipients.Select(recipient =>
            {
                _logger.Info($"Sending email to: {recipient}");
                 return _backgroundNotificationService.SendEmail(CreateEmailForRecipient(recipient, emailMessage));
            });

            await Task.WhenAll(tasks);
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
