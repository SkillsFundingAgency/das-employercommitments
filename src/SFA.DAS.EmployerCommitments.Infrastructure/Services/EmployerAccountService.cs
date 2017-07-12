using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;
using SFA.DAS.EmployerCommitments.Domain.Models.AccountTeam;
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

        public async Task<List<LegalEntityViewModel>> GetLegalEntitiesForAccount(string accountId)
        {
            var listOfEntities = await _client.GetLegalEntitiesConnectedToAccount(accountId);

            var list = new List<LegalEntityViewModel>();

            if (listOfEntities.Count == 0)
                return list;

            foreach (var entity in listOfEntities)
            {
                list.Add(await _client.GetLegalEntity(accountId,Convert.ToInt64(entity.Id)));
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
