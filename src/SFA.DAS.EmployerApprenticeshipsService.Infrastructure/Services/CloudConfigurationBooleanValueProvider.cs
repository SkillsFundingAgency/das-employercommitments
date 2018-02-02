using System;
using FeatureToggle;
using Microsoft.Azure;
using NLog;

namespace SFA.DAS.EmployerCommitments.Infrastructure.Services
{
    public class CloudConfigurationBooleanValueProvider: IBooleanToggleValueProvider
    {
        private readonly ILogger _logger;

        public CloudConfigurationBooleanValueProvider(ILogger logger)
        {
            _logger = logger;
        }

        public bool EvaluateBooleanToggleValue(IFeatureToggle toggle)
        {
            var name = "FeatureToggle." + toggle.GetType().Name;

            var value = CloudConfigurationManager.GetSetting(name);

            if (value == null)
            {
                _logger.Log(LogLevel.Error, $"Unable to find entry for {0} in Cloud Configuration (defaulting to false)");
                return false;
            }

            if (!bool.TryParse(value, out var result))
            {
                _logger.Log(LogLevel.Error, $"Unable to find entry for {0} in Cloud Configuration (defaulting to false)");
                return false;
            }

            return result;
        }
    }
}
