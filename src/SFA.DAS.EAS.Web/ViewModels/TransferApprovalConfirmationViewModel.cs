using FluentValidation.Attributes;
using SFA.DAS.EmployerCommitments.Web.Validators;

namespace SFA.DAS.EmployerCommitments.Web.ViewModels
{
    [Validator(typeof(TransferApprovalConfirmationViewModelValidator))]
    public sealed class TransferApprovalConfirmationViewModel
    {
        public string HashedTransferSenderAccountId { get; set; }
        public string HashedCommitmentId { get; set; }
        public bool? ApprovalConfirmed { get; set; }
    }
}