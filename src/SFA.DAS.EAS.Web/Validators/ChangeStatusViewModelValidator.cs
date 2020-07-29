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
                .NotNull().WithMessage("Select whether to change this apprenticeship status or not")
                .IsInEnum().WithMessage("Select an option");

            RuleSet("Date", () =>
            {
                When(x => x.ChangeType == ChangeStatusType.Stop && x.ChangeConfirmed != false, () =>
                {
                    RuleFor(r => r.DateOfChange)
                               .Cascade(CascadeMode.StopOnFirstFailure)
                               .Must(d => d.DateTime.HasValue).WithMessage("Enter the stop date for this apprenticeship");

                    When(x => x.StartDate < new DateTime(_currentDateTime.Now.Year, _currentDateTime.Now.Month, 1), () =>
                    {
                        RuleFor(r => r.DateOfChange)
                                   .Cascade(CascadeMode.StopOnFirstFailure)
                                   .Must(d => d.DateTime <= new DateTime(_currentDateTime.Now.Year, _currentDateTime.Now.Month, 1)).WithMessage("The stop date cannot be in the future");
                    });
                });

            });

            RuleSet("Redundant", () =>
            {
                When(x => x.ChangeType == ChangeStatusType.Stop, () =>
                {
                    RuleFor(x => x.MadeRedundant)
                        .NotNull()
                        .WithMessage("Select yes if the apprentice has been made redundant");
                });

            });

            RuleSet("Confirm", () =>
            {
                When(x => x.ChangeType == ChangeStatusType.Stop, () =>
                {
                    RuleFor(x => x.ChangeConfirmed)
                    .NotNull().WithMessage("Select whether to stop this apprenticeship or not");
                });

                When(x => x.ChangeType == ChangeStatusType.Pause, () =>
                {
                    RuleFor(x => x.ChangeConfirmed)
                    .NotNull().WithMessage("Select whether to pause this apprenticeship or not");
                });

                When(x => x.ChangeType == ChangeStatusType.Resume, () =>
                {
                    RuleFor(x => x.ChangeConfirmed)
                        .NotNull().WithMessage("Select whether to resume this apprenticeship or not");
                });

            });

        }
    }
}