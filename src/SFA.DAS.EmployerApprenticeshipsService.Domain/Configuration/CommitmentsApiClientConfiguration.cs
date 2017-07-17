using SFA.DAS.Commitments.Api.Client.Configuration;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;

namespace SFA.DAS.EmployerCommitments.Domain.Configuration
{
    public class CommitmentsApiClientConfiguration : ICommitmentsApiClientConfiguration, IConfiguration
    {
        public string BaseUrl { get; set; }
        public string ClientToken { get; set; }
        public string DatabaseConnectionString { get; set; }
        public string ServiceBusConnectionString { get; set; }
    }
}