using System.Threading.Tasks;
using SFA.DAS.Notifications.Api.Types;

namespace SFA.DAS.EmployerCommitments.Domain.Interfaces
{
    public interface IBackgroundNotificationsService
    {
        Task SendEmail(Email email);
    }
}