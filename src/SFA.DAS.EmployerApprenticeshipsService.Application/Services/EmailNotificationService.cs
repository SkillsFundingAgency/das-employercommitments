
namespace SFA.DAS.EmployerCommitments.Application.Services
{
    public class EmailNotificationService
    {
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
