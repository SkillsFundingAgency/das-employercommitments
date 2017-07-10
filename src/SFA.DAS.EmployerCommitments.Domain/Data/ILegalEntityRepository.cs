using System.Threading.Tasks;
using SFA.DAS.EmployerCommitments.Domain.Data.Entities.Account;

namespace SFA.DAS.EmployerCommitments.Domain.Data
{
    public interface ILegalEntityRepository
    {
        Task<LegalEntityView> GetLegalEntityById(long id);
    }
}
