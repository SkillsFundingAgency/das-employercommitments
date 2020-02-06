﻿using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EmployerCommitments.Domain.Models.AccountTeam;
using SFA.DAS.EmployerCommitments.Domain.Models.Organisation;

namespace SFA.DAS.EmployerCommitments.Domain.Interfaces
{
    public interface IEmployerAccountService
    {
        Task<Account> GetAccount(long accountId);
        Task<TeamMember> GetUserRoleOnAccount(string accountId, string userId);
        Task<List<LegalEntity>> GetLegalEntitiesForAccount(string accountId);
        Task<List<TransferConnection>> GetTransferConnectionsForAccount(string accountId);
    }
}
