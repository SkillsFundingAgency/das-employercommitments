using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EmployerCommitments.Domain.Interfaces;

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

        public async Task<TeamMemberViewModel> GetUserRoleOnAccount(string expectedAccountId, string expectedUserId)
        {
            var accounts = await _client.GetAccountUsers(expectedAccountId);

            if (accounts == null || !accounts.Any())
            {
                return null;
            }

            var teamMember = accounts.FirstOrDefault(c => c.UserRef.Equals(expectedUserId, StringComparison.CurrentCultureIgnoreCase));

            return teamMember;
        }
    }
}
