using FeatureToggle;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;

namespace SFA.DAS.EmployerCommitments.Infrastructure.Services
{
    public class FeatureToggleService : IFeatureToggleService
    {
        private readonly IBooleanToggleValueProvider _booleanToggleValueProvider;

        public FeatureToggleService(IBooleanToggleValueProvider booleanToggleValueProvider)
        {
            _booleanToggleValueProvider = booleanToggleValueProvider;
        }

        public IFeatureToggle Get<T>() where T : SimpleFeatureToggle, new()
        {
            var result = new T() as SimpleFeatureToggle;
            result.ToggleValueProvider = _booleanToggleValueProvider;
            return result;
        }
    }
}