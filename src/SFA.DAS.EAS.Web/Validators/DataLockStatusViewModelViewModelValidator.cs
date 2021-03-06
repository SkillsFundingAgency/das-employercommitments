using FluentValidation;
using SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships;

namespace SFA.DAS.EmployerCommitments.Web.Validators
{
    public sealed class DataLockStatusViewModelViewModelValidator : AbstractValidator<DataLockStatusViewModel>
    {
        public DataLockStatusViewModelViewModelValidator()
        {
            RuleFor(x => x.ChangesConfirmed).NotNull().WithMessage("Select an option");
        }
    }
}