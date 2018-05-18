using FluentValidation.Attributes;
using SFA.DAS.EmployerCommitments.Web.Validators;

namespace SFA.DAS.EmployerCommitments.Web.ViewModels
{
    [Validator(typeof(TransferApprovalConfirmationViewModelValidator))]
    public sealed class TransferApprovalConfirmationViewModel
    {
        public string TransferReceiverName { get; set; }
        public bool? ApprovalConfirmed { get; set; }
        public string HashedCohortReference { get; set; }

    }
}