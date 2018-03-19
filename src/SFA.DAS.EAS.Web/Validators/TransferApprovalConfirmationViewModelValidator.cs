using FluentValidation;
using SFA.DAS.EmployerCommitments.Web.ViewModels;

namespace SFA.DAS.EmployerCommitments.Web.Validators
{
    public sealed class TransferApprovalConfirmationViewModelValidator : AbstractValidator<TransferApprovalConfirmationViewModel>
    {
        public TransferApprovalConfirmationViewModelValidator()
        {
            RuleFor(x => x.ApprovalConfirmed).NotNull().WithMessage("You must either approve the transfer or reject the transfer");
        }
    }
}