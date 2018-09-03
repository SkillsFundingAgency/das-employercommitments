using SFA.DAS.Notifications.Api.Types;

namespace SFA.DAS.EmployerCommitments.Domain.Interfaces
{
    public interface IBackgroundNotificationService
    {
        void SendEmail(Email email);
    }
}