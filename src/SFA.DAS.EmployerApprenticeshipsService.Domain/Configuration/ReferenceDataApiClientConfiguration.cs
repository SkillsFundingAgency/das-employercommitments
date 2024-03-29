﻿using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.ReferenceData.Api.Client;

namespace SFA.DAS.EmployerCommitments.Domain.Configuration
{
    public class ReferenceDataApiClientConfiguration : IReferenceDataApiConfiguration, IConfiguration
    {
        public string ApiBaseUrl { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string IdentifierUri { get; set; }
        public string Tenant { get; set; }
        public string DatabaseConnectionString { get; set; }
        public string ServiceBusConnectionString { get; set; }
    }
}
