using FluentValidation;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Web.ViewModels;
using SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships;

namespace SFA.DAS.EmployerCommitments.Web.Validators
{
    public sealed class EditApprenticeshipStopDateViewModelValidator : AbstractValidator<EditApprenticeshipStopDateViewModel>
    {
        public EditApprenticeshipStopDateViewModelValidator(ICurrentDateTime currentDateTime, IAcademicYearDateProvider academicYearDateProvider)
        {
            RuleFor(r => r.NewStopDate)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("Date is not valid")
                .Must(ValidateStopDate).WithMessage("Date is not valid")
                .Must(d => d.DateTime <= currentDateTime.Now.Date).WithMessage("New stop date must not be in future")
                .Must((model, newStopDate) => newStopDate.DateTime < model.CurrentStopDate).WithMessage("Date must be before current stop date")
                .Must((model, newStopDate) => newStopDate.DateTime >= model.ApprenticeshipStartDate).WithMessage("Date cannot be earlier than training start date")
                .Must(ValidateStopDateAgainstAcademicYear).WithMessage(
                    $"The earliest date you can stop this apprenticeship is {academicYearDateProvider.CurrentAcademicYearStartDate:dd MM yyyy}");
        }
        
        private static bool ValidateStopDate(DateTimeViewModel date)
        {
            // Check the day has value as the view model supports just month and year entry
            return date.DateTime != null && date.Day.HasValue;
        }

        private static bool ValidateStopDateAgainstAcademicYear(EditApprenticeshipStopDateViewModel model, DateTimeViewModel date)
        {
            if (!model.AcademicYearRestriction.HasValue) return true;
            return date.DateTime >= model.AcademicYearRestriction.Value;
        }
    }
}