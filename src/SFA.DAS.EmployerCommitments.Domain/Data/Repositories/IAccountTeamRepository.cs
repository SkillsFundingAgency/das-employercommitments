using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EmployerCommitments.Domain.Models.AccountTeam;

namespace SFA.DAS.EmployerCommitments.Domain.Data.Repositories
{
    public interface IAccountTeamRepository
    {
        Task<List<TeamMember>> GetAccountTeamMembersForUserId(string hashedAccountId, string externalUserId);
        Task<TeamMember> GetMember(string hashedAccountId, string email);
        Task<ICollection<TeamMember>> GetAccountTeamMembers(string hashedAccountId);
    }
}
