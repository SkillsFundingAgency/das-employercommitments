﻿using System;
using FluentValidation;
using SFA.DAS.EmployerCommitments.Web.ViewModels;
using SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships;

namespace SFA.DAS.EmployerCommitments.Web.Validators
{
    public sealed class ChangeStatusViewModelValidator : AbstractValidator<ChangeStatusViewModel>
    {
        public ChangeStatusViewModelValidator()
        {
            RuleFor(x => x.ChangeType)
                .NotNull().WithMessage("Select an option")
                .IsInEnum().WithMessage("Select an option");

            RuleSet("Date", () => 
            {
                When(x => x.ChangeType == ChangeStatusType.Stop, () =>
                {
                    RuleFor(x => x.WhenToMakeChange)
                        .NotNull().WithMessage("Select an option")
                        .IsInEnum().WithMessage("Select an option");

                    When(x => x.WhenToMakeChange == WhenToMakeChangeOptions.SpecificDate, () =>
                    {
                        RuleFor(r => r.DateOfChange)
                                .Cascade(CascadeMode.StopOnFirstFailure)
                                .NotNull().WithMessage("Date is not valid")
                                .Must(ValidateDateOfBirth).WithMessage("Date is not valid")
                                .Must(d => d.DateTime < DateTime.UtcNow.Date.AddDays(1)).WithMessage("Date must be a date in the past");
                    });
                });
            });

            RuleSet("Confirm", () =>
            {
                RuleFor(x => x.ChangeConfirmed)
                    .NotNull().WithMessage("Select an option");
            });

        }

        private bool ValidateDateOfBirth(DateTimeViewModel date)
        {
            // Check the day has value as the view model supports just month and year entry
            if (date.DateTime == null || !date.Day.HasValue) return false;

            return true;
        }
    }
}