using System.Threading.Tasks;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.Notifications.Api.Client;
using SFA.DAS.Notifications.Api.Types;

namespace SFA.DAS.EmployerCommitments.Application.Services
{
    public class ProviderEmailService : IProviderEmailService
    {
        private IProviderEmailLookupService _providerEmailLookupService;
        private readonly INotificationsApi _notificationsApi;

        public ProviderEmailService(IProviderEmailLookupService providerEmailLookupService, INotificationsApi notificationsApi)
        {
            _providerEmailLookupService = providerEmailLookupService;
            _notificationsApi = notificationsApi;
        }
        public Task SendEmailToAllProviderRecipients(long providerId, string lastUpdateEmailAddress, Email email)
        {

        //    _logger.Info($"Sending notification for commitment {commitment.Id} to providers with ukprn {commitment.ProviderId}");
        //    var emails = await
        //        _providerEmailLookupService.GetEmailsAsync(
        //            commitment.ProviderId.GetValueOrDefault(),
        //            commitment.ProviderLastUpdateInfo?.EmailAddress ?? string.Empty);

        //    _logger.Info($"{emails.Count} provider found email address/es");
        //    if (!_configuration.CommitmentNotification.SendEmail) return;

        //    foreach (var email in emails)
        //    {
        //        _logger.Info($"Sending email to {email}");
        //        var notificationCommand = BuildNotificationCommand(email, commitment);
        //        await _mediator.SendAsync(notificationCommand);
        //    }


            throw new System.NotImplementedException();
        }
    }
}
