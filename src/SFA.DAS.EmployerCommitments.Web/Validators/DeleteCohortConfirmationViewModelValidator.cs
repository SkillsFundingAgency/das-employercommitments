using FluentValidation;
using SFA.DAS.EmployerCommitments.Web.ViewModels;

namespace SFA.DAS.EmployerCommitments.Web.Validators
{
    public sealed class DeleteCohortConfirmationViewModelValidator : AbstractValidator<DeleteCommitmentViewModel>
    {
        public DeleteCohortConfirmationViewModelValidator()
        {
            RuleFor(x => x.DeleteConfirmed).NotNull().WithMessage("Confirm deletion");
        }
    }
}