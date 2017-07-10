using FluentValidation;
using SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships;

namespace SFA.DAS.EmployerCommitments.Web.Validators
{
    public sealed class UpdateApprenticeshipViewModelValidator : AbstractValidator<UpdateApprenticeshipViewModel>
    {
        public UpdateApprenticeshipViewModelValidator()
        {
            RuleFor(x => x.ChangesConfirmed).NotNull().WithMessage("Select an option");
        }
    }
}