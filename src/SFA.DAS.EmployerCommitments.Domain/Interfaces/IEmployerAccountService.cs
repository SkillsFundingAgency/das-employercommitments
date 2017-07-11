using System.Threading.Tasks;
using SFA.DAS.EAS.Account.Api.Types;

namespace SFA.DAS.EmployerCommitments.Domain.Interfaces
{
    public interface IEmployerAccountService
    {
        Task<TeamMemberViewModel> GetUserRoleOnAccount(string expectedAccountId, string expectedUserId);
    }
}
