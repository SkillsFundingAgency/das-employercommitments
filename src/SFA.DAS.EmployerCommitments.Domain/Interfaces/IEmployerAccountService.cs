using System.Threading.Tasks;
using SFA.DAS.EmployerCommitments.Domain.Models.AccountTeam;

namespace SFA.DAS.EmployerCommitments.Domain.Interfaces
{
    public interface IEmployerAccountService
    {
        Task<TeamMember> GetUserRoleOnAccount(string accountId, string userId);
    }
}
