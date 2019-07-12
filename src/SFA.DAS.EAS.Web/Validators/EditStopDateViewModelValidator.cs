using FluentValidation;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships;
using System;

namespace SFA.DAS.EmployerCommitments.Web.Validators
{
    public sealed class EditApprenticeshipStopDateViewModelValidator : AbstractValidator<EditApprenticeshipStopDateViewModel>
    {
        private readonly ICurrentDateTime _currentDateTime;

        public EditApprenticeshipStopDateViewModelValidator(ICurrentDateTime currentDateTime, IAcademicYearDateProvider academicYearDateProvider)
        {
            _currentDateTime = currentDateTime;

            RuleFor(r => r.NewStopDate)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("Enter the stop date for this apprenticeship")
                .Must(d => d.DateTime.HasValue).WithMessage("Enter the stop date for this apprenticeship")
                .Must(d => d.DateTime <= new DateTime(_currentDateTime.Now.Year, _currentDateTime.Now.Month, 1)).WithMessage("The stop date cannot be in the future")
                .Must((model, newStopDate) => newStopDate.DateTime >= new DateTime(model.ApprenticeshipStartDate.Year, model.ApprenticeshipStartDate.Month, 1)).WithMessage("The stop month cannot be before the apprenticeship started");
        }
    }
}