using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.EmployerCommitments.Domain.Configuration;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.Notification;
using SFA.DAS.EmployerCommitments.Infrastructure.Services;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Notifications.Api.Types;
using SFA.DAS.PAS.Account.Api.Types;

namespace SFA.DAS.EmployerCommitments.Application.Services
{
    public class ProviderEmailService : IProviderEmailService
    {
        private readonly ILog _logger;
        private readonly IProviderNotifyService _providerNotifyService;

        public ProviderEmailService(ILog logger, IProviderNotifyService providerNotifyService)
        {
            _logger = logger;
            _providerNotifyService = providerNotifyService;
        }
        public async Task SendEmailToAllProviderRecipients(long providerId, string lastUpdateEmailAddress, EmailMessage emailMessage)
        {
            IEnumerable<string> explicitAddresses = null;

            if (!string.IsNullOrEmpty(lastUpdateEmailAddress))
            {
                _logger.Debug($"Using provider last updated email ({lastUpdateEmailAddress})");
                explicitAddresses = new List<string> { lastUpdateEmailAddress };
            }

            await _providerNotifyService.SendProviderEmailNotifications(providerId, new ProviderEmailRequest
            {
                ExplicitEmailAddresses = explicitAddresses?.ToList(),
                TemplateId = emailMessage.TemplateId,
                Tokens = emailMessage.Tokens
            });

            _logger.Info("Emails have been handed to the provider account api.");
        }
    }
}
