using FluentValidation;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Web.ViewModels;
using SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships;

namespace SFA.DAS.EmployerCommitments.Web.Validators
{
    public sealed class EditStopDateViewModelValidator : AbstractValidator<EditStopDateViewModel>
    {
        public EditStopDateViewModelValidator(ICurrentDateTime currentDateTime)
        {
            RuleFor(r => r.NewStopDate)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("Date is not valid")
                .Must(ValidateStopDate).WithMessage("Date is not valid")
                .Must(d => d.DateTime <= currentDateTime.Now.Date).WithMessage("New stop date must not be in future");
        }
        
        private bool ValidateStopDate(DateTimeViewModel date)
        {
            // Check the day has value as the view model supports just month and year entry
            if (date.DateTime == null || !date.Day.HasValue) return false;
            return true;
        }
    }
}