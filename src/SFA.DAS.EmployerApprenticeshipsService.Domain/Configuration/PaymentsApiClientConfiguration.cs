using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.Provider.Events.Api.Client;

namespace SFA.DAS.EmployerCommitments.Domain.Configuration
{
    public class PaymentsApiClientConfiguration : IPaymentsEventsApiConfiguration, IConfiguration
    {
        public string ClientToken { get; set; }
        public string ApiBaseUrl { get; set; }
        public string DatabaseConnectionString { get; set; }
        public string ServiceBusConnectionString { get; set; }
        public bool PaymentsDisabled { get; set; }
    }
}