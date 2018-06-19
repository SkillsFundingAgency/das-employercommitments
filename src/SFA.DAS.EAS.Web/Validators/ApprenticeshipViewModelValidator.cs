using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.AcademicYear;
using SFA.DAS.EmployerCommitments.Web.Validators.Messages;
using SFA.DAS.EmployerCommitments.Web.ViewModels;

namespace SFA.DAS.EmployerCommitments.Web.Validators
{
    public class ApprenticeshipViewModelValidator : ApprenticeshipCoreValidator, IApprenticeshipViewModelValidator
    {
        private readonly IAcademicYearValidator _academicYearValidator;
        private readonly ICurrentDateTime _currentDateTime; //todo: remove & tidy this file up again

        public ApprenticeshipViewModelValidator(
            IApprenticeshipValidationErrorText validationText, 
            IAcademicYearDateProvider academicYear, 
            IAcademicYearValidator academicYearValidator,
            ICurrentDateTime currentDateTime)
            : base(validationText, academicYear)
        {
            _academicYearValidator = academicYearValidator;
            _currentDateTime = currentDateTime;
        }

        public Dictionary<string, string> ValidateToDictionary(ApprenticeshipViewModel instance)
        {
            var result = base.Validate(instance);

            return result.Errors.ToDictionary(a => a.PropertyName, b => b.ErrorMessage);
        }

        public Dictionary<string, string> ValidateAcademicYear(ApprenticeshipViewModel model)
        {
            var dict = new Dictionary<string, string>();

            if (model.StartDate?.DateTime != null &&
                _academicYearValidator.Validate(model.StartDate.DateTime.Value) == AcademicYearValidationResult.NotWithinFundingPeriod)
            {
                dict.Add($"{nameof(model.StartDate)}", ValidationText.AcademicYearStartDate01.Text);
            }

            return dict;
        }

        protected override void ValidateUln()
        {
            When(x => !string.IsNullOrEmpty(x.ULN), () =>
            {
                base.ValidateUln();
            });
        }

        protected override void ValidateTraining()
        {
            When(x => !string.IsNullOrEmpty(x.TrainingCode), () =>
            {
                base.ValidateTraining();
            });
        }

        protected override void ValidateDateOfBirth()
        {
            When(x => HasAnyValuesSet(x.DateOfBirth), () =>
            {
                base.ValidateDateOfBirth();
            });
        }

        protected override void ValidateStartDate()
        {
            When(x => HasYearOrMonthValueSet(x.StartDate), () =>
            {
                base.ValidateStartDate();
            });
        }

        //protected override IRuleBuilderOptions<ApprenticeshipViewModel, DateTimeViewModel> ValidateEndDate()
        //{
        //    IRuleBuilderOptions<ApprenticeshipViewModel, DateTimeViewModel> result = null;
        //    When(x => HasYearOrMonthValueSet(x.EndDate), () =>
        //    {
        //        result = base.ValidateEndDate();
        //        When(IsNotApprovedByBoth, () =>
        //        {
        //            result = result.Must(m => m.DateTime > _currentDateTime.Now)
        //                .WithMessage(_validationText.LearnPlanEndDate03.Text)
        //                .WithErrorCode(_validationText.LearnPlanEndDate03.ErrorCode);
        //        });
        //    });
        //    return result;
        //}

        protected override IRuleBuilderOptions<ApprenticeshipViewModel, DateTimeViewModel> ValidateEndDate()
        {
            IRuleBuilderOptions<ApprenticeshipViewModel, DateTimeViewModel> result = null;
            When(x => HasYearOrMonthValueSet(x.EndDate), () =>
            {
                result = base.ValidateEndDate();
            });

            //if (result == null)
            //    return result;

            //result = result.Must(m => m.DateTime > _currentDateTime.Now)
            //    .WithMessage(_validationText.LearnPlanEndDate03.Text)
            //    .WithErrorCode(_validationText.LearnPlanEndDate03.ErrorCode)
            //    .When(IsNotApprovedByBoth);

            return result;
        }

        private bool IsNotApprovedByBoth(ApprenticeshipViewModel model)
        {
            //can't do this agreementstatus not correctly populated in model, would need to fetch apprenticeship!
            //options:
            //add hidden input
            return model.AgreementStatus != AgreementStatus.BothAgreed;
        }

        protected override void ValidateCost()
        {
            When(x => !string.IsNullOrEmpty(x.Cost), () =>
            {
                base.ValidateCost();
            });
        }

        private bool HasYearOrMonthValueSet(DateTimeViewModel date)
        {
            //todo: this looks suspiciously like HasAnyValuesSet() below, not year or month!
            return date != null && (date.Day.HasValue || date.Month.HasValue || date.Year.HasValue);
        }

        private bool HasAnyValuesSet(DateTimeViewModel dateOfBirth)
        {
            return dateOfBirth != null && (dateOfBirth.Day.HasValue || dateOfBirth.Month.HasValue || dateOfBirth.Year.HasValue);
        }
    }
}