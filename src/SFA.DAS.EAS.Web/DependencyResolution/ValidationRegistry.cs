using FluentValidation;
using SFA.DAS.EmployerCommitments.Web.Validators;
using SFA.DAS.EmployerCommitments.Web.ViewModels;
using StructureMap;

namespace SFA.DAS.EmployerCommitments.Web.DependencyResolution
{
    public class ValidationRegistry : Registry
    {
        public ValidationRegistry()
        {
            For<IValidator<ApprenticeshipViewModel>>().Use<ApprenticeshipViewModelValidator>();
        }
    }
}