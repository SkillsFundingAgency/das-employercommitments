using FeatureToggle;
using FluentValidation;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.FeatureToggles;
using SFA.DAS.EmployerCommitments.Web.ViewModels;

namespace SFA.DAS.EmployerCommitments.Web.Validators
{
    public sealed class TransferApprovalConfirmationViewModelValidator : AbstractValidator<TransferApprovalConfirmationViewModel>
    {
        private readonly IFeatureToggleService _featureToggleService;

        public TransferApprovalConfirmationViewModelValidator(IFeatureToggleService featureToggleService)
        {
            _featureToggleService = featureToggleService;

            RuleFor(x => x.ApprovalConfirmed).NotNull().WithMessage("You must select an option")
                .Must(EnsureOnlyTheApproveOptionIsValid).WithMessage("You can only approve the transfer");
        }

        private bool EnsureOnlyTheApproveOptionIsValid(bool? x)
        {
            var rejection = _featureToggleService.Get<TransfersRejectOption>();
            if (rejection.FeatureEnabled) return true;

            if (x == null)
                return true;
            return x.Value;
        }
    }
}