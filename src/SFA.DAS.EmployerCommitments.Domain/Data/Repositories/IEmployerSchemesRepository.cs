using System.Threading.Tasks;
using SFA.DAS.EmployerCommitments.Domain.Models.PAYE;

namespace SFA.DAS.EmployerCommitments.Domain.Data.Repositories
{
    public interface IEmployerSchemesRepository
    {
        Task<PayeSchemes> GetSchemesByEmployerId(long employerId);
        Task<PayeScheme> GetSchemeByRef(string empref);
    }
}