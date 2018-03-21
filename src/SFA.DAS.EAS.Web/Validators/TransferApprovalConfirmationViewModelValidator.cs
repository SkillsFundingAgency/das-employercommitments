using FluentValidation;
using SFA.DAS.EmployerCommitments.Web.ViewModels;

namespace SFA.DAS.EmployerCommitments.Web.Validators
{
    public sealed class TransferApprovalConfirmationViewModelValidator : AbstractValidator<TransferApprovalConfirmationViewModel>
    {
        public TransferApprovalConfirmationViewModelValidator()
        {
            RuleFor(x => x.ApprovalConfirmed).NotNull().WithMessage("You must select an option")
                .Must(EnsureOnlyTheApproveOptionIsValid).WithMessage("You can only aprove the transfer");
        }

        private static bool EnsureOnlyTheApproveOptionIsValid(bool? x)
        {
            if (x == null)
                return true;
            return x.Value;
        }
    }
}