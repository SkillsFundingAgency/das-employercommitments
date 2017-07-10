using System.Threading.Tasks;
using SFA.DAS.EmployerCommitments.Domain.Models.PAYE;

namespace SFA.DAS.EmployerCommitments.Domain.Interfaces
{
    public interface IDasAccountService
    {
        Task<PayeSchemes> GetAccountSchemes(long accountId);
        Task UpdatePayeScheme(string empRef);
    }
}