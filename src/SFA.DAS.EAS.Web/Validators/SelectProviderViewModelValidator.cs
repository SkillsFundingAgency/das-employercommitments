using FluentValidation;
using SFA.DAS.EmployerCommitments.Web.ViewModels;

namespace SFA.DAS.EmployerCommitments.Web.Validators
{
    public class SelectProviderViewModelValidator : AbstractValidator<SelectProviderViewModel>
    {
        public SelectProviderViewModelValidator()
        {
            RuleSet("Request", () =>
            {
                RuleFor(x => x.LegalEntityCode).NotEmpty();

                RuleFor(x => x.ProviderId)
                    .NotEmpty().WithMessage("Check UK Provider Reference Number")
                    .Matches("^((?!(0))[0-9]{8})$").WithMessage("Check UK Provider Reference Number");
            });

            RuleSet("SearchResult", () =>
            {
                RuleFor(x => x.NotFound).Equal(false).WithMessage("Check UK Provider Reference Number");
            });
        }
    }
}