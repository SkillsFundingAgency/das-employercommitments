using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EmployerCommitments.Domain.Extensions;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.AccountTeam;
using SFA.DAS.EmployerCommitments.Domain.Models.Organisation;
using SFA.DAS.EmployerCommitments.Domain.Models.UserProfile;

namespace SFA.DAS.EmployerCommitments.Infrastructure.Services
{
    public class EmployerAccountService : IEmployerAccountService
    {
        private readonly IAccountApiClient _client;

        public EmployerAccountService(IAccountApiClient client)
        {
            _client = client;
        }

        public async Task<Account> GetAccount(long accountId)
        {
            var account = await _client.GetAccount(accountId);
            var response = new Account
            {
                Id = account.AccountId,
                ApprenticeshipEmployerType = account.ApprenticeshipEmployerType.ToEnum<ApprenticeshipEmployerType>()
            };

            return response;
        }

        public async Task<List<LegalEntity>> GetLegalEntitiesForAccount(string accountId)
        {
            var listOfEntities = await _client.GetLegalEntitiesConnectedToAccount(accountId);

            var list = new List<LegalEntity>();

            if (listOfEntities.Count == 0)
                return list;

            foreach (var entity in listOfEntities)
            {
                var legalEntityViewModel = await _client.GetLegalEntity(accountId,Convert.ToInt64(entity.Id));

                list.Add(new LegalEntity
                {
                    Name = legalEntityViewModel.Name,
                    RegisteredAddress = legalEntityViewModel.Address,
                    Source =  legalEntityViewModel.SourceNumeric,
                    Agreements = 
                        legalEntityViewModel.Agreements.Select(agreementSource => new Agreement
                        {
                            Id  = agreementSource.Id,
                            SignedDate = agreementSource.SignedDate,
                            SignedByName = agreementSource.SignedByName,
                            Status = (EmployerAgreementStatus) agreementSource.Status,
                            TemplateVersionNumber = agreementSource.TemplateVersionNumber
                        }).ToList(),
                    Code = legalEntityViewModel.Code,
                    Id = legalEntityViewModel.LegalEntityId,
                    AccountLegalEntityPublicHashedId = legalEntityViewModel.AccountLegalEntityPublicHashedId
                });
            }

            return list;
        }

        public async Task<List<TransferConnection>> GetTransferConnectionsForAccount(string hashedAccountId)
        {
            var listOfTransferConnections = await _client.GetTransferConnections(hashedAccountId);

            if (listOfTransferConnections == null)
            {
                return new List<TransferConnection>();
            }

            return listOfTransferConnections.Select(x => new TransferConnection
            {
                AccountId = x.FundingEmployerAccountId,
                AccountName = x.FundingEmployerAccountName
            }).ToList();
        }
        
        public async Task<TeamMember> GetUserRoleOnAccount(string accountId, string userId)
        {
            var accounts = await _client.GetAccountUsers(accountId);

            if (accounts == null || !accounts.Any())
            {
                return null;
            }

            var teamMember = accounts.FirstOrDefault(c => c.UserRef.Equals(userId, StringComparison.CurrentCultureIgnoreCase));

            if (teamMember == null)
            {
                return null;
            }

            Role usrRoleResult;
            Enum.TryParse(teamMember.Role, true, out usrRoleResult);

            return new TeamMember
            {
                HashedAccountId = accountId,
                Email = teamMember.Email,
                UserRef= teamMember.UserRef,
                Role = usrRoleResult
            };
        }
    }
}
