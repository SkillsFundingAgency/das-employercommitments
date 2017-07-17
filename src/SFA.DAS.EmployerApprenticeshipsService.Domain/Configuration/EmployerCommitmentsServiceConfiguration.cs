using System.Collections.Generic;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;

namespace SFA.DAS.EmployerCommitments.Domain.Configuration
{
    public class EmployerCommitmentsServiceConfiguration : IConfiguration
    {
        public string ServiceBusConnectionString { get; set; }
        public IdentityServerConfiguration Identity { get; set; }
        public string DashboardUrl { get; set; }
        public string DatabaseConnectionString { get; set; }
        public CommitmentsApiClientConfiguration CommitmentsApi { get; set; }
        public EventsApiClientConfiguration EventsApi { get; set; }
        public string Hashstring { get; set; }
        public ApprenticeshipInfoServiceConfiguration ApprenticeshipInfoService { get; set; }
		public CommitmentNotificationConfiguration CommitmentNotification { get; set; }
    }

    public class CommitmentNotificationConfiguration
    {
        public bool UseProviderEmail { get; set; }

        public bool SendEmail { get; set; }

        public List<string> ProviderTestEmails { get; set; }

        public string IdamsListUsersUrl { get; set; }

        public string DasUserRoleId { get; set; }

        public string SuperUserRoleId { get; set; }

        public string ClientToken { get; set; }
    }

    public class ApprenticeshipInfoServiceConfiguration : IApprenticeshipInfoServiceConfiguration
    {
        public string BaseUrl { get; set; }
    }

    
}