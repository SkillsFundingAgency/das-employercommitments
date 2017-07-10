using System.Threading.Tasks;

namespace SFA.DAS.EmployerCommitments.Domain.Interfaces
{
    public interface IEmpRefFileBasedService
    {
        Task<string> GetEmpRef(string email, string id);
    }
}
