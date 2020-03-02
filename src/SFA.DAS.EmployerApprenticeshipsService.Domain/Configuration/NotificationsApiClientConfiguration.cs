using SFA.DAS.Notifications.Api.Client.Configuration;

namespace SFA.DAS.EmployerCommitments.Domain.Configuration
{
    public class NotificationsApiClientConfiguration : INotificationsApiClientConfiguration
    {
        public string ApiBaseUrl { get; set; }
        public string ClientToken { get; set; }
        public string DatabaseConnectionString { get; set; }
        public string ServiceBusConnectionString { get; set; }
        public string Tenant { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string IdentifierUri { get; set; }
    }
}
