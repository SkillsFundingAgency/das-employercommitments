﻿using System.Threading.Tasks;
using SFA.DAS.PAS.Account.Api.Client;
using SFA.DAS.PAS.Account.Api.Types;

namespace SFA.DAS.EmployerCommitments.Infrastructure.Services
{
    public interface IProviderNotifyService
    {
        Task SendProviderEmailNotifications(long ukprn, ProviderEmailRequest request);
    }

    public class ProviderNotifyService : IProviderNotifyService
    {
        private readonly IPasAccountApiClient _accountApiClient;

        public ProviderNotifyService(IPasAccountApiClient accountApiClient)
        {
            _accountApiClient = accountApiClient;
        }

        public async Task SendProviderEmailNotifications(long ukprn, ProviderEmailRequest request)
        {
            await _accountApiClient.SendEmailToAllProviderRecipients(ukprn, request);
        }

    }
}
