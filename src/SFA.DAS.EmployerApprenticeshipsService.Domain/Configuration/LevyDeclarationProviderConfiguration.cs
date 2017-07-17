using SFA.DAS.EmployerCommitments.Domain.Interfaces;

namespace SFA.DAS.EmployerCommitments.Domain.Configuration
{
    public class LevyDeclarationProviderConfiguration : IConfiguration
    {
        public string DatabaseConnectionString { get; set; }
        public string ServiceBusConnectionString { get; set; }
    }
}