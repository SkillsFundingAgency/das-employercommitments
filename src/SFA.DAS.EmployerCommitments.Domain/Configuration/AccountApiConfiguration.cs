using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;

namespace SFA.DAS.EmployerCommitments.Domain.Configuration
{
    public class AccountApiConfiguration : IAccountApiConfiguration, IConfiguration
    {
        public string ApiBaseUrl { get; }
        public string ClientId { get; }
        public string ClientSecret { get; }
        public string IdentifierUri { get; }
        public string Tenant { get; }
        public string DatabaseConnectionString { get; set; }
        public string ServiceBusConnectionString { get; set; }
    }
}
