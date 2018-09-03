using System.Web.Hosting;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.Notifications.Api.Client;
using SFA.DAS.Notifications.Api.Types;

namespace SFA.DAS.EmployerCommitments.Infrastructure.Services
{
    public class BackgroundNotificationService : IBackgroundNotificationService
    {
        private readonly INotificationsApi _notificationsApi;

        public BackgroundNotificationService(INotificationsApi notificationsApi)
        {
            _notificationsApi = notificationsApi;
        }

        public void SendEmail(Email email)
        {
            HostingEnvironment.QueueBackgroundWorkItem(async cancellationToken =>
            {
                await _notificationsApi.SendEmail(email);
            });
        }
    }
}