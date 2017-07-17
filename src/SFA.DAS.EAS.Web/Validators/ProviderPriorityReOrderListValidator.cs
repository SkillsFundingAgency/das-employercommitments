using System.Linq;
using FluentValidation;
using SFA.DAS.EmployerCommitments.Web.ViewModels;

namespace SFA.DAS.EmployerCommitments.Web.Validators
{
    public sealed class ProviderPriorityReOrderListValidator : AbstractValidator<ProviderPriorityReOrderViewModel>
    {
        public ProviderPriorityReOrderListValidator()
        {
            RuleFor(x => x.Priorities)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty()
                .Must(x =>
                {
                    var hasDuplicates = x.GroupBy(y => y).Any(g => g.Count() > 1);
                    return !hasDuplicates;
                }).WithMessage("Set payment order");
        }
    }
}