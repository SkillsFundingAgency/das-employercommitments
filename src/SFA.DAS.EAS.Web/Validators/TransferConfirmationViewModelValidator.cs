using FluentValidation;
using SFA.DAS.EmployerCommitments.Web.ViewModels;

namespace SFA.DAS.EmployerCommitments.Web.Validators
{
    public sealed class TransferConfirmationViewModelValidator : AbstractValidator<TransferConfirmationViewModel>
    {
        public TransferConfirmationViewModelValidator()
        {
            RuleFor(x => x.SelectedOption).NotNull().WithMessage("You must select an option");
        }
    }
}