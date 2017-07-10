﻿using System.Collections.Generic;
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

        private readonly IApprenticeshipInfoServiceWrapper _apprenticeshipInfoService;

        private readonly CommitmentNotificationConfiguration _configuration;

        public ProviderEmailLookupService(
            ILog logger,
            IdamsEmailServiceWrapper idamsEmailServiceWrapper,
            EmployerApprenticeshipsServiceConfiguration employerConfiguration,
            IApprenticeshipInfoServiceWrapper apprenticeshipInfoService)
        {
            _logger = logger;
            _idamsEmailServiceWrapper = idamsEmailServiceWrapper;
            _apprenticeshipInfoService = apprenticeshipInfoService;
            _configuration = employerConfiguration.CommitmentNotification;
        }

        public async Task<List<string>> GetEmailsAsync(long providerId, string lastUpdateEmail)
        {
            List<string> addresses;
            if (!_configuration.UseProviderEmail)
            {
                _logger.Info($"Getting provider test email (${string.Join(", ", _configuration.ProviderTestEmails)})");
                return _configuration.ProviderTestEmails;
            }

            if (!string.IsNullOrEmpty(lastUpdateEmail))
                return new List<string> { lastUpdateEmail };

            addresses = await _idamsEmailServiceWrapper.GetEmailsAsync(providerId);
            if (addresses.Any())
                return addresses;

            addresses = await _idamsEmailServiceWrapper.GetSuperUserEmailsAsync(providerId);
            if (addresses.Any())
                return addresses;

            if (GetProviderAddresses(providerId, out addresses))
                return addresses;

            if (!addresses.Any())
                _logger.Warn($"Could not find any email adresses for provider: {providerId}");

            return addresses;
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