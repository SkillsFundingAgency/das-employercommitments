using FluentValidation;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Web.ViewModels.ManageApprenticeships;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Validation;
using SFA.DAS.EmployerCommitments.Web.ViewModels;
using SFA.DAS.HashingService;

namespace SFA.DAS.EmployerCommitments.Web.Validators
{
    public sealed class EditApprenticeshipStopDateViewModelValidator : AbstractValidator<EditApprenticeshipStopDateViewModel>
    {
        private readonly ICurrentDateTime _currentDateTime;
        private IValidationApi _validationApi;
        private IHashingService _hashingService;

        public EditApprenticeshipStopDateViewModelValidator(ICurrentDateTime currentDateTime, IAcademicYearDateProvider academicYearDateProvider, IValidationApi validationApi, IHashingService hashingService)
        {
            _currentDateTime = currentDateTime;
            _validationApi = validationApi;
            _hashingService = hashingService;

            RuleFor(r => r.NewStopDate)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("Enter the stop date for this apprenticeship")
                .Must(d => d.DateTime.HasValue).WithMessage("Enter the stop date for this apprenticeship")
                .Must(d => d.DateTime <= new DateTime(_currentDateTime.Now.Year, _currentDateTime.Now.Month, 1)).WithMessage("The stop date cannot be in the future")
                .Must((model, newStopDate) => newStopDate.DateTime >= new DateTime(model.ApprenticeshipStartDate.Year, model.ApprenticeshipStartDate.Month, 1)).WithMessage("The stop month cannot be before the apprenticeship started")
                .Must((model, newStopDate) => newStopDate.DateTime != model.CurrentStopDate).WithMessage("Enter a date that is different to the current stopped date")
                .MustAsync(NotOverlap).WithMessage("The date overlaps with existing dates for the same apprentice.");
        }

        private async Task<bool> NotOverlap(EditApprenticeshipStopDateViewModel model, DateTimeViewModel viewModel, CancellationToken arg3)
        {
            if (model.NewStopDate.DateTime <= model.CurrentStopDate) return true;

            var apprenticeshipId = _hashingService.DecodeValue(model.ApprenticeshipHashedId);

            var overlapResult = await _validationApi.ValidateOverlapping(new ApprenticeshipOverlapValidationRequest
            {
                ApprenticeshipId = apprenticeshipId,
                StartDate = model.ApprenticeshipStartDate,
                EndDate = model.NewStopDate.DateTime.Value,
                Uln = model.ApprenticeshipULN
            });

            return overlapResult == null || !overlapResult.OverlappingApprenticeships.Any();
        }
    }
}