using FluentValidation;
using SFA.DAS.EmployerCommitments.Web.ViewModels.Organisation;

namespace SFA.DAS.EmployerCommitments.Web.Validators
{
    public sealed class OrganisationDetailsViewModelValidator : AbstractValidator<OrganisationDetailsViewModel>
    {
        public OrganisationDetailsViewModelValidator()
        {
            RuleFor(r => r.Name).NotEmpty().WithMessage("Enter a name");
        }
    }
}