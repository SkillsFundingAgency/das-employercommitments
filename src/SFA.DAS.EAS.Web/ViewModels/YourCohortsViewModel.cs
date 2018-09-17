namespace SFA.DAS.EmployerCommitments.Web.ViewModels
{
    public class YourCohortsViewModel
    {
        public int DraftCount { get; set; }

        public int ReadyForReviewCount { get; set; }

        public int WithProviderCount { get; set; }

        public int? TransferFundedCohortsCount { get; set; }

        public int? RejectedTransferFundedCohortsCount { get; set; }
    }
}