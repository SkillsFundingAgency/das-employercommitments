﻿using System;
using System.Collections.Generic;
using System.Linq;
using MediatR;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.AcademicYear;
using SFA.DAS.EmployerCommitments.Web.Validators.Messages;
using SFA.DAS.EmployerCommitments.Web.ViewModels;
using SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships;

namespace SFA.DAS.EmployerCommitments.Web.Validators
{
    public class ApprovedApprenticeshipViewModelValidator : ApprenticeshipCoreValidator, IValidateApprovedApprenticeship
    {
        private readonly IAcademicYearValidator _academicYearValidator;

        public ApprovedApprenticeshipViewModelValidator(
            IApprenticeshipValidationErrorText validationText,
            IAcademicYearDateProvider academicYear,
            IAcademicYearValidator academicYearValidator,
            ICurrentDateTime currentDateTime,
            IMediator mediator)
            : base(validationText, academicYear, currentDateTime, mediator)
        {
            _academicYearValidator = academicYearValidator;
        }

        public Dictionary<string, string> ValidateToDictionary(ApprenticeshipViewModel instance)
        {
            var result = Validate(instance);

            return result.Errors.ToDictionary(a => a.PropertyName, b => b.ErrorMessage);
        }

        public Dictionary<string, string> ValidateAcademicYear(UpdateApprenticeshipViewModel model)
        {
            var dict = new Dictionary<string, string>();

            if (model.StartDate?.DateTime != null &&
                _academicYearValidator.Validate(model.StartDate.DateTime.Value) == AcademicYearValidationResult.NotWithinFundingPeriod)
            {
                dict.Add($"{nameof(model.StartDate)}", ValidationText.AcademicYearStartDate01.Text);
            }

            return dict;
        }
    }
}