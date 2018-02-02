using FeatureToggle;
using Microsoft.Azure;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerCommitments.Infrastructure.Services
{
    public class CloudConfigurationBooleanValueProvider: IBooleanToggleValueProvider
    {
        private const string TogglePrefix = "FeatureToggle";
        private readonly ILog _logger;

        public CloudConfigurationBooleanValueProvider(ILog logger)
        {
            _logger = logger;
        }

        public bool EvaluateBooleanToggleValue(IFeatureToggle toggle)
        {
            var toggleName = $"{TogglePrefix}.{toggle.GetType().Name}";

            var value = CloudConfigurationManager.GetSetting(toggleName);

            if (value == null)
            {
                _logger.Warn($"Unable to find entry for {toggleName} in Cloud Configuration (defaulting to false)");
                return false;
            }

            if (!bool.TryParse(value, out var result))
            {
                _logger.Warn($"Unable to parse entry for {toggleName} in Cloud Configuration (defaulting to false)");
                return false;
            }

            return result;
        }
    }
}
