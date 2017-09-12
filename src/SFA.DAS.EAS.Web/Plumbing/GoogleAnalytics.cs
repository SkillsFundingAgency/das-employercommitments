using Microsoft.Azure;
using SFA.DAS.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Configuration.FileStorage;
using SFA.DAS.EmployerCommitments.Domain.Configuration;
using System;
using System.Configuration;

namespace SFA.DAS.EmployerCommitments.Web.Plumbing
{
    public sealed class GoogleAnalytics
    {
        private const string ServiceName = "SFA.DAS.GoogleAnalyticsValues";

        private static GoogleAnalytics _instance;

        private GoogleAnalytics()
        {
            GetConfiguration();
        }

        public static GoogleAnalytics Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GoogleAnalytics();
                }
                return _instance;
            }
        }

        public string GoogleHeaderUrl { get; private set; }
        public string GoogleBodyUrl { get; private set; }

        private void GetConfiguration()
        {
            var environment = Environment.GetEnvironmentVariable("DASENV");
            if (string.IsNullOrEmpty(environment))
            {
                environment = CloudConfigurationManager.GetSetting("EnvironmentName");
            }

            var configurationRepository = GetConfigurationRepository();
            var configurationService = new ConfigurationService(configurationRepository,
                new ConfigurationOptions(ServiceName, environment, "1.0"));

            PopulateGoogleEnvironmentDetails(configurationService.Get<GoogleAnalyticsConfiguration>());
        }

        private void PopulateGoogleEnvironmentDetails(GoogleAnalyticsConfiguration environmentConfig)
        {
            GoogleHeaderUrl = environmentConfig.GoogleAnalyticsValues.GoogleHeaderUrl;
            GoogleBodyUrl = environmentConfig.GoogleAnalyticsValues.GoogleBodyUrl;
        }

        private static IConfigurationRepository GetConfigurationRepository()
        {
            IConfigurationRepository configurationRepository;
            if (bool.Parse(ConfigurationManager.AppSettings["LocalConfig"]))
            {
                configurationRepository = new FileStorageConfigurationRepository();
            }
            else
            {
                configurationRepository = new AzureTableStorageConfigurationRepository(CloudConfigurationManager.GetSetting("ConfigurationStorageConnectionString"));
            }
            return configurationRepository;
        }
    }
}