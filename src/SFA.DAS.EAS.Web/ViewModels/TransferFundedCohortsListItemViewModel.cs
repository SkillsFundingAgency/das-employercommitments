using SFA.DAS.Commitments.Api.Types;

namespace SFA.DAS.EmployerCommitments.Web.ViewModels
{
    public enum ShowLink
    {
        Details,
        Edit
    }

    public sealed class TransferFundedCohortsListItemViewModel
    {
        public string HashedCommitmentId { get; set; }
        public string SendingEmployer { get; set; }
        public string ProviderName { get; set; }
        public TransferApprovalStatus TransferApprovalStatus { get; set; }
        public ShowLink ShowLink { get; set; }
    }
}