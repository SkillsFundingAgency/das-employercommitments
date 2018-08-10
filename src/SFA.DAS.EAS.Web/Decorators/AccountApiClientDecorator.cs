using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.HashingService;

namespace SFA.DAS.EmployerCommitments.Web.Decorators
{
    public class AccountApiClientDecorator : IAccountApiClient
    {
        private readonly IAccountApiClient _inner;
        private readonly IHashingService _hashingService;

        public AccountApiClientDecorator(IAccountApiClient inner, IHashingService hashingService)
        {
            _inner = inner;
            _hashingService = hashingService;
        }

        public async Task<AccountDetailViewModel> GetAccount(string hashedAccountId)
        {
            return await _inner.GetAccount(hashedAccountId);
        }

        public async Task<AccountDetailViewModel> GetAccount(long accountId)
        {
            return await _inner.GetAccount(accountId);
        }

        public Task<ICollection<AccountLegalEntityViewModel>> GetAccountLegalEntities(string highWaterMarkId = null)
        {
            return _inner.GetAccountLegalEntities(highWaterMarkId);
        }

        public async Task<PagedApiResponseViewModel<AccountWithBalanceViewModel>> GetPageOfAccounts(int pageNumber = 1, int pageSize = 1000, DateTime? toDate = null)
        {
            return await _inner.GetPageOfAccounts(pageNumber, pageSize, toDate);
        }

        public async Task<ICollection<TeamMemberViewModel>> GetAccountUsers(string accountId)
        {
            return await _inner.GetAccountUsers(accountId);
        }

        public async Task<ICollection<TeamMemberViewModel>> GetAccountUsers(long accountId)
        {
            return await _inner.GetAccountUsers(accountId);
        }

        public async Task<ICollection<AccountDetailViewModel>> GetUserAccounts(string userId)
        {
            return await _inner.GetUserAccounts(userId);
        }

        public Task<StatisticsViewModel> GetStatistics()
        {
            return _inner.GetStatistics();
        }

        public async Task<LegalEntityViewModel> GetLegalEntity(string accountId, long id)
        {
            return await _inner.GetLegalEntity(accountId, id);
        }

        public async Task<ICollection<ResourceViewModel>> GetLegalEntitiesConnectedToAccount(string accountId)
        {
            return await _inner.GetLegalEntitiesConnectedToAccount(accountId);
        }

        public async Task<ICollection<ResourceViewModel>> GetPayeSchemesConnectedToAccount(string accountId)
        {
            return await _inner.GetPayeSchemesConnectedToAccount(accountId);
        }

        public async Task<ICollection<LevyDeclarationViewModel>> GetLevyDeclarations(string accountId)
        {
            return await _inner.GetLevyDeclarations(accountId);
        }

        public async Task<EmployerAgreementView> GetEmployerAgreement(string accountId, string legalEntityId, string agreementId)
        {
            return await _inner.GetEmployerAgreement(accountId, legalEntityId, agreementId);
        }

        public async Task<TransactionsViewModel> GetTransactions(string accountId, int year, int month)
        {
            return await _inner.GetTransactions(accountId, year, month);
        }

        public async Task<ICollection<TransactionSummaryViewModel>> GetTransactionSummary(string accountId)
        {
            return await _inner.GetTransactionSummary(accountId);
        }

        public async Task<T> GetResource<T>(string uri) where T : IAccountResource
        {
            return await _inner.GetResource<T>(uri);
        }

        public Task<ICollection<TransferConnectionViewModel>> GetTransferConnections(string accountId)
        {
            var result = new List<TransferConnectionViewModel>();

            var recordsSetting = CloudConfigurationManager.GetSetting("AccountApiTransfersStubRecordCount") ?? "0";

            if (int.TryParse(recordsSetting, out var records))
            {
                for (var i = 1; i <= records-1; i++)
                {
                    result.Add(new TransferConnectionViewModel
                    {
                        FundingEmployerAccountId = i,
                        FundingEmployerAccountName = $"Test Transfer Connection {i}",
                        FundingEmployerHashedAccountId = _hashingService.HashValue(i)
                    });
                }
                result.Add(new TransferConnectionViewModel
                {
                    FundingEmployerAccountId = 8955,
                    FundingEmployerAccountName = "Phil's Sender Account 8955",
                    FundingEmployerHashedAccountId = _hashingService.HashValue(8955)
                });
            }

            return Task.FromResult((ICollection<TransferConnectionViewModel>)result);
        }
    }
}