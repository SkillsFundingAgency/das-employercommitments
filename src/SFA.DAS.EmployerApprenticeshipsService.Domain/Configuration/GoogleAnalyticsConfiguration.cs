using SFA.DAS.EmployerCommitments.Domain.Interfaces;

namespace SFA.DAS.EmployerCommitments.Domain.Configuration
{
    public class GoogleAnalyticsConfiguration : IConfiguration
    {
        public string DatabaseConnectionString { get; set; } // Not required
        public string ServiceBusConnectionString { get; set; } // Not required

        public GoogleAnalyticsValues GoogleAnalyticsValues { get; set; }
    }
}
