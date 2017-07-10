using FluentValidation;
using SFA.DAS.EmployerCommitments.Web.ViewModels;

namespace SFA.DAS.EmployerCommitments.Web.Validators
{
    public class DeleteApprenticeshipConfirmationViewModelValidator : AbstractValidator<DeleteApprenticeshipConfirmationViewModel>
    {
        public DeleteApprenticeshipConfirmationViewModelValidator()
        {
            RuleFor(x => x.DeleteConfirmed).NotNull().WithMessage("Please choose an option");
        }
    }
}