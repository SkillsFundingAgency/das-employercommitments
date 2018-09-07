using System;
using System.Threading.Tasks;
using System.Web.Hosting;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Notifications.Api.Client;
using SFA.DAS.Notifications.Api.Types;

namespace SFA.DAS.EmployerCommitments.Infrastructure.Services
{
    public class BackgroundNotificationService : IBackgroundNotificationService
    {
        private readonly ILog _logger;
        private readonly INotificationsApi _notificationsApi;

        public BackgroundNotificationService(ILog logger, INotificationsApi notificationsApi)
        {
            _logger = logger;
            _notificationsApi = notificationsApi;
        }

        public Task SendEmail(Email email)
        {
            _logger.Debug($"Sending email to [{email.RecipientsAddress}] in a background task.");
            HostingEnvironment.QueueBackgroundWorkItem(async cancellationToken =>
            {
                try
                {
                    await _notificationsApi.SendEmail(email);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, $"Error using the Notification Api when trying to send email {email.RecipientsAddress}.");
                }
            });
            return Task.CompletedTask;
        }
    }
}