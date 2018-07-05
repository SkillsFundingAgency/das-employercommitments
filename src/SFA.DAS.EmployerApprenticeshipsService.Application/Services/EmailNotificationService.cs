using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerCommitments.Application.Services
{
    public class EmailNotificationService
    {
        protected readonly ILog Logger;
        protected readonly IHashingService HashingService;

        public EmailNotificationService(ILog logger, IHashingService hashingService)
        {
            Logger = logger;
            HashingService = hashingService;
        }

        protected enum RecipientType
        {
            Employer,
            Provider
        }

        protected string GenerateSenderApprovedOrRejectedTemplateId(Commitments.Api.Types.TransferApprovalStatus transferApprovalStatus, RecipientType recipientType)
        {
            return $"Sender{transferApprovalStatus}Commitment{recipientType}Notification";
        }
    }
}
