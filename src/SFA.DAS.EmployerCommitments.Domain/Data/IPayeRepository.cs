using System.Threading.Tasks;
using SFA.DAS.EmployerCommitments.Domain.Data.Entities.Account;
using SFA.DAS.EmployerCommitments.Domain.Models.PAYE;

namespace SFA.DAS.EmployerCommitments.Domain.Data
{
    public interface IPayeRepository
    {
        Task<PayeSchemeView> GetPayeForAccountByRef(string hashedAccountId, string payeRef);
        Task<Paye> GetPayeSchemeByRef(string payeRef);
        Task UpdatePayeSchemeName(string payeRef, string refName);
    }
}
