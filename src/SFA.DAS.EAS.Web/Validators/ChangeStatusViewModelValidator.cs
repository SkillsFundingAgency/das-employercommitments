using System;
using FluentValidation;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships;

namespace SFA.DAS.EmployerCommitments.Web.Validators
{
    public sealed class ChangeStatusViewModelValidator : AbstractValidator<ChangeStatusViewModel>
    {
        private readonly ICurrentDateTime _currentDateTime;

        public ChangeStatusViewModelValidator(ICurrentDateTime currentDateTime)
        {
            _currentDateTime = currentDateTime;
            RuleFor(x => x.ChangeType)
                .NotNull().WithMessage("Select an option")
                .IsInEnum().WithMessage("Select an option");

            RuleSet("Date", () => 
            {
                When(x => x.ChangeType == ChangeStatusType.Stop, () =>
                {
                    RuleFor(r => r.DateOfChange)
                               .Cascade(CascadeMode.StopOnFirstFailure)
                               .Must(d => d.DateTime.HasValue).WithMessage("Enter the stop date for this apprenticeship")
                               .Must(d => d.DateTime <= new DateTime(_currentDateTime.Now.Year, _currentDateTime.Now.Month, 1)).WithMessage("The stop date cannot be in the future");
                });
            });

            RuleSet("Confirm", () =>
            {
                RuleFor(x => x.ChangeConfirmed)
                    .NotNull().WithMessage("Select an option");
            });

        }
    }
}