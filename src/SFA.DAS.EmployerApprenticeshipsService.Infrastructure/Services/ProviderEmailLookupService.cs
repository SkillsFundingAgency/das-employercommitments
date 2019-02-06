using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.EmployerCommitments.Domain.Configuration;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Infrastructure.Data;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerCommitments.Infrastructure.Services
{
    public class ProviderEmailLookupService : IProviderEmailLookupService
    {
        private readonly ILog _logger;

        private readonly IdamsEmailServiceWrapper _idamsEmailServiceWrapper;

        private readonly IApprenticeshipInfoService _apprenticeshipInfoService;

        private readonly CommitmentNotificationConfiguration _configuration;

        public ProviderEmailLookupService(
            ILog logger,
            IdamsEmailServiceWrapper idamsEmailServiceWrapper,
            EmployerCommitmentsServiceConfiguration employerConfiguration,
            IApprenticeshipInfoService apprenticeshipInfoService)
        {
            _logger = logger;
            _idamsEmailServiceWrapper = idamsEmailServiceWrapper;
            _apprenticeshipInfoService = apprenticeshipInfoService;
            _configuration = employerConfiguration.CommitmentNotification;
        }

        public async Task<List<string>>GetEmailsAsync(long providerId, string lastUpdateEmail)
        {
            await Task.Delay(0);
            throw new NotImplementedException();
            
        }

        private bool GetProviderAddresses(long providerId, out List<string> addresses)
        {
            addresses = new List<string>();
            var providers = _apprenticeshipInfoService.GetProvider(providerId);
            if (!string.IsNullOrEmpty(providers?.Provider?.Email))
            {
                _logger.Info($"Getting email from apprenticeship provider service");
                addresses = new List<string> { providers?.Provider?.Email };
            }
            return addresses.Any();
        }
    }
}
