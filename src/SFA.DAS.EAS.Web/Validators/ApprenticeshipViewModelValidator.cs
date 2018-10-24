using MediatR;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Web.Validators.Messages;
using SFA.DAS.EmployerCommitments.Web.ViewModels;

namespace SFA.DAS.EmployerCommitments.Web.Validators
{
    public class ApprenticeshipViewModelValidator : ApprenticeshipCoreValidator
    {
        public ApprenticeshipViewModelValidator(
            IApprenticeshipValidationErrorText validationText, 
            IAcademicYearDateProvider academicYear, 
            ICurrentDateTime currentDateTime, IMediator mediator)
            : base(validationText, academicYear, currentDateTime, mediator)
        {
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

        protected override void ValidateEndDate()
        {
            When(x => HasYearOrMonthValueSet(x.EndDate), () =>
            {
                base.ValidateEndDate();
            });
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