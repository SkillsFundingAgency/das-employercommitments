using System.Collections.Generic;
using FluentValidation.Attributes;
using SFA.DAS.EmployerCommitments.Web.Validators;

namespace SFA.DAS.EmployerCommitments.Web.ViewModels
{
    [Validator(typeof(ProviderPriorityReOrderListValidator))]
    public sealed class ProviderPriorityReOrderViewModel
    {
        public ProviderPriorityReOrderViewModel()
        {
            Priorities = new List<long>();
        }

        public IList<long> Priorities { get; set; }
    }
}