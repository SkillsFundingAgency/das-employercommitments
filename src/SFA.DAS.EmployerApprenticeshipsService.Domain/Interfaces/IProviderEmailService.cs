using System.Threading.Tasks;
using SFA.DAS.EmployerCommitments.Domain.Models.Notification;

namespace SFA.DAS.EmployerCommitments.Domain.Interfaces
{
    public interface IProviderEmailService
    {
        Task SendEmailToAllProviderRecipients(long providerId, string lastUpdateEmailAddress, EmailMessage email);
    }
}
