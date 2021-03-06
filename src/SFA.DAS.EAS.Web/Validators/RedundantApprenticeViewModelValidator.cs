﻿using FluentValidation;
using SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships;

namespace SFA.DAS.EmployerCommitments.Web.Validators
{
    public sealed class RedundantApprenticeViewModelValidator : AbstractValidator<RedundantApprenticeViewModel>
    {
        public RedundantApprenticeViewModelValidator()
        {
            RuleFor(x => x.ChangeStatusViewModel.MadeRedundant).NotNull().WithMessage("Select yes if the apprentice has been made redundant");
        }
    }
}