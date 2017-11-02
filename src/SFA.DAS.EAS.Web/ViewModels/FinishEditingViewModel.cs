using FluentValidation.Attributes;
using SFA.DAS.EmployerCommitments.Web.Enums;
using SFA.DAS.EmployerCommitments.Web.Validators;

namespace SFA.DAS.EmployerCommitments.Web.ViewModels
{

    [Validator(typeof(FinishEditingViewModelValidator))]
    public sealed class FinishEditingViewModel
    {
        public string HashedAccountId { get; set; }

        public string HashedCommitmentId { get; set; }

        public bool HasApprenticeships { get; set; }

        public int InvalidApprenticeshipCount { get; set; }

        public SaveStatus SaveStatus { get; set; }

        public ApprovalState ApprovalState { get; set; }

        public bool NotReadyForApproval { get;  set; }

        public bool IsApproveAndSend => ApprovalState == ApprovalState.ApproveAndSend;

        public bool HasSignedTheAgreement { get; set; }

        public bool HasOverlappingErrors { get; set; }

        public bool CanApprove
        {
            get
            {
                return !NotReadyForApproval &&
                    HasSignedTheAgreement &&
                    !HasOverlappingErrors;
            }

        }
    }

    public enum ApprovalState
    {
        ApproveAndSend = 0,
        ApproveOnly = 1
    }
}