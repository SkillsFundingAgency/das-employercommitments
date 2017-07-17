using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.EAS.Account.Api.Client;
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
                    Name=legalEntityViewModel.Name,
                    RegisteredAddress = legalEntityViewModel.Address,
                    Source =  legalEntityViewModel.SourceNumeric,
                    AgreementStatus = (EmployerAgreementStatus)legalEntityViewModel.AgreementStatus,
                    Code = legalEntityViewModel.Code,
                    Id = legalEntityViewModel.LegalEntityId
                });
            }

            return list;
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
