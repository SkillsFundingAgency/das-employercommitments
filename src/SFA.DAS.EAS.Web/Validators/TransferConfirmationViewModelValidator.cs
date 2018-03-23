using FluentValidation;
using SFA.DAS.EmployerCommitments.Web.ViewModels;

namespace SFA.DAS.EmployerCommitments.Web.Validators
{
    public sealed class TransferConfirmationViewModelValidator : AbstractValidator<TransferConfirmationViewModel>
    {
        public TransferConfirmationViewModelValidator()
        {
            RuleFor(x => x.UrlAddress).NotNull().WithMessage("You must select an option");
        }
    }
}