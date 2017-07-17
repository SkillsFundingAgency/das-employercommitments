using FluentValidation;
using SFA.DAS.EmployerCommitments.Web.ViewModels;

namespace SFA.DAS.EmployerCommitments.Web.Validators
{
    public sealed class FinishEditingViewModelValidator : AbstractValidator<FinishEditingViewModel>
    {
        public FinishEditingViewModelValidator()
        {
            RuleFor(x => x.SaveStatus).IsInEnum().WithMessage("Select an option");
        }
    }
}